using System;
using System.Collections.Generic;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Inserts an item into the list using insertion sorting.
        /// </summary>
        /// <typeparam name="T">The type of item being inserted into the list.</typeparam>
        /// <param name="list">The list to insert into.</param>
        /// <param name="item">The item being inserted into the list.</param>
        /// <param name="comparer">The comparer to use when determining where the item should be inserted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> or <paramref name="comparer"/> is null.</exception>
        public static void SortedInsert<T>(this IList<T> list, T item, IComparer<T> comparer)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));

            for (var i = 0; i < list.Count; i++)
            {
                if (comparer.Compare(item, list[i]) <= 0)
                {
                    list.Insert(i, item);
                    return;
                }
            }

            // If we got this far, it just goes at the end
            list.Add(item);
        }

        /// <summary>
        /// Inserts an item into the list using insertion sorting.
        /// </summary>
        /// <typeparam name="T">The type of item being inserted into the list.</typeparam>
        /// <param name="list">The list to insert into.</param>
        /// <param name="item">The item being inserted into the list.</param>
        /// <param name="comparerMethod">The comparer method to use when determining
        ///     where the item should be inserted.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>
        ///     or <paramref name="comparerMethod"/> is null.</exception>
        public static void SortedInsert<T>(this IList<T> list, T item, Func<T, T, int> comparerMethod)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            if (comparerMethod is null)
                throw new ArgumentNullException(nameof(comparerMethod));

            for (var i = 0; i < list.Count; i++)
            {
                if (comparerMethod.Invoke(item, list[i]) <= 0)
                    list.Insert(i, item);
            }

            // If we got this far, it just goes at the end
            list.Add(item);
        }

        /// <remarks>Uses <see cref="EqualityComparer{T}.Default"/> to compare keys.</remarks>
        /// <inheritdoc cref="ToDictionaryOfIndices{T}(IList{T}, IEqualityComparer{T})"/>
        public static Dictionary<T, int> ToDictionaryOfIndices<T>(this IList<T> list)
            where T : notnull =>            
            ToDictionaryOfIndices(list, EqualityComparer<T>.Default);

        /// <summary>
        /// Creates a dictionary where the keys are each item from the list, and the values are the respective indices in this list.
        /// </summary>
        /// <typeparam name="T">The type of item in the list.</typeparam>
        /// <param name="list">The list to convert. Cannot contain null.</param>
        /// <param name="keyComparer">The comparer to use for keys.</param>
        /// <returns>The created dictionary.</returns>
        /// <exception cref="ArgumentException"><paramref name="list"/> contains null.
        ///     -or-The items in <paramref name="list"/> are not unique.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> or <paramref name="keyComparer"/> is null.</exception>
        public static Dictionary<T, int> ToDictionaryOfIndices<T>(this IList<T> list, IEqualityComparer<T> keyComparer)
            where T : notnull
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            if (keyComparer is null)
                throw new ArgumentNullException(nameof(keyComparer));

            var dictionary = new Dictionary<T, int>(list.Count, keyComparer);
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is null)
                    throw new ArgumentException($"Input list cannot contain null!", nameof(list));
                if (!dictionary.TryAdd(item, i))
                    throw new ArgumentException($"Found repeated item {item} - all items must be unique!", nameof(list));
            }

            return dictionary;
        }
    }
}
