using System;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="PropertyInfo"/>
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Gets whether or not the property is declared statically.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>True if static, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
        public static bool IsStatic(this PropertyInfo property)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));

            return property.GetMethod?.IsStatic ??
                   property.SetMethod?.IsStatic ??
                   throw new ArgumentException($"{property.DeclaringType}.{property} has no getter or setter!");
        }

        /// <summary>
        /// Gets the value of a property using its getter, or by getting its backing field if it is an auto-property, if requested.
        /// </summary>
        /// <param name="property">The property to get the value of.</param>
        /// <param name="target">The target instance containing the property to get the value of.</param>
        /// <param name="useBackingFieldIfNoGetter">If true and the property has no getter, its backing field will be retrieved if it is an auto-property.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is null but <paramref name="property"/> is non-static.
        ///     -or-<paramref name="property"/> is a non-auto property with no getter.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
        public static object? GetValue(this PropertyInfo property, object target, bool useBackingFieldIfNoGetter)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (target is null && !property.IsStatic())
                throw new ArgumentException($"{nameof(target)} cannot be null: {property} is non-static!");

            EnsurePropertyIsAsDeclared(ref property, target);

            var getter = property.GetGetMethod(true);
            if (getter != null)
            {
                return getter.Invoke(target, Array.Empty<object>());
            }
            else if (useBackingFieldIfNoGetter)
            {
                var backingField = property.GetBackingField(target);

                if (backingField != null)
                    return backingField.GetValue(target);
                else
                    throw new ArgumentException($"Could not locate getter or backing field for member {property.DeclaringType}.{property}!");
            }
            else
            {
                throw new ArgumentException($"{property.DeclaringType}.{property} has no setter!");
            }
        }

        /// <summary>
        /// Sets the value of a property using its setter, or by setting its backing field if it is an auto-property, if requested.
        /// </summary>
        /// <param name="property">The property being set.</param>
        /// <param name="target">The target instance containing the property being set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="useBackingFieldIfNoSetter">If true and the property has no setter, its backing field will be set if it is an auto-property.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is null but <paramref name="property"/> is non-static.
        ///     -or-<paramref name="property"/> is a non-auto property with no setter.
        ///     -or-<paramref name="property"/> cannot be assigned from <paramref name="value"/> by type.
        ///     -or-<paramref name="value"/> is null but <paramref name="property"/> cannot be set to null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
        public static void SetValue(this PropertyInfo property, object target, object? value, bool useBackingFieldIfNoSetter)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (target is null && !property.IsStatic())
                throw new ArgumentException($"{nameof(target)} cannot be null: property {property} is non-static!", nameof(target));
            
            if (value is null)
            {
                if (!property.PropertyType.CanBeSetToNull())
                    throw new ArgumentException($"Cannot set non-nullable property {property} to null!", nameof(value));
            }
            else
            {
                if (!property.PropertyType.IsAssignableFrom(value.GetType()))
                    throw new ArgumentException($"Cannot set property {property} with value of type {value.GetType()}!", nameof(value));
            }

            EnsurePropertyIsAsDeclared(ref property, target);

            var setter = property.GetSetMethod(true);

            if (setter != null)
            {
                setter.Invoke(target, new object?[] { value });
            }
            else if (useBackingFieldIfNoSetter)
            {
                var backingField = property.GetBackingField(target);

                if (backingField != null)
                    backingField.SetValue(target, value);
                else
                    throw new ArgumentException($"Could not locate setter or backing field for property {property}!", nameof(property));
            }
            else
            {
                throw new ArgumentException($"{property.DeclaringType}.{property.Name} has no setter!", nameof(property));
            }
        }

        private static void EnsurePropertyIsAsDeclared(ref PropertyInfo property, object? target)
        {
            // Make sure our property comes from its declaring type so that all
            // the metadata is there (so we can properly find private setters)
            if (target is not null)
            {
                var targetType = target.GetType();
                if (property.DeclaringType != targetType)
                {
                    if (property.DeclaringType is null)
                        throw new InvalidOperationException($"Cannot get declaring type of {property}!");

                    if (!property.DeclaringType.IsAssignableFrom(targetType))
                        throw new ArgumentException($"{targetType} does not contain property {property}!", nameof(property));

                    property = property
                        .DeclaringType
                        .GetProperty(
                            property.Name,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                        ?? throw new InvalidOperationException();
                }
            }
        }

        private static FieldInfo? GetBackingField(this PropertyInfo property, object? target)
        {
            var backingFieldTargetType =
                target?.GetType() ??
                property.DeclaringType ??
                throw new InvalidOperationException($"Cannot get declaring type of {property}!");

            return property.GetBackingField(backingFieldTargetType);
        }

        /// <summary>
        /// Gets the backing field for an auto-property.
        /// </summary>
        /// <param name="property">The property to get a backing field for.</param>
        /// <param name="type">The <see cref="Type"/> implementing the property.</param>
        /// <returns>The property's backing field, or null if no backing field found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="type"/> is null.</exception>
        public static FieldInfo? GetBackingField(this PropertyInfo property, Type type)
        {
            if (property is null)
                throw new ArgumentNullException(nameof(property));
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (property.DeclaringType is null)
                throw new ArgumentException("Cannot get backing field for global types!", nameof(type));

            if (property.DeclaringType.IsInterface)
            {
                var correspondingProperty = type.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                if (correspondingProperty != null)
                {
                    // Property is implicitly implemented
                    return correspondingProperty.GetBackingField(type);
                }
                else
                {
                    // Property is explicitly implemented
                    if (property.DeclaringType.FullName is null)
                        throw new InvalidOperationException("Cannot get backing field for type with no FullName!");

                    var backingFieldName = $"<{property.DeclaringType.FullName.Replace('+', '.')}.{property.Name}>k__BackingField";
                    return type.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            else
            {
                var backingFieldName = $"<{property.Name}>k__BackingField";
                return property.DeclaringType.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
