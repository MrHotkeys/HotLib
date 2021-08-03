﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Creates an enumerable of the given item and then performs <see cref="Enumerable.Union{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
        /// and returns the result.
        /// </summary>
        /// <typeparam name="TSource">The type of item in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to union the item into.</param>
        /// <param name="item">The item to union with the enumerable.</param>
        /// <returns>The result of the union.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> enumerable, TSource item)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            var itemEnumerable = Enumerable.Repeat(item, 1);
            return enumerable.Union(itemEnumerable);
        }

        /// <summary>
        /// Performs an aggregator function which combines all elements in the enumerable to return a single value.
        /// </summary>
        /// <typeparam name="T">The type of value in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to aggregate.</param>
        /// <param name="aggregator">The aggregator function to use to combine the enumerable.</param>
        /// <returns>The aggregated value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="aggregator"/> is null.</exception>
        public static T Aggregate<T>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, T> aggregator)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (aggregator == null)
                throw new ArgumentNullException(nameof(aggregator));
            return aggregator.Invoke(enumerable);
        }

        /// <summary>
        /// Gets whether the enumerable has a single element or not.
        /// </summary>
        /// <typeparam name="T">The type of the values in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <returns>True if there is a single element, false if there are 0 or 2+ elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static bool HasSingle<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            return enumerator.MoveNext() && !enumerator.MoveNext();
        }

        /// <summary>
        /// Tries to get the single element from the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the values in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <param name="single">Will be set to the single item from the enumerable, or the default
        ///     value for <typeparamref name="T"/> if no items or too many items.</param>
        /// <returns>True if there is a single element, false if there are 0 or 2+ elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, out T? single)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var first = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    single = first;
                    enumerator.Dispose();
                    return true;
                }
            }

            single = default;
            return false;
        }

        /// <summary>
        /// Tries to get the single element from the enumerable.
        /// If there are no elements, the default value for <typeparamref name="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">The type of the values in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <param name="single">Will be set to the single item from the enumerable, or the default
        ///     value for <typeparamref name="T"/> if no items or too many items.</param>
        /// <returns>True if there is a single element, false if there are 0 or 2+ elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static bool TryGetSingleOrDefault<T>(this IEnumerable<T> enumerable, out T? single)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var first = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    single = first;
                    enumerator.Dispose();
                    return true;
                }
                else
                {
                    single = default;
                    return false;
                }
            }
            else
            {
                single = default;
                return true;
            }

        }

        /// <summary>
        /// Tries to get the first element from the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the values in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to check.</param>
        /// <param name="first">Will be set to the first item from the enumerable, or the default
        ///     value for <typeparamref name="T"/> if no items.</param>
        /// <returns>True if there is at least one element, false if there are 0 elements.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, out T? first)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                first = enumerator.Current;
                enumerator.Dispose();
                return true;
            }
            else
            {
                first = default;
                return false;
            }
        }

        /// <summary>
        /// Filters out the first item in the enumerable that matches the predicate, and yield returns the rest, in order.
        /// If no match, simply all the items in the given enumerable will be yield returned.
        /// </summary>
        /// <param name="enumerable">The enumerable through.</param>
        /// <param name="filter">The predicate used to find the item to filter out.</param>
        /// <returns>The filtered enum.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="filter"/> is null.</exception>
        public static IEnumerable<T> FilterFirst<T>(this IEnumerable<T> enumerable, Func<T, bool> filter)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var skipped = false;
            foreach (var item in enumerable)
            {
                if (filter.Invoke(item))
                {
                    if (skipped)
                        yield return item;
                    else
                        skipped = true;
                }
                else
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets an enumerable of <see cref="ValueTuple"/> of each item in the enumerable and its place, as it was enumerated.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="items">The enumerable of items to get with their place.</param>
        /// <returns>An enumerable of <see cref="ValueTuple"/> of items and their places.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public static IEnumerable<(int place, T item)> WithPlace<T>(this IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var place = 0;
            foreach (var item in items)
                yield return (place++, item);
        }

        /// <summary>
        /// Gets an enumerable of <see cref="ValueTuple"/> of each item in the list and its index.
        /// </summary>
        /// <typeparam name="T">The type of item in the list.</typeparam>
        /// <param name="list">The list of items to get with their indices.</param>
        /// <returns>An enumerable of <see cref="ValueTuple"/> of items and their indices.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public static IEnumerable<(int index, T item)> WithIndex<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (var i = 0; i < list.Count; i++)
                yield return (i, list[i]);
        }

        /// <summary>
        /// Gets an integer enumerable of all the indices in the list.
        /// </summary>
        /// <typeparam name="T">The type of item in the list.</typeparam>
        /// <param name="list">The list of items to get indices for.</param>
        /// <returns>An enumerable of all the integer indices.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public static IEnumerable<int> SelectIndex<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return Enumerable.Range(0, list.Count);
        }

        /// <summary>
        /// Gets an enumerable of <see cref="ValueTuple"/> of each item in the enumerable and an index as
        /// provided by the given index selector. Does not enforce distinctness among the indices.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="items">The enumerable of items to select with indices.</param>
        /// <param name="indexSelector">Retrieves the appropriate index for the given item.</param>
        /// <returns>An enumerable of <see cref="ValueTuple"/> of all items from the enumerable and their indices.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> or <paramref name="indexSelector"/> is null.</exception>
        public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> items, Func<T, int> indexSelector)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (indexSelector == null)
                throw new ArgumentNullException(nameof(indexSelector));

            foreach (var value in items)
                yield return (indexSelector(value), value);
        }

        /// <summary>
        /// Tests if the enumerable has at least a certain number of items. Only counts
        /// until it is certain that the enumerable does or does not meet the check.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="items">The enumerable to test.</param>
        /// <param name="count">The minimum number of items to check for.</param>
        /// <returns>True if the enumerable has at least the given number of items, false if it contains fewer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public static bool HasAtLeast<T>(this IEnumerable<T> items, int count)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var itemsLeft = items;
            for (var i = 0; i < count; i++)
            {
                if (itemsLeft.Any())
                    itemsLeft = itemsLeft.Skip(1);
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests if the enumerable has no more than a certain number of items. Only counts
        /// until it is certain that the enumerable does or does not meet the check.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="items">The enumerable to test.</param>
        /// <param name="count">The maximum number of items to check for.</param>
        /// <returns>True if the enumerable has no more than the given number of items, false if it contains more.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> is null.</exception>
        public static bool HasNoMoreThan<T>(this IEnumerable<T> items, int count) => !items.HasAtLeast(count + 1);

        /// <summary>
        /// Converts the enumerable to an array, using the given index selector to determine the index at
        /// which each item in the enumerable goes. Any indices not filled with an item will be left at
        /// the default value for the enumerable's item type.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="items">The enumerable of items to create an array from.</param>
        /// <param name="indexSelector">Retrieves the appropriate index for the given item.</param>
        /// <returns>An array of items from the enumerable.</returns>
        /// <exception cref="ArgumentException">The given index selector returned the same index for two or more items.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="items"/> or <paramref name="indexSelector"/> is null.</exception>
        public static T[] ToArray<T>(this IEnumerable<T> items, Func<T, int> indexSelector)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (indexSelector == null)
                throw new ArgumentNullException(nameof(indexSelector));

            if (items.Any())
            {
                var itemTuples = items.WithIndex(indexSelector)
                                      .OrderBy(tuple => tuple.index);

                if (itemTuples.HasAtLeast(2))
                    ensureUniqueIndices(itemTuples);

                var highestIndex = itemTuples.Max(tuple => tuple.index);
                var array = new T[highestIndex + 1];

                fillArray(itemTuples, array);

                return array;
            }
            else
            {
                return Array.Empty<T>();
            }

            void ensureUniqueIndices(IEnumerable<(int index, T value)> orderedItemTuples)
            {
                var last = orderedItemTuples.First();
                var tuplesLeft = orderedItemTuples.Skip(1);
                while (tuplesLeft.Any())
                {
                    var current = tuplesLeft.First();
                    tuplesLeft = tuplesLeft.Skip(1);

                    if (last.index == current.index)
                        throw new ArgumentException($"Given index selector assigned two or more items ({last}, {current}) " +
                                                    $"with same index {current.index}!", nameof(indexSelector));

                    last = current;
                }
            }

            void fillArray(IEnumerable<(int index, T value)> orderedItemTuples, T[] array)
            {
                var current = orderedItemTuples.First();
                var tuplesLeft = orderedItemTuples.Skip(1);
                for (var index = 0; index < array.Length; index++)
                {
                    if (index == array.Length - 1)
                    {
                        array[index] = current.value;
                    }
                    else if (current.index == index)
                    {
                        array[index] = current.value;
                        current = tuplesLeft.First();
                        tuplesLeft = tuplesLeft.Skip(1);
                    }
                }
            }
        }

        /// <summary>
        /// Aggregates an enumerable of <see cref="string"/> using <see cref="string.Join(string, IEnumerable{string})"/>.
        /// </summary>
        /// <param name="strings">The enumerable of strings to join together.</param>
        /// <param name="seperator">The seperator string to place between each joined string in the result (eg a comma in a list).</param>
        /// <returns>A string of all the joined together strings and seperators.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="strings"/> or <paramref name="seperator"/> is null.</exception>
        public static string AggregateWithJoin(this IEnumerable<string> strings, string seperator)
        {
            if (strings == null)
                throw new ArgumentNullException(nameof(strings));
            if (seperator == null)
                throw new ArgumentNullException(nameof(seperator));

            return string.Join(seperator, strings);
        }

        /// <summary>
        /// Copies all items from the enumerable into a new two-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable of items to copy from.</param>
        /// <param name="length0">The length of the 0th dimension in the 2D array.</param>
        /// <param name="length1">The length of the 1st dimension in the 2D array.</param>
        /// <returns>The created 2D array.</returns>
        /// <exception cref="ArgumentException"><paramref name="length0"/> or <paramref name="length1"/> is non-positive.
        ///     -or-<paramref name="enumerable"/> does not contain enough items for the given <paramref name="length0"/>
        ///     and <paramref name="length1"/> values.
        ///     -or-<paramref name="enumerable"/> contains too many tems for the given <paramref name="length0"/> 
        ///     and <paramref name="length1"/> values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static T[,] ToArray2D<T>(this IEnumerable<T> enumerable, int length0, int length1)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (length0 <= 0)
                throw new ArgumentException("Must be positive!", nameof(length0));
            if (length1 <= 0)
                throw new ArgumentException("Must be positive!", nameof(length1));

            var enumerator = enumerable.GetEnumerator();
            var array = new T[length0, length1];
            for (var i1 = 0; i1 < length1; i1++)
            {
                for (var i0 = 0; i0 < length0; i0++)
                {
                    // Make sure we have an item for this spot
                    if (!enumerator.MoveNext())
                    {
                        throw new ArgumentException($"Enumerable only has {i1 * length0 + i0} items (requires " +
                                                    $"{length0 * length1})!", nameof(enumerable));
                    }

                    array[i0, i1] = enumerator.Current;
                }
            }

            // Make sure nothing left over
            if (enumerator.MoveNext())
            {
                var count = enumerable.Count();
                throw new ArgumentException($"Enumerable is too large for the given 2D array " +
                                            $"dimensions (has {count} items, only space for " +
                                            $"{length0 * length1})!", nameof(enumerable));
            }

            enumerator.Dispose();

            return array;
        }

        /// <summary>
        /// Copies all items from the enumerable into a new two-dimensional array in the heap.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable. Must be an unmanaged type.</typeparam>
        /// <param name="enumerable">The enumerable of items to copy from.</param>
        /// <param name="length0">The length of the 0th dimension in the 2D array.</param>
        /// <param name="length1">The length of the 1st dimension in the 2D array.</param>
        /// <returns>An <see cref="IntPtr"/> to the created 2D array.</returns>
        /// <exception cref="ArgumentException"><paramref name="length0"/> or <paramref name="length1"/> is non-positive.
        ///     -or-<paramref name="enumerable"/> does not contain enough items for the given <paramref name="length0"/>
        ///     and <paramref name="length1"/> values.
        ///     -or-<paramref name="enumerable"/> contains too many tems for the given <paramref name="length0"/> 
        ///     and <paramref name="length1"/> values.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        public static unsafe IntPtr ToArray2DPointer<T>(this IEnumerable<T> enumerable, int length0, int length1)
            where T : unmanaged
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            if (length0 <= 0)
                throw new ArgumentException("Must be positive!", nameof(length0));
            if (length1 <= 0)
                throw new ArgumentException("Must be positive!", nameof(length1));

            var itemsTotal = length0 * length1;
            var pointer = Marshal.AllocHGlobal(itemsTotal * sizeof(T));
            var array = (T*)pointer.ToPointer();

            var enumerator = enumerable.GetEnumerator();
            for (var i1 = 0; i1 < length1; i1++)
            {
                for (var i0 = 0; i0 < length0; i0++)
                {
                    // Make sure we have an item for this spot
                    if (!enumerator.MoveNext())
                    {
                        throw new ArgumentException($"Enumerable only has {i1 * length0 + i0} items (requires " +
                                                    $"{itemsTotal})!", nameof(enumerable));
                    }

                    var index = i1 * length0 + i0;
                    array[index] = enumerator.Current;
                }
            }

            // Make sure nothing left over
            if (enumerator.MoveNext())
            {
                var itemsGiven = enumerable.Count();
                throw new ArgumentException($"Enumerable is too large for the given 2D array " +
                                            $"dimensions (has {itemsGiven} items, only space for " +
                                            $"{itemsTotal})!", nameof(enumerable));
            }

            enumerator.Dispose();

            return pointer;
        }
    }
}
