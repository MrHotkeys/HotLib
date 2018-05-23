using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HotLib.Bits
{
    /// <summary>
    /// Contains helper methods for swapping the endianness of unmanaged values.
    /// Uses statically generated swapping methods with no loops for best performance.
    /// </summary>
    /// <typeparam name="T">The type of value being swapped.</typeparam>
    public static class EndiannessSwapper<T>
        where T : unmanaged
    {
        /// <summary>
        /// The delegate type for a generated swapping method.
        /// </summary>
        /// <param name="value">A wrapped pointer to the initial value.</param>
        /// <param name="result">A wrapped pointer to the result value.</param>
        private delegate void SwapMethod(WrappedPointer<T> value, WrappedPointer<T> result);

        /// <summary>
        /// Gets the generated swapping method for T.
        /// </summary>
        private readonly static SwapMethod Swapper;

        /// <summary>
        /// Initializes <see cref="EndiannessSwapper{T}"/> by generating the appropriate swapping method.
        /// </summary>
        unsafe static EndiannessSwapper()
        {
            var valueParam = Expression.Parameter(typeof(WrappedPointer<T>), "value");
            var resultParam = Expression.Parameter(typeof(WrappedPointer<T>), "result");

            var indexer = typeof(WrappedPointer<T>).GetProperty("Item");

            IEnumerable<Expression> GetBody(int byteCount)
            {
                for (var offset = 0; offset < byteCount; offset++)
                {
                    var valueOffset = Expression.Constant(offset);
                    var accessValue = Expression.MakeIndex(valueParam, indexer, Enumerable.Repeat(valueOffset, 1));

                    var resultOffset = Expression.Constant(byteCount - offset - 1);
                    var accessResult = Expression.MakeIndex(resultParam, indexer, Enumerable.Repeat(resultOffset, 1));

                    yield return Expression.Assign(accessResult, accessValue);
                }
            }

            var body = Expression.Block(GetBody(sizeof(T)));

            Swapper = Expression.Lambda<SwapMethod>(body, valueParam, resultParam)
                                .Compile();
        }

        /// <summary>
        /// Swaps the endianness of the given value.
        /// </summary>
        /// <param name="value">The value to swap.</param>
        /// <returns>The swapped value.</returns>
        public unsafe static T Swap(T value)
        {
            var result = stackalloc T[1];

            Swapper.Invoke(new WrappedPointer<T>(&value),
                           new WrappedPointer<T>(result));

            return *result;
        }

        /// <summary>
        /// Swaps the endianness of the given value.
        /// </summary>
        /// <param name="value">The value to swap.</param>
        public unsafe static void Swap(ref T value)
        {
            var result = stackalloc T[1];

            fixed (T* ptr = &value)
            {
                Swapper.Invoke(new WrappedPointer<T>(ptr),
                               new WrappedPointer<T>(result));
            }

            value = *result;
        }
    }
}
