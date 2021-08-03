using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="MemberInfo"/> and <see cref="IEnumerable{T}"/> of <see cref="MemberInfo"/>.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets whether or not the member is declared statically. Supports <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>,
        /// <see cref="MethodInfo"/>, <see cref="ConstructorInfo"/>, <see cref="EventInfo"/>, <see cref="TypeInfo"/>, and <see cref="Type"/>.
        /// Custom member types will cause a <see cref="ArgumentException"/> to be thrown.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if static, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        /// <exception cref="NotSupportedException">The given member is a custom member type.</exception>
        public static bool IsStatic(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.IsStatic;
                case PropertyInfo propertyInfo:
                    return propertyInfo.IsStatic();
                case MethodInfo methodInfo:
                    return methodInfo.IsStatic;
                case ConstructorInfo constructorInfo:
                    return constructorInfo.IsStatic;
                case EventInfo eventInfo:
                    return eventInfo.AddMethod?.IsStatic ??
                        eventInfo.RemoveMethod?.IsStatic ??
                        throw new InvalidOperationException("Cannot determine if event with no add or remove method is static!");
                case TypeInfo typeInfo:
                    return typeInfo.IsStatic();
                case Type type:
                    return type.IsStatic();
                default:
                    throw new ArgumentException($"Attempt to check static-ness on unsupported member type {member.GetType()}!");
            }
        }

        /// <summary>
        /// If the member is a field or a property, returns the type of value stored in the field or property.
        /// If not, throws an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>The type stored in the member.</returns>
        /// <exception cref="ArgumentException"><paramref name="member"/> is not a field or property.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.FieldType;
                case PropertyInfo propertyInfo:
                    return propertyInfo.PropertyType;
                default:
                    throw new ArgumentException("This only works when the member is a field or property!");
            }
        }

        /// <summary>
        /// If the member is a field or a property, returns the value stored in the field or property. 
        /// If not, throws an <see cref="ArgumentException"/>.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <param name="target">The target object containing the member to check.</param>
        /// <returns>The value stored in the member.</returns>
        /// <exception cref="ArgumentException"><paramref name="member"/> is not a field or property.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        public static object? GetFieldOrPropertyValue(this MemberInfo member, object target)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (member)
            {
                case FieldInfo fieldInfo:
                    return fieldInfo.GetValue(target);
                case PropertyInfo propertyInfo:
                    return propertyInfo.GetValue(target);
                default:
                    throw new ArgumentException("This only works when the member is a field or property!");
            }
        }

        /// <summary>
        /// If the member is a field or property, sets that member's value. In the case of auto-properties,
        /// finds the backing field and sets its value.
        /// </summary>
        /// <param name="member">The member being set.</param>
        /// <param name="target">The target instance containing the member being set.</param>
        /// <param name="value">The value to set the member to.</param>
        /// <exception cref="ArgumentException"><paramref name="member"/> is a non-auto property with no setter.</exception>
        /// <exception cref="ArgumentException"><paramref name="member"/> is not a field or property.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> or <paramref name="target"/> is null.</exception>
        public static void SetFieldOrPropertyValue(this MemberInfo member, object target, object value)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            switch (member)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    break;
                case PropertyInfo property:
                    property.SetValue(target, value, true);
                    break;
                default:
                    throw new ArgumentException($"Can only set fields or properties, not {member.MemberType}!", nameof(member));
            }
        }

        /// <summary>
        /// True if the member is a field or property, false if not.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if the member is a field or property, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        public static bool IsFieldOrProperty(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            return member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property;
        }

        /// <summary>
        /// Checks whether or not the member is decorated with the at least one attribute of the given type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="member">The member to check.</param>
        /// <returns>True if the member has the attribute, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        public static bool HasCustomAttribute<T>(this MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            return member.GetCustomAttributes().OfType<T>().Any();
        }

        /// <summary>
        /// Tries to get the single custom attribute on the member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="member">The member to check.</param>
        /// <param name="attribute">The found attribute.</param>
        /// <returns>True if the member has a single matching attribute, false if there are zero matches or more than one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="member"/> is null.</exception>
        public static bool TryGetCustomAttribute<T>(this MemberInfo member, out T? attribute)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var matches = member.GetCustomAttributes().OfType<T>();
            attribute = matches.FirstOrDefault();
            return matches.HasSingle();
        }

        /// <summary>
        /// Gets every <see cref="MemberInfo"/> in the enumerable that has an attribute of the given
        /// type or an attribute that extends or implements the given type.
        /// </summary>
        /// <typeparam name="AttributeType">The attribute to check for.</typeparam>
        /// <param name="members">The enumerable of members to filter.</param>
        /// <returns>Every MemberInfo wtih the attribute.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="members"/> is null.</exception>
        public static IEnumerable<MemberInfo> WithAttribute<AttributeType>(this IEnumerable<MemberInfo> members)
        {
            if (members == null)
                throw new ArgumentNullException(nameof(members));
            return members.Where(m => m.GetCustomAttributes().Any(a => a is AttributeType));
        }
    }
}
