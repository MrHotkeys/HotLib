using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HotLib
{
    /// <summary>
    /// Provides quick byte conversion which does not require
    /// any math using <see cref="StructLayoutAttribute"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct StructOffsetByteConverter : IEnumerable<byte>
    {
        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 0.
        /// </summary>
        [FieldOffset(0)]
        public byte Byte0;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 1.
        /// </summary>
        [FieldOffset(1)]
        public byte Byte1;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 2.
        /// </summary>
        [FieldOffset(2)]
        public byte Byte2;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 3.
        /// </summary>
        [FieldOffset(3)]
        public byte Byte3;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 4.
        /// </summary>
        [FieldOffset(4)]
        public byte Byte4;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 5.
        /// </summary>
        [FieldOffset(5)]
        public byte Byte5;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 6.
        /// </summary>
        [FieldOffset(6)]
        public byte Byte6;

        /// <summary>
        /// Gets the value of the <see cref="byte"/> at offset 7.
        /// </summary>
        [FieldOffset(7)]
        public byte Byte7;

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
        /// <exception cref="ArgumentException"><paramref name="count"/> is negative or larger than 8.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.
        ///     -or- <paramref name="index"/> is larger than the max index in <paramref name="bytes"/>.
        ///     -or- The values of <paramref name="index"/> and <paramref name="count"/> will
        ///     go out of bounds of <paramref name="bytes"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public StructOffsetByteConverter(byte[] bytes, int index, int count)
            : this() // Initialize all the fields with default values
        {
            CopyFrom(bytes, index, count);
        }

        /// <summary>
        /// Copies all bytes from the given array into the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <param name="bytes">An array of bytes to start with.</param>
        /// <param name="index">The index to start copying from.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <exception cref="ArgumentException"><paramref name="count"/> is negative or larger than 8.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative.
        ///     -or- <paramref name="index"/> is larger than the max index in <paramref name="bytes"/>.
        ///     -or- The values of <paramref name="index"/> and <paramref name="count"/> will
        ///     go out of bounds of <paramref name="bytes"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public void CopyFrom(byte[] bytes, int index, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (index < 0)
                throw new IndexOutOfRangeException("Cannot be negative!");
            if (index >= bytes.Length)
                throw new IndexOutOfRangeException("Index larger than max index in byte array!");
            if (count < 0)
                throw new ArgumentException("Cannot be negative!", nameof(count));
            if (count > 8)
                throw new ArgumentException("Cannot copy more than 8 bytes!", nameof(count));
            if (index + count > bytes.Length)
                throw new IndexOutOfRangeException("Byte array is too small for given index and count!");

            for (var offset = 0; offset < count; offset++)
                this[offset] = bytes[index + offset];
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
                    case 0: return Byte0;
                    case 1: return Byte1;
                    case 2: return Byte2;
                    case 3: return Byte3;
                    case 4: return Byte4;
                    case 5: return Byte5;
                    case 6: return Byte6;
                    case 7: return Byte7;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (offset)
                {
                    case 0: Byte0 = value; break;
                    case 1: Byte1 = value; break;
                    case 2: Byte2 = value; break;
                    case 3: Byte3 = value; break;
                    case 4: Byte4 = value; break;
                    case 5: Byte5 = value; break;
                    case 6: Byte6 = value; break;
                    case 7: Byte7 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Gets an enumerator for all eight <see cref="byte"/>
        /// values from the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <returns>The enumerator of bytes.</returns>
        public IEnumerator<byte> GetEnumerator() => GetEnumerator(8);

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
        /// Gets an enumerator for all eight <see cref="byte"/>
        /// values from the <see cref="StructOffsetByteConverter"/>.
        /// </summary>
        /// <returns>The enumerator of bytes.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
