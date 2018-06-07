using System;
using System.Collections.Generic;
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
        public unsafe static string GetBitString<T>(T value, bool swapEndianness = false)
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

        /// <summary>
        /// Converts a series of bytes into an unmanaged value.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to convert the bytes to. Must be unmanaged.</typeparam>
        /// <param name="bytes">The bytes to convert. Will be taken starting from the beginning of the span.</param>
        /// <param name="endianness">The endianness of the given bytes. If it doesn't match the system's endianness,
        ///     the given bytes will be used in reverse order to create the value, such that the endianness of the
        ///     created value will match the system's.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="bytes"/> does not contain enough bytes.</exception>
        public unsafe static T To<T>(Span<byte> bytes, Endianness endianness)
            where T : unmanaged
        {
            var ptr = stackalloc byte[sizeof(T)];

            try
            {
                if (endianness.MatchesSystemEndianness())
                {
                    for (var i = 0; i < sizeof(T); i++)
                        ptr[i] = bytes[i];
                }
                else
                {
                    for (var i = 0; i < sizeof(T); i++)
                        ptr[sizeof(T) - i - 1] = bytes[i];
                }
            }
            catch (IndexOutOfRangeException e)
            {
                if (bytes.Length < sizeof(T))
                {
                    throw new ArgumentException($"Not enough bytes for {typeof(T)} (got {bytes.Length}, " +
                                                $"expected {sizeof(T)}!", nameof(bytes), e);
                }
                else
                {
                    throw;
                }
            }

            return *(T*)ptr;
        }

        /// <summary>
        /// Converts a series of bytes into an unmanaged value.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to convert the bytes to. Must be unmanaged.</typeparam>
        /// <param name="bytes">A pointer to the bytes to convert.</param>
        /// <param name="endianness">The endianness of the given bytes. If it doesn't match the system's endianness,
        ///     the given bytes will be used in reverse order to create the value, such that the endianness of the
        ///     created value will match the system's.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="bytes"/> does not contain enough bytes.</exception>
        public unsafe static T To<T>(byte* bytes, Endianness endianness)
            where T : unmanaged
        {
            var ptr = stackalloc byte[sizeof(T)];

            if (endianness.MatchesSystemEndianness())
            {
                for (var i = 0; i < sizeof(T); i++)
                    ptr[i] = bytes[i];
            }
            else
            {
                for (var i = 0; i < sizeof(T); i++)
                    ptr[sizeof(T) - i - 1] = bytes[i];
            }

            return *(T*)ptr;
        }

        /// <summary>
        /// Converts a series of bytes into an unmanaged value.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to convert the bytes to. Must be unmanaged.</typeparam>
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="offset">The offset from which to start copying in <paramref name="bytes"/>.</param>
        /// <param name="endianness">The endianness of the given bytes. If it doesn't match the system's endianness,
        ///     the given bytes will be used in reverse order to create the value, such that the endianness of the
        ///     created value will match the system's.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="bytes"/> does not contain enough bytes.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public unsafe static T To<T>(byte[] bytes, int offset, Endianness endianness)
            where T : unmanaged
        {
            var ptr = stackalloc byte[sizeof(T)];

            try
            {
                if (endianness.MatchesSystemEndianness())
                {
                    for (var i = 0; i < sizeof(T); i++)
                        ptr[i] = bytes[i + offset];
                }
                else
                {
                    for (var i = 0; i < sizeof(T); i++)
                        ptr[sizeof(T) - i - 1] = bytes[i + offset];
                }
            }
            catch (IndexOutOfRangeException e)
            {
                if (bytes.Length - offset < sizeof(T))
                {
                    throw new ArgumentException($"Not enough bytes for {typeof(T)} with offset {offset} (got " +
                                                $"{bytes.Length - offset}, expected {sizeof(T)}!", nameof(bytes), e);
                }
                else
                {
                    throw;
                }
            }
            catch (ArgumentNullException e)
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes), e);
                else
                    throw;
            }

            return *(T*)ptr;
        }


        /// <summary>
        /// Converts a series of bytes into an unmanaged value.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to convert the bytes to. Must be unmanaged.</typeparam>
        /// <param name="bytes">The bytes to convert.</param>
        /// <param name="endianness">The endianness of the given bytes. If it doesn't match the system's endianness,
        ///     the given bytes will be used in reverse order to create the value, such that the endianness of the
        ///     created value will match the system's.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> does not contain enough bytes.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public unsafe static T To<T>(IEnumerable<byte> bytes, Endianness endianness)
            where T : unmanaged
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var ptr = stackalloc byte[sizeof(T)];

            var enumerator = bytes.GetEnumerator();
            if (endianness.MatchesSystemEndianness())
            {
                for (var i = 0; i < sizeof(T); i++)
                {
                    if (enumerator.MoveNext())
                    {
                        ptr[i] = enumerator.Current;
                    }
                    else
                    {
                        throw new ArgumentException($"Not enough bytes for {typeof(T)} (got " +
                                                    $"{i}, expected {sizeof(T)}!", nameof(bytes));
                    }
                }
            }
            else
            {
                for (var i = 0; i < sizeof(T); i++)
                {
                    if (enumerator.MoveNext())
                    {
                        ptr[sizeof(T) - i - 1] = enumerator.Current;
                    }
                    else
                    {
                        throw new ArgumentException($"Not enough bytes for {typeof(T)} (got " +
                                                    $"{i}, expected {sizeof(T)}!", nameof(bytes));
                    }
                }
            }

            return *(T*)ptr;
        }

        /// <summary>
        /// Gets every <see cref="byte"/> from the given unmanaged value and inserts them into the given <see cref="Span{T}"/> of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to get the bytes of. Must be unmanaged.</typeparam>
        /// <param name="value">The value to get the bytes of.</param>
        /// <param name="bytes">The span to insert the bytes into.</param>
        /// <param name="endianness">The intended endianness of the written bytes. If it does not match the
        ///     system's endianness, the bytes will be written in reverse order, such that their endianness
        ///     will match the requested endianness.</param>
        /// <exception cref="IndexOutOfRangeException"><paramref name="bytes"/> does not have enough room
        ///     for all the bytes from <paramref name="value"/>.</exception>
        public unsafe static void GetBytes<T>(T value, Span<byte> bytes, Endianness endianness)
            where T : unmanaged
        {
            var ptr = (byte*)&value;

            try
            {
                if (endianness.MatchesSystemEndianness())
                {
                    for (var i = 0; i < sizeof(T); i++)
                        bytes[i] = ptr[i];
                }
                else
                {
                    for (var i = 0; i < sizeof(T); i++)
                        bytes[i] = ptr[sizeof(T) - i - 1];
                }
            }
            catch (IndexOutOfRangeException e)
            {
                if (bytes.Length < sizeof(T))
                {
                    throw new ArgumentException($"Not room for all bytes from {typeof(T)} (got {bytes.Length}, " +
                                                $"expected {sizeof(T)}!", nameof(bytes), e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets every <see cref="byte"/> from the given unmanaged value and writes them in order to the given pointer.
        /// </summary>
        /// <typeparam name="T">The type of value to get the bytes of. Must be unmanaged.</typeparam>
        /// <param name="value">The value to get the bytes of.</param>
        /// <param name="bytes">The pointer to write the bytes at.</param>
        /// <param name="endianness">The intended endianness of the written bytes. If it does not match the
        ///     system's endianness, the bytes will be written in reverse order, such that their endianness
        ///     will match the requested endianness.</param>
        public unsafe static void GetBytes<T>(T value, byte* bytes, Endianness endianness)
            where T : unmanaged
        {
            var ptr = (byte*)&value;

            if (endianness.MatchesSystemEndianness())
            {
                for (var i = 0; i < sizeof(T); i++)
                    bytes[i] = ptr[i];
            }
            else
            {
                for (var i = 0; i < sizeof(T); i++)
                    bytes[i] = ptr[sizeof(T) - i - 1];
            }
        }

        /// <summary>
        /// Gets every <see cref="byte"/> from the given unmanaged value and inserts them into the given array of bytes.
        /// </summary>
        /// <typeparam name="T">The type of value to get the bytes of. Must be unmanaged.</typeparam>
        /// <param name="value">The value to get the bytes of.</param>
        /// <param name="bytes">The array to insert the bytes into.</param>
        /// <param name="endianness">The intended endianness of the written bytes. If it does not match the
        ///     system's endianness, the bytes will be written in reverse order, such that their endianness
        ///     will match the requested endianness.</param>
        /// <exception cref="IndexOutOfRangeException"><paramref name="bytes"/> does not have enough room
        ///     for all the bytes from <paramref name="value"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public unsafe static void GetBytes<T>(T value, byte[] bytes, int offset, Endianness endianness)
            where T : unmanaged
        {
            var ptr = (byte*)&value;

            try
            {
                if (endianness.MatchesSystemEndianness())
                {
                    for (var i = 0; i < sizeof(T); i++)
                        bytes[i + offset] = ptr[i];
                }
                else
                {
                    for (var i = 0; i < sizeof(T); i++)
                        bytes[i + offset] = ptr[sizeof(T) - i - 1];
                }
            }
            catch (IndexOutOfRangeException e)
            {
                if (bytes.Length < sizeof(T))
                {
                    throw new ArgumentException($"Not room for all bytes from {typeof(T)} (got {bytes.Length}, " +
                                                $"expected {sizeof(T)}!", nameof(bytes), e);
                }
                else
                {
                    throw;
                }
            }
            catch (ArgumentNullException e)
            {
                if (bytes == null)
                    throw new ArgumentNullException(nameof(bytes), e);
                else
                    throw;
            }
        }

        /// <summary>
        /// Gets every <see cref="byte"/> from the given unmanaged value and iterates them.
        /// </summary>
        /// <typeparam name="T">The type of value to get the bytes of. Must be unmanaged.</typeparam>
        /// <param name="value">The value to get the bytes of.</param>
        /// <param name="endianness">The intended endianness of the written bytes. If it does not match the
        ///     system's endianness, the bytes will be written in reverse order, such that their endianness
        ///     will match the requested endianness.</param>
        /// <returns>The created enumerable of bytes.</returns>
        public unsafe static IEnumerable<byte> GetBytes<T>(T value, Endianness endianness)
            where T : unmanaged
        {
            var wrapped = new WrappedPointer<T>(&value);
            var size = sizeof(T);

            IEnumerable<byte> Iterate()
            {
                if (endianness.MatchesSystemEndianness())
                {
                    for (var i = 0; i < size; i++)
                        yield return wrapped[i];
                }
                else
                {
                    for (var i = 0; i < size; i++)
                        yield return wrapped[size - i - 1];
                }
            }

            return Iterate();
        }

        /// <summary>
        /// Reverses the bits in the given value.
        /// </summary>
        /// <typeparam name="T">The type of value of which to reverse the bits. Must be unmanaged.</typeparam>
        /// <param name="value">The value of which to reverse the bits.</param>
        /// <returns>The value with reversed bits.</returns>
        public unsafe static T ReverseBits<T>(T value)
            where T : unmanaged
        {
            var valuePtr = (byte*)&value;
            var resultPtr = stackalloc byte[sizeof(T)];

            for (var byteOffset = 0; byteOffset < sizeof(T); byteOffset++)
            {
                for (var bitOffset = 0; bitOffset < 8; bitOffset++)
                {
                    // The mask to get the corresponding bit from the given value
                    var valueMask = 1 << bitOffset;

                    // The bit from the given value, in the rightmost place
                    var bit = (valuePtr[byteOffset] & valueMask) >> bitOffset;

                    // The bit from the given value, shifted to line up with where it's going in the result
                    var bitShifted = bit << (8 - bitOffset - 1);
                    
                    // Add the bit to the result
                    resultPtr[sizeof(T) - byteOffset - 1] |= (byte)bitShifted;
                }
            }

            return *(T*)resultPtr;
        }

        /// <summary>
        /// Reverses the bytes in the given value.
        /// </summary>
        /// <typeparam name="T">The type of value of which to reverse the bytes. Must be unmanaged.</typeparam>
        /// <param name="value">The value of which to reverse the bytes.</param>
        /// <returns>The value with reversed bytes.</returns>
        public unsafe static T ReverseBytes<T>(T value)
            where T : unmanaged
        {
            var valuePtr = (byte*)&value;
            var resultPtr = stackalloc byte[sizeof(T)];

            for (var byteOffset = 0; byteOffset < sizeof(T); byteOffset++)
                resultPtr[sizeof(T) - byteOffset - 1] = valuePtr[byteOffset];

            return *(T*)resultPtr;
        }
    }
}
