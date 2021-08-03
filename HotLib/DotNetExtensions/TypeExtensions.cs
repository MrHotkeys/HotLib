using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="Type"/> and <see cref="IEnumerable{T}"/> of <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets whether or not the type is declared statically.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if static, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static bool IsStatic(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsAbstract && type.IsSealed;
        }

        /// <summary>
        /// Gets all instance fields and properties (public and non-public) from the type as an enumerable.
        /// </summary>
        /// <param name="type">The type to go through.</param>
        /// <param name="ignoreBackingFields">If true, backing fields will be ignored and not returned (based on their names).</param>
        /// <returns>All instance fields and properties (minus backing fields if requested).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static IEnumerable<MemberInfo> GetInstanceFieldsAndProperties(this Type type, bool ignoreBackingFields = true)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Get all instance fields and properties
            var result = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);

            // Check if we're ignoring backing fields
            if (ignoreBackingFields)
                return result.Where(m => !m.Name.EndsWith(">k__BackingField"));
            else
                return result;
        }

        /// <summary>
        /// Gets whether variables the given type can be set to null.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if can be set to null, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static bool CanBeSetToNull(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Gets whether the <see cref="Type"/> implements an interface or any interfaces that extend the interface.
        /// </summary>
        /// <typeparam name="InterfaceType">The type of interface to check for.</typeparam>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True if the type implements the given interface or an extending interface, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static bool HasInterface<InterfaceType>(this Type type)
            where InterfaceType : class
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return typeof(InterfaceType).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the default value for the <see cref="Type"/>. Similar to using default, but can by used dynamically with any type at runtime.
        /// </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>The default value for the given type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static object? GetDefault(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Gets every <see cref="Type"/> in the enumerable that has an attribute of the given
        /// type or an attribute that extends or implements the given type.
        /// </summary>
        /// <typeparam name="AttributeType">The attribute to check for.</typeparam>
        /// <param name="types">The enumerable of members to filter.</param>
        /// <returns>Every Type wtih the attribute.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/> is null.</exception>
        public static IEnumerable<Type> WithAttribute<AttributeType>(this IEnumerable<Type> types)
            where AttributeType : Attribute
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            return types.Where(m => m.GetCustomAttributes<AttributeType>().Any());
        }

        /// <summary>
        /// Checks if the <see cref="Type"/> either is the constructed version of the given open generic
        /// <see cref="Type"/>, or if its base <see cref="Type"/> (and so forth recursively) or interfaces are.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <param name="openGenericType">The open generic <see cref="Type"/> to search for. Must be open - no type parameters.</param>
        /// <returns>True if <paramref name="type"/> is at any point constructed from </returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="openGenericType"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="openGenericType"/> is not an open generic type.</exception>
        public static bool HasOpenGenericTypeInHierarchy(this Type type, Type openGenericType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (openGenericType == null)
                throw new ArgumentNullException(nameof(openGenericType));
            if (!openGenericType.IsGenericTypeDefinition)
                throw new ArgumentException("Must be an open generic type!", nameof(openGenericType));

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == openGenericType)
                return true;

            return openGenericType.IsInterface ?
                   type.GetInterfaces().Any(i => i.HasOpenGenericTypeInHierarchy(openGenericType)) :
                   type.BaseType != null && type.BaseType.HasOpenGenericTypeInHierarchy(openGenericType);
        }

        /// <summary>
        /// Checks if the type has a public constructor with no parameters.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if it has a public, parameterless constructor, false otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static bool HasPublicParameterlessConstructor(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var flags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetConstructor(flags, null, Type.EmptyTypes, Array.Empty<ParameterModifier>()) != null;
        }

        /// <summary>
        /// Gets if the type is a value tuple type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if it is a value tuple type, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static bool IsValueTupleType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsGenericType)
                return false;

            if (!type.IsGenericTypeDefinition)
                type = type.GetGenericTypeDefinition();

            return type == typeof(ValueTuple<>) ||
                   type == typeof(ValueTuple<,>) ||
                   type == typeof(ValueTuple<,,>) ||
                   type == typeof(ValueTuple<,,,>) ||
                   type == typeof(ValueTuple<,,,,>) ||
                   type == typeof(ValueTuple<,,,,,>) ||
                   type == typeof(ValueTuple<,,,,,,>) ||
                   type == typeof(ValueTuple<,,,,,,,>);
        }

        /// <summary>
        /// Gets the type arguments to the given value tuple type. Supports value tuple types of all lengths.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>An array containing all type arguments.</returns>
        /// <exception cref="ArgumentException"><paramref name="type"/> is not a value tuple.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public static Type[] GetValueTupleTypeArguments(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!type.IsValueTupleType())
                throw new ArgumentException(nameof(type));

            var types = type.GetGenericArguments();

            if (types.Length == 8 && types[7].IsValueTupleType())
                types = types.Take(7).Concat(types[7].GetValueTupleTypeArguments()).ToArray();

            return types;
        }
        
        /// <summary>
        /// Gets the field or property with the given name. Searches all public and nonpublic instance members.
        /// </summary>
        /// <param name="type">The type to search through for the field or property.</param>
        /// <param name="memberName">The name of the field or property being searched for.</param>
        /// <returns>The found field or property.</returns>
        /// <exception cref="ArgumentException">No field or property found. -or-
        ///     Multiple fields and properties found with the given name.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="memberName"/> is null.</exception>
        public static MemberInfo GetFieldOrProperty(this Type type, string memberName)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (memberName == null)
                throw new ArgumentNullException(nameof(memberName));

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var members = type.GetMember(memberName, flags)
                              .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property);
            if (!members.Any())
                throw new ArgumentException($"Type {type.FullName} has no field or property with name {memberName}!", nameof(memberName));
            else if (members.Skip(1).Any())
                throw new ArgumentException($"Type {type.FullName} has multiple fields/properties with name {memberName}!", nameof(memberName));
            else
                return members.Single();
        }

        /// <summary>
        /// Determines whether two <see cref="Type"/> objects represent the same type.
        /// </summary>
        /// <param name="type">The current <see cref="Type"/> to check with for equality.</param>
        /// <param name="other">The other <see cref="Type"/> to check against for equality.</param>
        /// <param name="comparisonMethod">The method to use when comparing two <see cref="Type"/> objects.</param>
        /// <returns>True if the types are found to be equal, false if not.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonMethod"/> is
        ///     not a valid value in <see cref="TypeComparison"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="other"/> is null.</exception>
        public static bool Equals(this Type type, Type other, TypeComparison comparisonMethod)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            switch (comparisonMethod)
            {
                case TypeComparison.Default:
                    return type.Equals(other);
                case TypeComparison.AssemblyQualifiedName:
                    return string.Equals(type.AssemblyQualifiedName, other.AssemblyQualifiedName, StringComparison.Ordinal);
                default:
                    throw new ArgumentException($"Invalid {nameof(TypeComparison)} value \"{comparisonMethod}\"!");
            }
        }

        /// <summary>
        /// Determines whether the current <see cref="Type"/> derives from the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check for being a subclass.</param>
        /// <param name="other">The <see cref="Type"/> to check for being the superclass.</param>
        /// <param name="comparisonMethod">The method to use when comparing two <see cref="Type"/> objects.</param>
        /// <returns>True if the type is a subclass, false if not.</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonMethod"/> is
        ///     not a valid value in <see cref="TypeComparison"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="other"/> is null.</exception>
        public static bool IsSubclassOf(this Type type, Type other, TypeComparison comparisonMethod)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            switch (comparisonMethod)
            {
                case TypeComparison.Default:
                    return type.IsSubclassOf(other);
                case TypeComparison.AssemblyQualifiedName:
                    {
                        var current = type;
                        while (current != null)
                        {
                            if (string.Equals(current.AssemblyQualifiedName, other.AssemblyQualifiedName, StringComparison.Ordinal))
                                return true;

                            if (current != typeof(object))
                                current = current.BaseType;
                            else
                                break;
                        }

                        return false;
                    }
                default:
                    throw new ArgumentException($"Invalid {nameof(TypeComparison)} value \"{comparisonMethod}\"!");
            }
        }
    }
}
