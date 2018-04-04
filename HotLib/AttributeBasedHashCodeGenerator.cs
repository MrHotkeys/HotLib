using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib
{
    /// <summary>
    /// Used to get hash codes for objects by hashing all members marked with <see cref="IncludeInHashCodeAttribute"/>.
    /// Contains classes for marking members as belong to hash codes and for hashing them.
    /// </summary>
    public class AttributeBasedHashCodeGenerator : IHashCodeGenerator
    {
        /// <summary>
        /// Marks a field or property to be included in calculating a hash code.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class IncludeInHashCodeAttribute : Attribute
        {
            /// <summary>
            /// Instantiates a new <see cref="IncludeInHashCodeAttribute"/>.
            /// </summary>
            public IncludeInHashCodeAttribute()
            { }
        }

        /// <summary>
        /// The delegate type for hash code functions which are used to get hash codes from
        /// objects with members marked with <see cref="IncludeInHashCodeAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of target object that the function can create hash codes from.</typeparam>
        /// <param name="target">The target object to create a hash code from.</param>
        /// <param name="includeInherited">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeInHashCodeAttribute"/>.</param>
        /// <param name="hashCodeGenerator">Used to hash member values.</param>
        /// <returns>The hash code for the given target object.</returns>
        protected delegate int HashCodeFunction<T>(T target, bool includeInherited, IHashCodeGenerator hashCodeGenerator);

        /// <summary>
        /// Contains cached hash code functions, with the types the functions correspond to as the keys.
        /// </summary>
        protected ConcurrentDictionary<Type, object> HashCodeFunctionCache { get; } = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Instantiates a new <see cref="AttributeBasedHashCodeGenerator"/>.
        /// </summary>
        /// <param name="setupTypes">An array of <see cref="Type"/> objects for types that should
        ///     have their hash codes generated immediately instead of waiting for that type's
        ///     <see cref="GetHashCode{T}(T, bool, IHashCodeGenerator)"/> to be called.</param>
        /// <exception cref="InvalidOperationException">A type in <paramref name="setupTypes"/>> has
        ///     no members marked with <see cref="IncludeInHashCodeAttribute"/> found in the target object.</exception>
        /// <exception cref="ArgumentException"><paramref name="setupTypes"/> contains a null value.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="setupTypes"/> is null.</exception>
        public AttributeBasedHashCodeGenerator(params Type[] setupTypes)
        {
            if (setupTypes == null)
                throw new ArgumentNullException(nameof(setupTypes));

            if (setupTypes.Length > 0)
            {
                // Have to use reflection since GetHashCodeFunction has a generic
                // type parameter (which it needs since HashCodeFunction does too)
                var unconstructed = typeof(AttributeBasedHashCodeGenerator).GetMethod(nameof(GetHashCodeFunction),
                                                                                      BindingFlags.DeclaredOnly |
                                                                                      BindingFlags.NonPublic |
                                                                                      BindingFlags.Instance);
                foreach (var type in setupTypes)
                {
                    if (type == null)
                        throw new ArgumentException($"Cannot contain null!", nameof(setupTypes));

                    var boxedHashCodeFunction = unconstructed.MakeGenericMethod(type)
                                                             .Invoke(this, Array.Empty<object>());
                    HashCodeFunctionCache.TryAdd(type, boxedHashCodeFunction);
                }
            }
        }

        /// <summary>
        /// Gets a hash code for an object by hashing all members marked with <see cref="IncludeInHashCodeAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of target object that the function can create hash codes from.</typeparam>
        /// <param name="target">The target object to get a hash code for.</param>
        /// <param name="includeInherited">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeInHashCodeAttribute"/>.</param>
        /// <param name="hashCodeGenerator">Used to hash member values.
        ///     If null, <see cref="DefaultComparerHashCodeGenerator"/> will be used.</param>
        /// <returns>The generated hash code.</returns>
        /// <exception cref="InvalidOperationException">No members marked with
        ///     <see cref="IncludeInHashCodeAttribute"/> found in the target object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public virtual int GetHashCode<T>(T target, bool includeInherited = true, IHashCodeGenerator hashCodeGenerator = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (!HashCodeFunctionCache.TryGetValue(typeof(T), out var boxedHashCodeFunction))
            {
                boxedHashCodeFunction = GetHashCodeFunction<T>();
                HashCodeFunctionCache.TryAdd(typeof(T), boxedHashCodeFunction);
            }

            var hashCodeFunction = boxedHashCodeFunction as HashCodeFunction<T>;
            return hashCodeFunction.Invoke(target, includeInherited,
                                           hashCodeGenerator ?? DefaultComparerHashCodeGenerator.Instance);
        }

        /// <summary>
        /// Gets a hash code from the given target object.
        /// </summary>
        /// <typeparam name="T">The type of target object to get a hash code from.</typeparam>
        /// <param name="target">The target object to get a hash code from.</param>
        /// <returns>The object's hash code.</returns>
        int IHashCodeGenerator.GetHashCode<T>(T target) => GetHashCode(target);

        /// <summary>
        /// Gets a <see cref="HashCodeFunction{T}"/> for hashing objects of the given <see cref="Type"/>.
        /// </summary>
        /// <returns>The created hash code function.</returns>
        /// <exception cref="InvalidOperationException">No members marked with <see cref="IncludeInHashCodeAttribute"/>
        ///     found in the target type.</exception>
        protected virtual HashCodeFunction<T> GetHashCodeFunction<T>()
        {
            GetIncludedMembers<T>(out var declaredMembers, out var inheritedMembers);
            if (declaredMembers.Length == 0 && inheritedMembers.Length == 0)
                throw new InvalidOperationException($"{typeof(T)} has no members marked with {nameof(IncludeInHashCodeAttribute)}!");

            // Represents the parameters to the hash code function - the target object, whether or not
            // to include members from base types, and the hash code generator for hashing member values
            var targetParameterExpression = Expression.Parameter(typeof(T));
            var includeInheritedParameterExpression = Expression.Parameter(typeof(bool));
            var hashCodeGeneratorParameterExpression = Expression.Parameter(typeof(IHashCodeGenerator));

            // This is the expression that will give us the hash code
            // We start with a large prime and then mutate it with each member value
            var startHashCode = Expression.Constant(unchecked((int)2166136261), typeof(int));

            // Get the expression for the hash code with just declared values and create a local variable to store it in
            var declaredHashCodeExpression = AppendHashCodeExpression<T>(startHashCode, declaredMembers,
                                                                         targetParameterExpression,
                                                                         hashCodeGeneratorParameterExpression);
            var declaredHashCodeVariableExpression = Expression.Variable(typeof(int), "declaredHashCode");
            var declaredHashCodeVariableAssignExpression = Expression.Assign(declaredHashCodeVariableExpression,
                                                                             declaredHashCodeExpression);

            // Get the expression for the hash code with declared and inherited values
            var allHashCodeExpression = AppendHashCodeExpression<T>(declaredHashCodeVariableExpression, inheritedMembers,
                                                                    targetParameterExpression,
                                                                    hashCodeGeneratorParameterExpression);

            // The label used to return out of the block expression that represents the body of the method
            var returnLabel = Expression.Label(typeof(int));
            var returnExpression = Expression.Label(returnLabel,
                                                    Expression.Constant(-1, typeof(int)));

            // Branch depending on whether we are including inherited members
            var ifIncludeInheritedExpression = Expression.IfThenElse(includeInheritedParameterExpression,
                                                                     Expression.Return(returnLabel, allHashCodeExpression),
                                                                     Expression.Return(returnLabel, declaredHashCodeVariableExpression));

            // Holds the group of expressions as we're working with it and conditionally adding more
            var expressions = new List<Expression>()
            {
                declaredHashCodeVariableAssignExpression,
                ifIncludeInheritedExpression,
                returnExpression,
            };

            // If there are no declared members, add a guard clause to the beginning of the hash code function so that if false
            // is passed for including inherited members, an exception would be thrown (otherwise there'd be nothing to hash)
            if (declaredMembers.Length == 0)
            {
                var exceptionMessage = $"{typeof(T)} has no members marked with {typeof(IncludeInHashCodeAttribute)}, so " +
                                       $"inherited members must be included or there will be nothing to hash!";
                var exceptionConstructor = typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) });
                var exceptionExpression = Expression.New(exceptionConstructor,
                                                         Expression.Constant(exceptionMessage));
                var throwExpression = Expression.Throw(exceptionExpression);
                var guardClauseExpression = Expression.IfThen(Expression.Not(includeInheritedParameterExpression),
                                                              throwExpression);
                expressions.Insert(0, guardClauseExpression);
            }

            // The block expression that represents the body of the generated method
            var variables = Enumerable.Repeat(declaredHashCodeVariableExpression, 1);
            var blockExpression = Expression.Block(variables, expressions);

            return Expression.Lambda<HashCodeFunction<T>>(blockExpression,
                                                          targetParameterExpression,
                                                          includeInheritedParameterExpression,
                                                          hashCodeGeneratorParameterExpression)
                             .Compile();
        }

        /// <summary>
        /// Creates and returns an expression for appending the given
        /// expression for a hash code with more member value mutations.
        /// </summary>
        /// <typeparam name="T">The type of target object that the hash code is being created for.</typeparam>
        /// <param name="hashCodeExpression">The hash code expression to append.</param>
        /// <param name="members">The members to append the hash code with.</param>
        /// <param name="targetParameterExpression">The expression for the target object as a parameter.</param>
        /// <param name="hashCodeGeneratorExpression">The expression for the hash code generator as a parameter.</param>
        /// <returns>The appended hash code expression.</returns>
        protected virtual Expression AppendHashCodeExpression<T>(Expression hashCodeExpression, IEnumerable<MemberInfo> members,
                                                              ParameterExpression targetParameterExpression,
                                                              ParameterExpression hashCodeGeneratorExpression)
        {
            // A constant expression for the prime that we multiply with for each member value
            var hashMultiplyFactorExpression = Expression.Constant(16777619);

            foreach (var member in members)
            {
                // Represents the target object, as unboxed from the parameter to its actual type
                // We unbox here since we can unbox to the member's declaring type to be able to find hidden members
                var targetExpression = member.DeclaringType != typeof(T) ?
                                       Expression.Convert(targetParameterExpression, member.DeclaringType) :
                                       targetParameterExpression as Expression;

                // Get the value of the member
                var memberAccessExpression = Expression.PropertyOrField(targetExpression, member.Name);

                // MethodInfo for IHashCodeGenerator.GetHashCode<T>(T)
                var getHashCodeTypeParameters = new[] { member.GetFieldOrPropertyType() };
                var getHashCode = typeof(IHashCodeGenerator).GetMethod(nameof(IHashCodeGenerator.GetHashCode))
                                                            .MakeGenericMethod(getHashCodeTypeParameters);

                // Get the hash for the current member from the default comparer
                var memberHashExpression = Expression.Call(hashCodeGeneratorExpression,
                                                           getHashCode,
                                                           memberAccessExpression);

                // Mutate the hash code based off the hash code for the current member
                // (currentHash * multiplyFactor) ^ memberHash
                hashCodeExpression = Expression.Multiply(hashCodeExpression, hashMultiplyFactorExpression);
                hashCodeExpression = Expression.ExclusiveOr(hashCodeExpression, memberHashExpression);
            }

            return hashCodeExpression;
        }

        /// <summary>
        /// Gets all members marked with <see cref="IncludeInHashCodeAttribute"/> in the given type.
        /// </summary>
        /// <typeparam name="T">The type to get the members from.</typeparam>
        /// <param name="declared">Will be set to an array of all declared members marked
        ///     with <see cref="IncludeInHashCodeAttribute"/>.</param>
        /// <param name="inherited">Will be set to an array of all inherited members marked
        ///     with <see cref="IncludeInHashCodeAttribute"/>.</param>
        /// <returns>An enumerable of all found members.</returns>
        protected virtual void GetIncludedMembers<T>(out MemberInfo[] declared, out MemberInfo[] inherited)
        {
            if (typeof(T) == typeof(object))
            {
                declared = Array.Empty<MemberInfo>();
                inherited = Array.Empty<MemberInfo>();
                return;
            }
            else
            {
                declared = typeof(T).GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(m => m.GetCustomAttribute(typeof(IncludeInHashCodeAttribute)) != null)
                                    .ToArray();

                if (typeof(T).IsValueType)
                {
                    inherited = Array.Empty<MemberInfo>();
                    return;
                }

                var declaredProperties = declared.OfType<PropertyInfo>()
                                                 .ToArray();

                var inheritedSet = new HashSet<MemberInfo>();
                var currentType = typeof(T).BaseType;
                while (currentType != typeof(object))
                {
                    var membersWithAttribute = currentType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                                      BindingFlags.NonPublic | BindingFlags.Instance)
                                                          .Where(m => m.GetCustomAttribute(typeof(IncludeInHashCodeAttribute)) != null);

                    foreach (var member in membersWithAttribute)
                    {
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field when member is FieldInfo field:
                                {
                                    inheritedSet.Add(field);
                                    break;
                                }
                            case MemberTypes.Property when member is PropertyInfo property:
                                {
                                    if (property.GetMethod != null)
                                    {
                                        if (!declaredProperties.Any(p => p.GetMethod != null &&
                                            p.GetMethod.IsOverrideOf(property.GetMethod)))
                                        {
                                            inheritedSet.Add(property);
                                        }
                                    }
                                    else if (property.SetMethod != null)
                                    {
                                        if (!declaredProperties.Any(p => p.SetMethod != null &&
                                            p.SetMethod.IsOverrideOf(property.SetMethod)))
                                        {
                                            inheritedSet.Add(property);
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException();
                                    }

                                    break;
                                }
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    currentType = currentType.BaseType;
                }

                inherited = inheritedSet.ToArray();
            }
        }
    }
}
