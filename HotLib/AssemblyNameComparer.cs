using System;
using System.Collections.Generic;
using System.Reflection;

namespace HotLib
{
    /// <summary>
    /// Compares two <see cref="AssemblyName"/> instances for equality by comparing
    /// the values of their <see cref="AssemblyName.FullName"/> properties.
    /// Stateless and thread-safe.
    /// </summary>
    public class AssemblyNameComparer : IEqualityComparer<AssemblyName>
    {
        /// <summary>
        /// Gets a singleton instance of <see cref="AssemblyNameComparer"/>.
        /// </summary>
        public static AssemblyNameComparer Instance { get; } = new AssemblyNameComparer();

        /// <summary>
        /// Instantiates a new <see cref="AssemblyNameComparer"/>.
        /// </summary>
        protected AssemblyNameComparer()
        { }

        /// <summary>
        /// Compares two <see cref="AssemblyName"/> instances for equality by comparing
        /// the values of their <see cref="AssemblyName.FullName"/> properties.
        /// </summary>
        /// <param name="x">The first <see cref="AssemblyName"/> to compare.</param>
        /// <param name="y">The second <see cref="AssemblyName"/> to compare.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool Equals(AssemblyName x, AssemblyName y)
        {
            if (x == null)
                return y == null;
            if (y == null)
                return false;

            return string.Equals(x.FullName, y.FullName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Compares two <see cref="AssemblyName"/> instances for equality by comparing
        /// the values of their <see cref="AssemblyName.FullName"/> properties.
        /// </summary>
        /// <param name="x">The first <see cref="AssemblyName"/> to compare.</param>
        /// <param name="y">The second <see cref="AssemblyName"/> to compare.</param>
        /// <returns>True if equal, false if not.</returns>
        bool IEqualityComparer<AssemblyName>.Equals(AssemblyName x, AssemblyName y) => Equals(x, y);

        /// <summary>
        /// Gets a hash code for an <see cref="AssemblyName"/> by hashing its full name.
        /// </summary>
        /// <param name="an">The assembly name to hash.</param>
        /// <returns>The generated hash code.</returns>
        public int GetHashCode(AssemblyName an) => EqualityComparer<string>.Default.GetHashCode(an.FullName);


    }
}
