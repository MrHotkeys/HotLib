using System;
using System.Text;

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
        /// Gets a <see cref="UInt16"/> containing a bit mask of the given size, dressed to the right (least significant end).
        /// </summary>
        /// <param name="maskSize">The size of the mask to get.</param>
        /// <returns>The created mask.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maskSize"/>
        ///     is less than 0 or greater than 16.</exception>
        public static ushort GetRightMaskUInt16(int maskSize)
        {
            switch (maskSize)
            {
                case 00: return 0;
                case 01: return 0b0000000000000001;
                case 02: return 0b0000000000000011;
                case 03: return 0b0000000000000111;
                case 04: return 0b0000000000001111;
                case 05: return 0b0000000000011111;
                case 06: return 0b0000000000111111;
                case 07: return 0b0000000001111111;
                case 08: return 0b0000000011111111;
                case 09: return 0b0000000111111111;
                case 10: return 0b0000001111111111;
                case 11: return 0b0000011111111111;
                case 12: return 0b0000111111111111;
                case 13: return 0b0001111111111111;
                case 14: return 0b0011111111111111;
                case 15: return 0b0111111111111111;
                case 16: return 0b1111111111111111;
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

        /// <summary>
        /// Creates a string of all bits in the given value.
        /// </summary>
        /// <typeparam name="T">The type of value to get the bits from.</typeparam>
        /// <param name="value">The value to get the bits from.</param>
        /// <param name="swapEndianness">Whether or not to swap the endiannes of the value's bytes when printing.</param>
        /// <returns>A string of the value's bits.</returns>
        public unsafe static string GetBits<T>(T value, bool swapEndianness = false)
            where T : unmanaged
        {
            var stringBuilder = new StringBuilder();

            var ptr = stackalloc byte[sizeof(T)];
            *(T*)ptr = value;

            if (swapEndianness)
            {
                for (var byteOffset = sizeof(T) - 1; byteOffset >= 0; byteOffset--)
                    AppendBits(ptr[byteOffset]);
            }
            else
            {
                for (var byteOffset = 0; byteOffset < sizeof(T); byteOffset++)
                    AppendBits(ptr[byteOffset]);
            }

            return stringBuilder.ToString();

            void AppendBits(byte b)
            {
                for (var bitOffset = 0; bitOffset < 8; bitOffset++)
                {
                    const byte mask = 0b10000000;
                    var bit = (mask >> bitOffset) & b;
                    stringBuilder.Append(bit == 0 ? '0' : '1');
                }
            }
        }
    }
}
