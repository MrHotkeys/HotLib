using System;

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
    }
}
