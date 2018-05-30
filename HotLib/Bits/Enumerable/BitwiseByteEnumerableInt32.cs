using System;
using System.Collections.Generic;

namespace HotLib.Bits.Enumerable
{
    /// <summary>
    /// Wraps an enumerable of bytes and enumerates all bits from them in <see cref="Int32"/>
    /// values, a set number of bits at a time. Can handle enumerating values of up to 16 bits.
    /// </summary>
    public class BitwiseByteEnumerableInt32 : BitwiseByteEnumerable<int>
    {
        /// <summary>
        /// The minimum number of bits the container can hold.
        /// </summary>
        public const int MinBits = 1;
        /// <summary>
        /// The maximum number of bits the container can hold.
        /// </summary>
        public const int MaxBits = sizeof(int) * 8;

        /// <summary>
        /// Instantiates a new <see cref="BitwiseByteEnumerableInt32"/>.
        /// </summary>
        /// <param name="bytes">The enumerable of bytes to wrap.</param>
        /// <param name="bitCount">The number of bits to enumerate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is less
        ///     than <see cref="MinBits"/> or more than <see cref="MaxBits"/>.</exception>
        public BitwiseByteEnumerableInt32(IEnumerable<byte> bytes, int bitCount)
            : base(bytes, bitCount)
        {
            if (bitCount < MinBits)
                throw new ArgumentOutOfRangeException($"Must be >= {MinBits}!", nameof(bitCount));
            if (bitCount > MaxBits)
                throw new ArgumentOutOfRangeException($"{typeof(BitwiseByteEnumerableInt32)} can only work " +
                                                      $"with a maximum of {MaxBits} bits!", nameof(bitCount));
        }

        /// <summary>
        /// Checks if the given container is clear.
        /// </summary>
        /// <param name="container">The container to check.</param>
        /// <returns>True if clear, false if not.</returns>
        protected override void ClearContainer(ref int container) => container = 0;

        /// <summary>
        /// Adds bits from the given byte to the container.
        /// </summary>
        /// <param name="container">The container to add to.</param>
        /// <param name="bits">A byte containing the bits to add. The bits should
        ///     be justified to the right end of the byte.</param>
        /// <param name="count">The number of bits being added.</param>
        protected override void AddToContainer(ref int container, byte bits, int count) => container = (container << count) | bits;
    }
}
