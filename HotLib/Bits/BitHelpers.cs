using System;

namespace HotLib.Bits
{
    /// <summary>
    /// Contains public static helper classes for working with bits.
    /// </summary>
    public static class BitHelpers
    {
        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(sbyte value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(byte value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(short value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(ushort value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(int value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(uint value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(long value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets if the value has a single bit set (true) or not (false).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if one bit set, false if zero or more.</returns>
        public static bool HasOneBitSet(ulong value) => (value != 0) && ((value & (value - 1)) == 0);

        /// <summary>
        /// Gets a byte containing a bit mask of the given size, dressed to the right (least significant end).
        /// </summary>
        /// <param name="maskSize">The size of the mask to get.</param>
        /// <returns>The created mask.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maskSize"/>
        ///     is less than 0 or greater than 8.</exception>
        public static byte GetRightMaskByte(int maskSize)
        {
            switch (maskSize)
            {
                case 0: return 0;
                case 1: return 0b00000001;
                case 2: return 0b00000011;
                case 3: return 0b00000111;
                case 4: return 0b00001111;
                case 5: return 0b00011111;
                case 6: return 0b00111111;
                case 7: return 0b01111111;
                case 8: return 0b11111111;
                default: throw new ArgumentOutOfRangeException("Must be >= 0 and <= 8!", nameof(maskSize));
            }
        }

        /// <summary>
        /// Gets a byte containing a bit mask of the given size, dressed to the left (most significant end).
        /// </summary>
        /// <param name="maskSize">The size of the mask to get.</param>
        /// <returns>The created mask.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maskSize"/>
        ///     is less than 0 or greater than 8.</exception>
        public static byte GetLeftMaskByte(int maskSize)
        {
            switch (maskSize)
            {
                case 0: return 0;
                case 1: return 0b10000000;
                case 2: return 0b11000000;
                case 3: return 0b11100000;
                case 4: return 0b11110000;
                case 5: return 0b11111000;
                case 6: return 0b11111100;
                case 7: return 0b11111110;
                case 8: return 0b11111111;
                default: throw new ArgumentOutOfRangeException("Must be >= 0 and <= 8!", nameof(maskSize));
            }
        }
    }
}
