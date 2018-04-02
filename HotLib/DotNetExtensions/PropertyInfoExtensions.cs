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
            if (property == null)
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
        /// <exception cref="ArgumentException"><paramref name="property"/> is a non-auto property with no getter.</exception>
        public static object GetValue(this PropertyInfo property, object target, bool useBackingFieldIfNoGetter)
        {
            var getter = property.GetGetMethod(true);
            if (getter != null)
            {
                return getter.Invoke(target, Array.Empty<object>());
            }
            else if (useBackingFieldIfNoGetter)
            {
                var backingField = property.GetBackingField(target.GetType());

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
        /// <exception cref="ArgumentException"><paramref name="property"/> is a non-auto property with no setter.</exception>
        public static void SetValue(this PropertyInfo property, object target, object value, bool useBackingFieldIfNoSetter)
        {
            var setter = property.GetSetMethod(true);
            if (setter != null)
            {
                setter.Invoke(target, new object[] { value });
            }
            else if (useBackingFieldIfNoSetter)
            {
                var backingField = property.GetBackingField(target.GetType());

                if (backingField != null)
                    backingField.SetValue(target, value);
                else
                    throw new ArgumentException($"Could not locate setter or backing field for member {property.DeclaringType}.{property}!");
            }
            else
            {
                throw new ArgumentException($"{property.DeclaringType}.{property} has no setter!");
            }
        }

        /// <summary>
        /// Gets the backing field for an auto-property.
        /// </summary>
        /// <param name="property">The property to get a backing field for.</param>
        /// <param name="type">The <see cref="Type"/> implementing the property.</param>
        /// <returns>The property's backing field, or null if no backing field found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> or <paramref name="type"/> is null.</exception>
        public static FieldInfo GetBackingField(this PropertyInfo property, Type type)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

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
                    var backingFieldName = $"<{property.DeclaringType.FullName.Replace('+', '.')}.{property.Name}>k__BackingField";
                    return type.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
            else
            {
                var backingFieldName = $"<{property.Name}>k__BackingField";
                return type.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }
    }
}
