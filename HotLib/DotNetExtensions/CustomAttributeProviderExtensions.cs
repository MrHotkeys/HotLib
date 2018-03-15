using System;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="ICustomAttributeProvider"/>.
    /// </summary>
    public static class CustomAttributeProviderExtensions
    {
        /// <summary>
        /// Checks whether or not the <see cref="ICustomAttributeProvider"/> has at least one attribute of the given type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <returns>True if the <see cref="ICustomAttributeProvider"/> has the attribute, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.HasCustomAttribute<T>(true);
        }

        /// <summary>
        /// Checks whether or not the <see cref="ICustomAttributeProvider"/> has at least one attribute of the given type.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>True if the <see cref="ICustomAttributeProvider"/> has the attribute, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit)
        {
            if (customAttributeProvider == null)
                throw new ArgumentNullException(nameof(customAttributeProvider));
            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit)
                                          .Any();
        }
        /// <summary>
        /// Checks whether or not the <see cref="ICustomAttributeProvider"/> has at least one attribute of the given type.
        /// </summary>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="attributeType">The type of attribute to check for.</param>
        /// <returns>True if the <see cref="ICustomAttributeProvider"/> has the attribute, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> or
        ///     <paramref name="attributeType"/> is null.</exception>
        public static bool HasCustomAttribute(this ICustomAttributeProvider customAttributeProvider, Type attributeType)
        {
            return customAttributeProvider.HasCustomAttribute(attributeType, true);
        }

        /// <summary>
        /// Checks whether or not the <see cref="ICustomAttributeProvider"/> has at least one attribute of the given type.
        /// </summary>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="attributeType">The type of attribute to check for.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>True if the <see cref="ICustomAttributeProvider"/> has the attribute, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> or
        ///     <paramref name="attributeType"/> is null.</exception>
        public static bool HasCustomAttribute(this ICustomAttributeProvider customAttributeProvider,
                                              Type attributeType, bool inherit)
        {
            if (customAttributeProvider == null)
                throw new ArgumentNullException(nameof(customAttributeProvider));
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            return customAttributeProvider.GetCustomAttributes(attributeType, inherit)
                                          .Any();
        }

        /// <summary>
        /// Tries to get the single custom attribute from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="attributeType">The type of attribute to check for.</param>
        /// <param name="attribute">The found attribute.</param>
        /// <returns>True if there is a single matching attribute, false if there are zero matches or more than one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> or
        ///     <paramref name="attributeType"/> is null.</exception>
        public static bool TryGetCustomAttribute(this ICustomAttributeProvider customAttributeProvider,
                                                 Type attributeType, out Attribute attribute)
        {
            return customAttributeProvider.TryGetCustomAttribute(attributeType, true, out attribute);
        }

        /// <summary>
        /// Tries to get the single custom attribute from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="attributeType">The type of attribute to check for.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <param name="attribute">The found attribute.</param>
        /// <returns>True if there is a single matching attribute, false if there are zero matches or more than one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> or
        ///     <paramref name="attributeType"/> is null.</exception>
        public static bool TryGetCustomAttribute(this ICustomAttributeProvider customAttributeProvider,
                                                 Type attributeType, bool inherit, out Attribute attribute)
        {
            if (customAttributeProvider == null)
                throw new ArgumentNullException(nameof(customAttributeProvider));
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            var matches = customAttributeProvider.GetCustomAttributes(attributeType, inherit);
            attribute = (Attribute)matches.FirstOrDefault();
            return matches.HasSingle();
        }

        /// <summary>
        /// Tries to get the single custom attribute from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="attribute">The found attribute.</param>
        /// <returns>True if there is a single matching attribute, false if there are zero matches or more than one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static bool TryGetCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider, out T attribute)
        {
            return customAttributeProvider.TryGetCustomAttribute<T>(true, out attribute);
        }

        /// <summary>
        /// Tries to get the single custom attribute from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The <see cref="ICustomAttributeProvider"/> to check.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <param name="attribute">The found attribute.</param>
        /// <returns>True if there is a single matching attribute, false if there are zero matches or more than one.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static bool TryGetCustomAttribute<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit, out T attribute)
        {
            if (customAttributeProvider == null)
                throw new ArgumentNullException(nameof(customAttributeProvider));

            var matches = customAttributeProvider.GetCustomAttributes(typeof(T), inherit)
                                                 .Cast<T>();
            attribute = matches.FirstOrDefault();
            return matches.HasSingle();
        }

        /// <summary>
        /// Gets all custom attributes of the given type from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The provider to get attributes from.</param>
        /// <returns>An array of all found attributes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider customAttributeProvider)
        {
            return customAttributeProvider.GetCustomAttributes<T>(true);
        }

        /// <summary>
        /// Gets all custom attributes of the given type from the <see cref="ICustomAttributeProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of attribute to check for.</typeparam>
        /// <param name="customAttributeProvider">The provider to get attributes from.</param>
        /// <param name="inherit">When true, look up the hierarchy chain for the inherited custom attribute.</param>
        /// <returns>An array of all found attributes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="customAttributeProvider"/> is null.</exception>
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider customAttributeProvider, bool inherit)
        {
            if (customAttributeProvider == null)
                throw new ArgumentNullException(nameof(customAttributeProvider));

            return customAttributeProvider.GetCustomAttributes(typeof(T), inherit)
                                          .Cast<T>()
                                          .ToArray();
        }
    }
}
