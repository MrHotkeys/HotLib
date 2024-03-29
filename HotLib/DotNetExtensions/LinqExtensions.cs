﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/>.
    /// </summary>
    public static class LinqExtensions
    {
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
        /// Returns the only item in the sequence, and throws an exception if no items or multiple
        /// items are found, as provided by the given exception factory delegates.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="Single{T}(IEnumerable{T}, Predicate{T}, Func{IEnumerable{T}, Exception?}?, Func{IEnumerable{T}, int, Exception?}?)"/>
        [return: NotNullIfNotNull("none")]
        public static T? Single<T>(
                this IEnumerable<T> enumerable,
                Func<IEnumerable<T>, Exception>? none = null,
                Func<IEnumerable<T>, int, Exception>? multiple = null) =>
            Single(enumerable, t => true, none, multiple);

        /// <summary>
        /// Returns the only item in the sequence that satisfies a condition, and throws an exception if no
        /// items or multiple items are found, as provided by the given exception factory delegates.
        /// </summary>
        /// <typeparam name="T">The type of item in the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable to get a single item from.</param>
        /// <param name="predicate">The condition the single item in the list must meet.</param>
        /// <param name="none">The exception factory that will be called when no items are found. Will
        ///     be given the enumerable as an argument, to help avoid closures. Can be null, in
        ///     which case no exception will be thrown when there are no items.</param>
        /// <param name="multiple">The exception factory that will be called when multiple items are found.
        ///     Will be given the enumerable and total count of items as an argument, to help avoid closures. Can be
        ///     null, in which case no exception will be thrown when there are multiple items.</param>
        /// <returns>The single item from the sequence, the default value for <typeparamref name="T"/> if no
        ///     items and <paramref name="none"/> is null, or the first item from the sequence if multiple
        ///     items and <paramref name="multiple"/> is null.</returns>
        /// <exception cref="InvalidOperationException">Condition for throwing an exception met but one of the exception
        ///     factory delegates returned null instead of a throwable exception instance.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="predicate"/> is null.</exception>
        [return: NotNullIfNotNull("none")]
        public static T? Single<T>(
                this IEnumerable<T> enumerable,
                Predicate<T> predicate,
                Func<IEnumerable<T>, Exception>? none = null,
                Func<IEnumerable<T>, int, Exception>? multiple = null)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (!enumerable.TryGetSingle(predicate, out var result, out var count))
            {
                if (count == 0)
                {
                    if (none is not null)
                    {
                        throw none(enumerable) ??
                            throw new InvalidOperationException($"No items found in {enumerable}, but exception factory delegate {nameof(none)} returned null!");
                    }
                }
                else // Multiple
                {
                    if (multiple is not null)
                    {
                        throw multiple(enumerable, count) ??
                            throw new InvalidOperationException($"Multiple items found in {enumerable}, but exception factory delegate {nameof(multiple)} returned null!");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the only item in the sequence, the default for <typeparamref name="T"/> if no items,
        /// or throws an exception if multiple items are found, as provided by the given exception factory delegate.
        /// </summary>
        /// <inheritdoc cref="Single"/>
        public static T? SingleOrDefault<T>(
                this IEnumerable<T> enumerable,
                Func<IEnumerable<T>, int, Exception?>? multiple = null) =>
            SingleOrDefault(enumerable, t => true, multiple);

        /// <summary>
        /// Returns the only item in the sequence that satisfies a condition, the default for <typeparamref name="T"/> if no items,
        /// or throws an exception if multiple items are found, as provided by the given exception factory delegate.
        /// </summary>
        /// <returns>The single item from the sequence, or the default value for <typeparamref name="T"/> if there
        ///     are no items in the sequence that satisfy the condition or <paramref name="multiple"/> is null and the
        ///     corresponding condition occurs.</returns>
        /// <inheritdoc cref="Single{T}(IEnumerable{T}, Predicate{T}, Func{IEnumerable{T}, Exception?}?, Func{IEnumerable{T}, int, Exception?}?)"/>
        public static T? SingleOrDefault<T>(
                this IEnumerable<T> enumerable,
                Predicate<T> predicate,
                Func<IEnumerable<T>, int, Exception?>? multiple = null) =>
            Single(enumerable, predicate, null, multiple);

        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="TryGetSingle{T}(IEnumerable{T}, out T, out int, bool)"/>
        public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T value) =>
            TryGetSingle(enumerable, out value, out _, false);

        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="TryGetSingle{T}(IEnumerable{T}, out T, out int, bool)"/>
        public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T value, out int count) =>
            TryGetSingle(enumerable, out value, out count, true);

        /// <summary>
        /// Attempts to get a single item from the the sequence.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="TryGetSingle{T}(IEnumerable{T}, Predicate{T}, out T, out int, bool)"/>
        private static bool TryGetSingle<T>(IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T value, out int count, bool countAll) =>
            TryGetSingle(enumerable, t => true, out value, out count, countAll);

        /// <inheritdoc cref="TryGetSingle{T}(IEnumerable{T}, Predicate{T}, out T, out int, bool)"/>
        public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(false)] out T value) =>
            TryGetSingle(enumerable, predicate, out value, out _, false);

        /// <inheritdoc cref="TryGetSingle{T}(IEnumerable{T}, Predicate{T}, out T, out int, bool)"/>
        public static bool TryGetSingle<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(false)] out T value, out int count) =>
            TryGetSingle(enumerable, predicate, out value, out count, true);

        /// <summary>
        /// Attempts to get a single item from the the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="enumerable">The sequence of items to get the single item from.</param>
        /// <param name="predicate">The predicate to filter items against.</param>
        /// <param name="value">
        ///     Will be set based on the number of items in the sequence:
        ///     <list type="bullet">
        ///         <item>
        ///             <term>1 Item</term><description>The single item in the sequence.</description>
        ///         </item>
        ///         <item>
        ///             <term>0 Items</term><description>The default value for <typeparamref name="T"/>.</description>
        ///         </item>
        ///         <item>
        ///             <term>2+ Items</term><description>The first item in the sequence.</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="count">Will be set to the number of matches found.</param>
        /// <param name="countAll">Whether or not to count all items when multiple found, or immediately return.
        ///     Intended for overloads which don't produce the count.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="predicate"/> is null.</exception>
        private static bool TryGetSingle<T>(IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(false)] out T value, out int count, bool countAll)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            using var enumerator = enumerable.GetEnumerator();

            // Check if no values
            if (!enumerator.MoveNextUntil(predicate))
            {
                count = 0;
                value = default;
                return false;
            }

            value = enumerator.Current;
            count = 1;

            // Check if multiple values
            if (enumerator.MoveNextUntil(predicate))
            {
                if (countAll)
                {
                    // Count exactly how many values
                    do
                    {
                        count++;
                    }
                    while (enumerator.MoveNextUntil(predicate));
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the first item in the sequence, and throws an exception if no items are found,
        /// as provided by the given exception factory delegate.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="First{T}(IEnumerable{T}, Predicate{T}, Func{IEnumerable{T}, Exception}?)"/>
        [return: NotNullIfNotNull("none")]
        public static T? First<T>(
                this IEnumerable<T> enumerable,
                Func<IEnumerable<T>, Exception>? none = null) =>
            First(enumerable, t => true, none);

        /// <summary>
        /// Returns the first item in the sequence that satisfies a condition, and throws an exception if no
        /// items are found, as provided by the given exception factory delegates.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="enumerable">The sequence of items to get the first item from.</param>
        /// <param name="predicate">The condition the item in the list must meet.</param>
        /// <param name="none">The exception factory that will be called when no items are found. Will
        ///     be given the enumerable as an argument, to help avoid closures. Can be null, in
        ///     which case no exception will be thrown when there are no items.</param>
        /// <returns>The first item from the sequence, or the default value for <typeparamref name="T"/> if no
        ///     items and <paramref name="none"/> is null.</returns>
        /// <exception cref="InvalidOperationException">Condition for throwing an exception met but one of the exception
        ///     factory delegates returned null instead of a throwable exception instance.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="predicate"/> is null.</exception>
        [return: NotNullIfNotNull("none")]
        public static T? First<T>(
                this IEnumerable<T> enumerable,
                Predicate<T> predicate,
                Func<IEnumerable<T>, Exception>? none = null)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (!enumerable.TryGetFirst(predicate, out var result) && none is not null)
            {
                throw none(enumerable) ??
                    throw new InvalidOperationException($"No items found in {enumerable}, but exception factory delegate {nameof(none)} returned null!");
            }

            return result;
        }

        /// <summary>
        /// Attempts to get the first item from the sequence.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null.</exception>
        /// <inheritdoc cref="TryGetFirst{T}(IEnumerable{T}, Predicate{T}, out T)"/>
        public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T value) =>
            TryGetFirst(enumerable, out value);

        /// <summary>
        /// Attempts to get the first item from the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="enumerable">The sequence of items to get the first item from.</param>
        /// <param name="predicate">The predicate to filter items against.</param>
        /// <param name="value">
        ///     Will be set based on the number of items in the sequence:
        ///     <list type="bullet">
        ///         <item>
        ///             <term>1+ Items</term><description>The first item in the sequence.</description>
        ///         </item>
        ///         <item>
        ///             <term>0 Items</term><description>The default value for <typeparamref name="T"/>.</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> or <paramref name="predicate"/> is null.</exception>
        public static bool TryGetFirst<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, [MaybeNullWhen(false)] out T value) =>
            TryGetSingle(enumerable, predicate, out value, out var count, false) || count > 0;

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
        /// Selects a tuple of every item in the list paired with its respective index in the list.
        /// </summary>
        /// <remarks>The list is processed in order from beginning (index 0) to end.</remarks>
        /// <typeparam name="T">The type of item in the list.</typeparam>
        /// <param name="list">The list of items to enumerate.</param>
        /// <returns>The enumerable of projected values as results of the projection function, in the order the original items appeared in the list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public static IEnumerable<(int Index, T Item)> SelectWithIndex<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            for (var i = 0; i < list.Count; i++)
                yield return (i, list[i]);
        }

        /// <summary>
        /// Selects a tuple of every item in the list paired with its respective index in the list, and applies a projection function to each tuple.
        /// </summary>
        /// <remarks>The list is processed in order from beginning (index 0) to end.</remarks>
        /// <typeparam name="T">The type of item in the list.</typeparam>
        /// <param name="list">The list of items to enumerate.</param>
        /// <param name="projection">The projection function to apply to the list.</param>
        /// <returns>The enumerable of projected values as results of the projection function, in the order the original items appeared in the list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> or <paramref name="projection"/> is null.</exception>
        public static IEnumerable<TProjected> SelectWithIndex<T, TProjected>(this IList<T> list, Func<(int Index, T Item), TProjected> projection)
        {
            if (projection == null)
                throw new ArgumentNullException(nameof(projection));

            foreach (var tuple in SelectWithIndex(list))
                yield return projection(tuple);
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
                var itemTuples = items.Select(i => (Index: indexSelector(i), Item: i))
                                      .OrderBy(tuple => tuple.Index);

                if (itemTuples.HasAtLeast(2))
                    ensureUniqueIndices(itemTuples);

                var highestIndex = itemTuples.Max(tuple => tuple.Index);
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

            void fillArray(IEnumerable<(int Index, T Value)> orderedItemTuples, T[] array)
            {
                var current = orderedItemTuples.First();
                var tuplesLeft = orderedItemTuples.Skip(1);
                for (var index = 0; index < array.Length; index++)
                {
                    if (index == array.Length - 1)
                    {
                        array[index] = current.Value;
                    }
                    else if (current.Index == index)
                    {
                        array[index] = current.Value;
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

        /// <summary>
        /// Calls <see cref="IEnumerator.MoveNext"/> on the given enumerator continuously until a value
        /// is found that matches the given predicate, or there are no items remaining.
        /// </summary>
        /// <typeparam name="T">The type of item being enumerated.</typeparam>
        /// <param name="enumerator">The enumerator to advance.</param>
        /// <param name="predicate">The predicate filter items against.</param>
        /// <returns><see langword="true"/> if an item matching the predicate is found, <see langword="false"/> if the enumerator hits the end.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> or <paramref name="predicate"/> is null.</exception>
        public static bool MoveNextUntil<T>(this IEnumerator<T> enumerator, Predicate<T> predicate)
        {
            if (enumerator is null)
                throw new ArgumentNullException(nameof(enumerator));
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    return true;
            }

            // No values left
            return false;
        }

        /// <inheritdoc cref="DumpToSpan{T}(IEnumerator{T})"/>
        public static Span<object> DumpToSpan(this IEnumerator enumerator) =>
            DumpToSpan(enumerator, false);

        /// <inheritdoc cref="DumpToSpan{T}(IEnumerator{T}, bool)"/>
        public static Span<object> DumpToSpan(this IEnumerator enumerator, bool includeCurrent)
        {
            if (enumerator is null)
                throw new ArgumentNullException(nameof(enumerator));

            var list = new List<object>();
            var hasItems = includeCurrent || enumerator.MoveNext();
            while (hasItems)
            {
                list.Add(enumerator.Current);
                hasItems = enumerator.MoveNext();
            }

            return CollectionsMarshal.AsSpan(list);
        }

        /// <remarks><see cref="IEnumerator.MoveNext"/> will be called before dumping items,
        ///     skipping past whatever is currently in <see cref="IEnumerator.Current"/>.</remarks>
        /// <inheritdoc cref="DumpToSpan{T}(IEnumerator{T}, bool)"/>
        public static Span<T> DumpToSpan<T>(this IEnumerator<T> enumerator) =>
            DumpToSpan(enumerator, false);

        /// <summary>
        /// Dumps all items left to be enumerated to a span.
        /// </summary>
        /// <param name="enumerator">The enumerator to dump.</param>
        /// <param name="includeCurrent">If <see langword="true"/>, the current item in the enumerator will be included.
        ///     If <see langword="false"/>, <see cref="IEnumerator.MoveNext"/> will be called before dumping items.</param>
        /// <returns>The span containing all items. Will have the same size as the number of
        ///     items remaining in the enumerator.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is null.</exception>
        public static Span<T> DumpToSpan<T>(this IEnumerator<T> enumerator, bool includeCurrent)
        {
            var list = new List<T>();
            var hasItems = includeCurrent || enumerator.MoveNext();
            while (hasItems)
            {
                list.Add(enumerator.Current);
                hasItems = enumerator.MoveNext();
            }

            return CollectionsMarshal.AsSpan(list);
        }

        /// <inheritdoc cref="DumpToArray{T}(IEnumerator{T})"/>
        public static object[] DumpToArray(this IEnumerator enumerator) =>
            DumpToArray(enumerator, false);

        /// <inheritdoc cref="DumpToArray{T}(IEnumerator{T}, bool)"/>
        public static object[] DumpToArray(this IEnumerator enumerator, bool includeCurrent)
        {
            var span = DumpToSpan(enumerator, includeCurrent);

            var arr = new object[span.Length];
            var arrSpan = arr.AsSpan();

            span.CopyTo(arrSpan);

            return arr;
        }

        /// <remarks><see cref="IEnumerator.MoveNext"/> will be called before dumping items,
        ///     skipping past whatever is currently in <see cref="IEnumerator.Current"/>.</remarks>
        /// <inheritdoc cref="DumpToArray{T}(IEnumerator{T}, bool)"/>
        public static T[] DumpToArray<T>(this IEnumerator<T> enumerator) =>
            DumpToArray(enumerator, false);

        /// <summary>
        /// Dumps all items left to be enumerated to an array.
        /// </summary>
        /// <param name="enumerator">The enumerator to dump.</param>
        /// <param name="includeCurrent">If <see langword="true"/>, the current item in the enumerator will be included.
        ///     If <see langword="false"/>, <see cref="IEnumerator.MoveNext"/> will be called before dumping items.</param>
        /// <returns>The array containing all items. Will have the same size as the number of
        ///     items remaining in the enumerator.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is null.</exception>
        public static T[] DumpToArray<T>(this IEnumerator<T> enumerator, bool includeCurrent)
        {
            var span = DumpToSpan(enumerator, includeCurrent);

            var arr = new T[span.Length];
            var arrSpan = arr.AsSpan();

            span.CopyTo(arrSpan);

            return arr;
        }
    }
}
