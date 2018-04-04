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
    /// Used to get hash codes for objects by hashing all members marked with <see cref="IncludeInHashAttribute"/>.
    /// </summary>
    public static class AutoHashCode
    {
        /// <summary>
        /// Marks a field or property to be included in calculating a hash code.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public sealed class IncludeInHashAttribute : Attribute
        { }

        private delegate int HashCodeFunction(object target, bool includeBaseTypes, IHashCodeGenerator hashCodeGenerator);

        /// <summary>
        /// Contains cached hash code functions, with the types the functions correspond to as the keys.
        /// </summary>
        private static ConcurrentDictionary<Type, HashCodeFunction> HashCodeFunctionCache { get; } =
            new ConcurrentDictionary<Type, HashCodeFunction>();

        /// <summary>
        /// Gets a hash code for an object by hashing all members marked with <see cref="IncludeInHashAttribute"/>.
        /// </summary>
        /// <param name="target">The target object to get a hash code for.</param>
        /// <param name="includeInherited">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeInHashAttribute"/>.</param>
        /// <param name="hashCodeGenerator">Used to hash member values.
        ///     If null, <see cref="DefaultComparerHashCodeGenerator"/> will be used.</param>
        /// <returns>The generated hash code.</returns>
        /// <exception cref="InvalidOperationException">No members marked with
        ///     <see cref="IncludeInHashAttribute"/> found in the target object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public static int GetHashCode(object target, bool includeInherited = true, IHashCodeGenerator hashCodeGenerator = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var type = target.GetType();
            if (!HashCodeFunctionCache.TryGetValue(type, out var func))
            {
                func = GetHashCodeFunction(type);
                HashCodeFunctionCache.TryAdd(type, func);
            }

            return func.Invoke(target, includeInherited, hashCodeGenerator ?? DefaultComparerHashCodeGenerator.Instance);
        }

        /// <summary>
        /// Gets a <see cref="HashCodeFunction"/> for hashing objects of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to get a hash code function for.</param>
        /// <exception cref="InvalidOperationException">No members marked with <see cref="IncludeInHashAttribute"/>
        ///     found in the target type.</exception>
        private static HashCodeFunction GetHashCodeFunction(Type type)
        {
            GetIncludedMembers(type, out var declaredMembers, out var inheritedMembers);
            if (declaredMembers.Length == 0 && inheritedMembers.Length == 0)
                throw new InvalidOperationException($"{type} has no members marked with {nameof(IncludeInHashAttribute)}!");

            // Represents the parameters to the hash code function - the target object, boxed as an object, whether
            // or not to include members from base types, and the hash code generator for hashing member values
            var boxedTargetParameterExpression = Expression.Parameter(typeof(object));
            var includeInheritedParameterExpression = Expression.Parameter(typeof(bool));
            var hashCodeGeneratorParameterExpression = Expression.Parameter(typeof(IHashCodeGenerator));

            // This is the expression that will give us the hash code
            // We start with a large prime and then mutate it with each member value
            var startHashCode = Expression.Constant(unchecked((int)2166136261), typeof(int));

            // Get the expression for the hash code with just declared values
            var declaredHashCodeExpression = AppendHashCodeExpression(startHashCode, declaredMembers,
                                                                  boxedTargetParameterExpression,
                                                                  hashCodeGeneratorParameterExpression);
            var declaredHashCodeVariableExpression = Expression.Variable(typeof(int), "declaredHashCode");
            var declaredHashCodeVariableAssignExpression = Expression.Assign(declaredHashCodeVariableExpression,
                                                                             declaredHashCodeExpression);

            // Get the expression for the hash code with declared and inherited values
            var allHashCodeExpression = AppendHashCodeExpression(declaredHashCodeVariableExpression, inheritedMembers,
                                                             boxedTargetParameterExpression,
                                                             hashCodeGeneratorParameterExpression);

            // The label used to return out of the block expression that represents the body of the method
            var returnLabel = Expression.Label(typeof(int));
            var returnExpression = Expression.Label(returnLabel, Expression.Constant(-1, typeof(int)));

            // Branch depending on whether we are including inherited members
            var ifIncludeInheritedExpression = Expression.IfThenElse(includeInheritedParameterExpression,
                                                       Expression.Return(returnLabel, allHashCodeExpression),
                                                       Expression.Return(returnLabel, declaredHashCodeVariableExpression));
            
            // The block expression that represents the body of the generated method
            var blockExpression = Expression.Block(new[] { declaredHashCodeVariableExpression },
                                                   declaredHashCodeVariableAssignExpression,
                                                   ifIncludeInheritedExpression,
                                                   returnExpression);

            return Expression.Lambda<HashCodeFunction>(blockExpression,
                                                       boxedTargetParameterExpression,
                                                       includeInheritedParameterExpression,
                                                       hashCodeGeneratorParameterExpression)
                             .Compile();
        }

        /// <summary>
        /// Creates and returns an expression for appending the given
        /// expression for a hash code with more member value mutations.
        /// </summary>
        /// <param name="hashCodeExpression">The hash code expression to append.</param>
        /// <param name="members">The members to append the hash code with.</param>
        /// <param name="boxedTargetParameterExpression">The expression for the target object as a parameter of type object.</param>
        /// <param name="hashCodeGeneratorExpression">The expression for the hash code generator as a parameter.</param>
        /// <returns>The appended hash code expression.</returns>
        private static Expression AppendHashCodeExpression(Expression hashCodeExpression, IEnumerable<MemberInfo> members,
                                                           ParameterExpression boxedTargetParameterExpression,
                                                           ParameterExpression hashCodeGeneratorExpression)
        {
            // A constant expression for the prime that we multiply with for each member value
            var hashMultiplyFactorExpression = Expression.Constant(16777619);

            foreach (var member in members)
            {
                // Represents the target object, as unboxed from the parameter to its actual type
                // We unbox here since we can unbox to the member's declaring type to be able to find hidden members
                var targetExpression = Expression.Convert(boxedTargetParameterExpression, member.DeclaringType);

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
        /// Gets all members marked with <see cref="IncludeInHashAttribute"/> in the given type.
        /// </summary>
        /// <param name="type">The type to get members from.</param>
        /// <param name="declared">Will be set to an array of all declared members marked
        ///     with <see cref="IncludeInHashAttribute"/>.</param>
        /// <param name="inherited">Will be set to an array of all inherited members marked
        ///     with <see cref="IncludeInHashAttribute"/>.</param>
        /// <returns>An enumerable of all found members.</returns>
        private static void GetIncludedMembers(Type type, out MemberInfo[] declared, out MemberInfo[] inherited)
        {
            if (type == typeof(object))
            {
                declared = Array.Empty<MemberInfo>();
                inherited = Array.Empty<MemberInfo>();
                return;
            }
            else
            {
                declared = type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                           BindingFlags.NonPublic | BindingFlags.Instance)
                               .Where(m => m.GetCustomAttribute(typeof(IncludeInHashAttribute)) != null)
                               .ToArray();

                var declaredProperties = declared.OfType<PropertyInfo>()
                                                 .ToArray();

                var inheritedSet = new HashSet<MemberInfo>();
                var currentType = type.BaseType;
                while (currentType != typeof(object))
                {
                    var membersWithAttribute = currentType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                                      BindingFlags.NonPublic | BindingFlags.Instance)
                                                          .Where(m => m.GetCustomAttribute(typeof(IncludeInHashAttribute)) != null);

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
                                        if (!declaredProperties.Any(p => p.GetMethod != null && p.GetMethod.IsOverrideOf(property.GetMethod)))
                                            inheritedSet.Add(property);
                                    }
                                    else if (property.SetMethod != null)
                                    {
                                        if (!declaredProperties.Any(p => p.SetMethod != null && p.SetMethod.IsOverrideOf(property.SetMethod)))
                                            inheritedSet.Add(property);
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
