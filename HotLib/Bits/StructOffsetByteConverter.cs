using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HotLib.Bits
{
    /// <summary>
    /// Provides quick byte conversion which does not require any
    /// math using <see cref="StructLayoutAttribute"/>. Mutable.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct StructOffsetByteConverter : IEnumerable<byte>
    {
        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 0.
        /// </summary>
        [FieldOffset(0)]
        public byte Byte00;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 1.
        /// </summary>
        [FieldOffset(1)]
        public byte Byte01;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 2.
        /// </summary>
        [FieldOffset(2)]
        public byte Byte02;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 3.
        /// </summary>
        [FieldOffset(3)]
        public byte Byte03;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 4.
        /// </summary>
        [FieldOffset(4)]
        public byte Byte04;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 5.
        /// </summary>
        [FieldOffset(5)]
        public byte Byte05;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 6.
        /// </summary>
        [FieldOffset(6)]
        public byte Byte06;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 7.
        /// </summary>
        [FieldOffset(7)]
        public byte Byte07;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 8.
        /// </summary>
        [FieldOffset(8)]
        public byte Byte08;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 9.
        /// </summary>
        [FieldOffset(9)]
        public byte Byte09;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 10.
        /// </summary>
        [FieldOffset(10)]
        public byte Byte10;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 11.
        /// </summary>
        [FieldOffset(11)]
        public byte Byte11;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 12.
        /// </summary>
        [FieldOffset(12)]
        public byte Byte12;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 13.
        /// </summary>
        [FieldOffset(13)]
        public byte Byte13;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 14.
        /// </summary>
        [FieldOffset(14)]
        public byte Byte14;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 15.
        /// </summary>
        [FieldOffset(15)]
        public byte Byte15;

        /// <summary>
        /// Gets the value of the first byte as an <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(0)]
        public sbyte SByte;

        /// <summary>
        /// Gets the value of the first 2 bytes as a <see cref="short"/>.
        /// </summary>
        [FieldOffset(0)]
        public short Short;

        /// <summary>
        /// Gets the value of the first 2 bytes as a <see cref="ushort"/>.
        /// </summary>
        [FieldOffset(0)]
        public ushort UShort;

        /// <summary>
        /// Gets the value of the first 4 bytes as an <see cref="int"/>.
        /// </summary>
        [FieldOffset(0)]
        public int Int;

        /// <summary>
        /// Gets the value of the first 4 bytes as a <see cref="uint"/>.
        /// </summary>
        [FieldOffset(0)]
        public uint UInt;

        /// <summary>
        /// Gets the value of the first 4 bytes as a <see cref="long"/>.
        /// </summary>
        [FieldOffset(0)]
        public long Long;

        /// <summary>
        /// Gets the value of the first 4 bytes as a <see cref="ulong"/>.
        /// </summary>
        [FieldOffset(0)]
        public ulong ULong;

        /// <summary>
        /// Gets the value of the first 4 bytes as a <see cref="float"/>.
        /// </summary>
        [FieldOffset(0)]
        public float Float;

        /// <summary>
        /// Gets the value of the first 8 bytes as a <see cref="double"/>.
        /// </summary>
        [FieldOffset(0)]
        public double Double;

        /// <summary>
        /// Gets the value of all 16 bytes as a <see cref="decimal"/>.
        /// </summary>
        [FieldOffset(0)]
        public decimal Decimal;

        /// <summary>
        /// Gets the value of all 16 bytes as a <see cref="System.DateTime"/>.
        /// </summary>
        [FieldOffset(0)]
        public DateTime DateTime;

        /// <summary>
        /// Gets the value of the first 8 bytes as a <see cref="System.TimeSpan"/>.
        /// </summary>
        [FieldOffset(0)]
        public TimeSpan TimeSpan;

        /// <summary>
        /// Gets the value of all 16 bytes as a <see cref="System.Guid"/>.
        /// </summary>
        [FieldOffset(0)]
        public Guid Guid;

        /// <summary>
        /// Gets the value of the first byte as a <see cref="bool"/>.
        /// </summary>
        [FieldOffset(0)]
        public bool Bool;

        /// <summary>
        /// Instantiates a new <see cref="StructOffsetByteConverter"/>, copying the bytes from the given byte array.
        /// </summary>
        /// <param name="bytes">An array of bytes to start with.</param>
        /// <param name="index">The index to start copying from.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="swapEndianness">If true, the endianness of the given byte array is swapped.</param>
        /// <exception cref="ArgumentException"><paramref name="count"/> is negative or larger than 16.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.
        ///     -or- <paramref name="index"/> is larger than the max index in <paramref name="bytes"/>.
        ///     -or- The values of <paramref name="index"/> and <paramref name="count"/> will
        ///     go out of bounds of <paramref name="bytes"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public StructOffsetByteConverter(byte[] bytes, int index, int count, bool swapEndianness = false)
            : this() // Initialize all the fields with default values
        {
            CopyFrom(bytes, index, count, swapEndianness);
        }

        /// <summary>
        /// Copies all bytes from the given array into the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <param name="bytes">An array of bytes to start with.</param>
        /// <param name="index">The index to start copying from.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="swapEndianness">If true, the endianness of the given byte array is swapped.</param>
        /// <exception cref="ArgumentException"><paramref name="count"/> is negative or larger than 16.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.
        ///     -or- <paramref name="index"/> is larger than the max index in <paramref name="bytes"/>.
        ///     -or- The values of <paramref name="index"/> and <paramref name="count"/> will
        ///     go out of bounds of <paramref name="bytes"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public void CopyFrom(byte[] bytes, int index, int count, bool swapEndianness = false)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (index < 0)
                throw new IndexOutOfRangeException("Cannot be negative!");
            if (index >= bytes.Length)
                throw new IndexOutOfRangeException("Index larger than max index in byte array!");
            if (count < 0)
                throw new ArgumentException("Cannot be negative!", nameof(count));
            if (count > 16)
                throw new ArgumentException("Cannot copy more than 16 bytes!", nameof(count));
            if (index + count > bytes.Length)
                throw new IndexOutOfRangeException("Byte array is too small for given index and count!");

            if (swapEndianness)
            {
                for (var offset = 0; offset < count; offset++)
                    this[count - offset - 1] = bytes[index + offset];
            }
            else
            {
                for (var offset = 0; offset < count; offset++)
                    this[offset] = bytes[index + offset];
            }
        }

        /// <summary>
        /// Copies bytes into the given array.
        /// </summary>
        /// <param name="bytes">The array to copy into.</param>
        /// <param name="index">The index to start copying into.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="swapEndianness">If true, the endianness of the given byte array is swapped.</param>
        /// <exception cref="ArgumentException"><paramref name="count"/> is negative or larger than 16.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.
        ///     -or- <paramref name="index"/> is larger than the max index in <paramref name="bytes"/>.
        ///     -or- The values of <paramref name="index"/> and <paramref name="count"/> will
        ///     go out of bounds of <paramref name="bytes"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public void CopyTo(byte[] bytes, int index, int count, bool swapEndianness = false)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (index < 0)
                throw new IndexOutOfRangeException("Cannot be negative!");
            if (index >= bytes.Length)
                throw new IndexOutOfRangeException("Index larger than max index in byte array!");
            if (count < 0)
                throw new ArgumentException("Cannot be negative!", nameof(count));
            if (count > 16)
                throw new ArgumentException("Cannot copy more than 16 bytes!", nameof(count));
            if (index + count > bytes.Length)
                throw new IndexOutOfRangeException("Byte array is too small for given index and count!");

            if (swapEndianness)
            {
                for (var offset = 0; offset < count; offset++)
                    bytes[index + offset] = this[count - offset - 1];
            }
            else
            {
                for (var offset = 0; offset < count; offset++)
                    bytes[index + offset] = this[offset];
            }
        }

        /// <summary>
        /// Gets/Sets the value of the <see cref="byte"/> at the given offset.
        /// </summary>
        /// <param name="offset">The offset of the <see cref="byte"/> to get/set.</param>
        /// <returns>The value of the <see cref="byte"/> at the given offset.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="offset"/> is negative or greater than 7.</exception>
        public byte this[int offset]
        {
            get
            {
                switch (offset)
                {
                    case  0: return Byte00;
                    case  1: return Byte01;
                    case  2: return Byte02;
                    case  3: return Byte03;
                    case  4: return Byte04;
                    case  5: return Byte05;
                    case  6: return Byte06;
                    case  7: return Byte07;
                    case  8: return Byte08;
                    case  9: return Byte09;
                    case 10: return Byte10;
                    case 11: return Byte11;
                    case 12: return Byte12;
                    case 13: return Byte13;
                    case 14: return Byte14;
                    case 15: return Byte15;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (offset)
                {
                    case  0: Byte00 = value; break;
                    case  1: Byte01 = value; break;
                    case  2: Byte02 = value; break;
                    case  3: Byte03 = value; break;
                    case  4: Byte04 = value; break;
                    case  5: Byte05 = value; break;
                    case  6: Byte06 = value; break;
                    case  7: Byte07 = value; break;
                    case  8: Byte08 = value; break;
                    case  9: Byte09 = value; break;
                    case 10: Byte10 = value; break;
                    case 11: Byte11 = value; break;
                    case 12: Byte12 = value; break;
                    case 13: Byte13 = value; break;
                    case 14: Byte14 = value; break;
                    case 15: Byte15 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets an enumerator for all 16 <see cref="byte"/>
        /// values from the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <returns>The enumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator() => GetEnumerator(16);

        /// <summary>
        /// Gets an enumerator for a specified number of <see cref="byte"/>
        /// values from the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <param name="count">The number of bytes to enumerate.</param>
        /// <returns>The enumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator(int count)
        {
            for (var offset = 0; offset < count; offset++)
                yield return this[offset];
        }

        /// <summary>
        /// Gets an enumerator for all 16 <see cref="byte"/>
        /// values from the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <returns>The enumerator of bytes.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
