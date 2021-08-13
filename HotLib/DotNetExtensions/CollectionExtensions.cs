using System;
using System.Collections.Generic;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <typeparam name="T">The type of item in the collection.</typeparam>
        /// <param name="collection">The collection being added to.</param>
        /// <param name="range">An enumerable of items to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> or <paramref name="range"/> is null.</exception>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
            if (range is null)
                throw new ArgumentNullException(nameof(range));

            foreach (var item in range)
                collection.Add(item);
        }
    }
}
