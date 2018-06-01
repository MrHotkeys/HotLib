using System;
using System.Collections.Generic;
using System.Linq;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="Array"/>.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fills the array with the given value. If a reference type, only sets every element in the array to the reference.
        /// </summary>
        /// <typeparam name="T">The type of item in the array.</typeparam>
        /// <param name="array">The array to fill.</param>
        /// <param name="value">The value to fill the array with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        public static void Fill<T>(this T[] array, T value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            for (var i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// Fills the array with the given value. If a reference type, only sets every element in the array to the reference.
        /// </summary>
        /// <param name="array">The array to fill.</param>
        /// <param name="value">The value to fill the array with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        public static void Fill(this Array array, object value)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            for (var i = 0; i < array.Length; i++)
                array.SetValue(value, i);
        }

        /// <summary>
        /// Grows an array to a certain size across all its dimensions.
        /// </summary>
        /// <param name="source">The array to grow.</param>
        /// <param name="dimensionSizes">The new dimension sizes. Array will grow to larger sizes, but will ignore smaller ones.</param>
        /// <returns>A new array of the right size with all values from the original array copied over.</returns>
        /// <remarks>Uses <see cref="Array"/> so that it can support multidimensional arrays.</remarks>
        public static Array Resize(this Array source, int[] dimensionSizes)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (dimensionSizes == null)
                throw new ArgumentNullException(nameof(source));
            if (source.Rank != dimensionSizes.Length)
                throw new ArgumentException($"Incorrect number of dimension sizes (expected {source.Rank}, got {dimensionSizes.Length})!");
            if (dimensionSizes.Any(s => s <= 0))
                throw new ArgumentException("A dimension size cannot be non-positive!");
            
            var target = Array.CreateInstance(source.GetType().GetElementType(), dimensionSizes);
            
            var indices = new int[source.Rank];
            CopyRecursivelyAtDimension(0);
            
            return target;

            void CopyRecursivelyAtDimension(int dimension)
            {
                var indexMax = System.Math.Min(source.GetLength(dimension), dimensionSizes[dimension]);
                for (var index = 0; index < indexMax; index++)
                {
                    indices[dimension] = index;

                    if (index < source.GetLength(dimension))
                        target.SetValue(source.GetValue(indices), indices);

                    if (source.Rank - 1 > dimension)
                        CopyRecursivelyAtDimension(dimension + 1);
                }
            }
        }

        /// <summary>
        /// Gets an enumerable for the 2D array that enumerates all values row by row,
        /// from x=0 to x=width and from y=0 to y=height.
        /// </summary>
        /// <typeparam name="T">The type of item in the array.</typeparam>
        /// <param name="arr">The array to get an enumerable for.</param>
        /// <returns>The created enumerable.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="arr"/> is null.</exception>
        public static IEnumerable<T> GetEnumerableByRow<T>(this T[,] arr)
        {
            if (arr is null)
                throw new ArgumentNullException(nameof(arr));

            var width = arr.GetLength(0);
            var height = arr.GetLength(1);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    yield return arr[x, y];
                }
            }
        }
    }
}
