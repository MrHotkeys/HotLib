using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HotLib.Equality
{
    /// <summary>
    /// An <see cref="IUniversalEqualityComparer"/> implementation that uses <see cref="EqualityComparer{T}.Default"/>
    /// for checking for equality and hashing. Immutable.
    /// </summary>
    public sealed class DefaultUniversalEqualityComparer : IUniversalEqualityComparer
    {
        /// <summary>
        /// Gets a static singleton instance of <see cref="DefaultUniversalEqualityComparer"/>.
        /// </summary>
        public static DefaultUniversalEqualityComparer Instance { get; } = new DefaultUniversalEqualityComparer();

        /// <summary>
        /// Instantiates a new <see cref="DefaultUniversalEqualityComparer"/>.
        /// </summary>
        private DefaultUniversalEqualityComparer()
        { }

        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <typeparam name="T">The type of the objects being compared.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the objects are equal, <see langword="false"/> if not.</returns>
        public bool Equals<T>(T? x, T? y) => EqualityComparer<T>.Default.Equals(x, y);

        /// <summary>
        /// Gets a hash code for an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to get a hash code for.</typeparam>
        /// <param name="obj">The object to get a hash code for.</param>
        /// <returns>The object's hash code.</returns>
        public int GetHashCode<T>([DisallowNull] T obj) => EqualityComparer<T>.Default.GetHashCode(obj);
    }
}
