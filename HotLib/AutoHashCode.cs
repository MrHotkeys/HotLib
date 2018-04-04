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
        /// <param name="includeBaseTypes">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeInHashAttribute"/>.</param>
        /// <param name="hashCodeGenerator">Used to hash member values.
        ///     If null, <see cref="DefaultComparerHashCodeGenerator"/> will be used.</param>
        /// <returns>The generated hash code.</returns>
        /// <exception cref="InvalidOperationException">No members marked with
        ///     <see cref="IncludeInHashAttribute"/> found in the target object.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public static int GetHashCode(object target, bool includeBaseTypes = true, IHashCodeGenerator hashCodeGenerator = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var type = target.GetType();
            if (!HashCodeFunctionCache.TryGetValue(type, out var func))
            {
                func = GetHashCodeFunction(type);
                HashCodeFunctionCache.TryAdd(type, func);
            }

            return func.Invoke(target, includeBaseTypes, hashCodeGenerator ?? DefaultComparerHashCodeGenerator.Instance);
        }

        /// <summary>
        /// Gets a <see cref="HashCodeFunction"/> for hashing objects of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to get a hash code function for.</param>
        /// <exception cref="InvalidOperationException">No members marked with <see cref="IncludeInHashAttribute"/>
        ///     found in the target type.</exception>
        private static HashCodeFunction GetHashCodeFunction(Type type)
        {
            var includedMembers = GetIncludedMembers(type, true);
            if (!includedMembers.Any())
                throw new InvalidOperationException($"{type} has no members marked with {nameof(IncludeInHashAttribute)}!");

            // Represents the parameters to the hash code function - the target object, boxed as an object, whether
            // or not to include members from base types, and the hash code generator for hashing member values
            var targetParameterExpression = Expression.Parameter(typeof(object));
            var includeBaseTypesParameterExpression = Expression.Parameter(typeof(bool));
            var hashCodeGeneratorExpression = Expression.Parameter(typeof(IHashCodeGenerator));

            // This is the expression that will give us the hash code
            // We start with a large prime and then mutate it with each member value
            var hashCodeExpression = (Expression)Expression.Constant(unchecked((int)2166136261), typeof(int));

            // A constant expression for the prime that we multiply with for each member value
            var hashMultiplyFactorExpression = Expression.Constant(16777619);

            foreach (var member in includedMembers.OrderBy(m => m.Name, StringComparer.Ordinal))
            {
                // Represents the target object, as unboxed from the parameter to its actual type
                // We unbox here since we can unbox to the member's declaring type to be able to find hidden members
                var targetExpression = Expression.Convert(targetParameterExpression, member.DeclaringType);

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

            return Expression.Lambda<HashCodeFunction>(hashCodeExpression,
                                                       targetParameterExpression,
                                                       includeBaseTypesParameterExpression,
                                                       hashCodeGeneratorExpression)
                             .Compile();
        }

        /// <summary>
        /// Gets all members marked with <see cref="IncludeInHashAttribute"/> in the given type.
        /// </summary>
        /// <param name="type">The type to get members from.</param>
        /// <param name="includeBaseTypes">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeInHashAttribute"/>.</param>
        /// <returns>An enumerable of all found members.</returns>
        private static IEnumerable<MemberInfo> GetIncludedMembers(Type type, bool includeBaseTypes)
        {
            if (type == typeof(object))
                yield break;

            var currentType = type;
            do
            {
                foreach (var member in currentType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                              BindingFlags.NonPublic | BindingFlags.Instance)
                                                  .Where(m => m.GetCustomAttribute(typeof(IncludeInHashAttribute)) != null))
                {
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field when member is FieldInfo field:
                            {
                                yield return field;
                                break;
                            }
                        case MemberTypes.Property when member is PropertyInfo property:
                            {
                                // If we're including members from base types, skip this property it if it's an override so
                                // we don't get duplicates (just having the base version of overridden methods is sufficient)
                                var propertyMethod = property.GetMethod ?? property.SetMethod;
                                if (!includeBaseTypes || propertyMethod.GetBaseDefinition() == propertyMethod)
                                    yield return property;

                                break;
                            }

                    }
                }

                currentType = currentType.BaseType;
            }
            while (includeBaseTypes && currentType != typeof(object));
        }
    }
}
