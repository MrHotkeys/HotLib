using System;
using System.Collections.Generic;

namespace HotLib.Equality
{
    /// <summary>
    /// Wraps an <see cref="IUniversalEqualityComparer"/> so that it can act as an <see cref="IEqualityComparer{T}"/> for a specific type.
    /// </summary>
    /// <typeparam name="T">The type to wrap as an equality comparer for.</typeparam>
    public class UniversalEqualityComparerSpecificationWrapper<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Gets the wrapped <see cref="IUniversalEqualityComparer"/>.
        /// </summary>
        protected IUniversalEqualityComparer Comparer { get; }

        /// <summary>
        /// Instantiates a new <see cref="UniversalEqualityComparerSpecificationWrapper{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IUniversalEqualityComparer"/> to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is null.</exception>
        public UniversalEqualityComparerSpecificationWrapper(IUniversalEqualityComparer comparer)
        {
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// Uses the wrapped <see cref="IUniversalEqualityComparer"/> to compare for equality between two objects.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the objects are equal, <see langword="false"/> if not.</returns>
        public bool Equals(T x, T y) => Comparer.Equals(x, y);

        /// <summary>
        /// Uses the wrapped <see cref="IUniversalEqualityComparer"/> to generate a hash code for an object.
        /// </summary>
        /// <param name="obj">The object to get a hash code for.</param>
        /// <returns>The object's hash code.</returns>
        public int GetHashCode(T obj) => Comparer.GetHashCode(obj);
    }
}
