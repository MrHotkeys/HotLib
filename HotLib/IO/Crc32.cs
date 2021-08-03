using System;
using System.Collections.Generic;

using HotLib.Bits;

namespace HotLib.IO
{
    /// <summary>
    /// Represents a CRC-32 hash. Immutable.
    /// </summary>
    public struct Crc32
    {
        /// <summary>
        /// Represents a pre-calculated CRC table for a certain polynomial.
        /// </summary>
        private sealed class CrcTable
        {
            /// <summary>
            /// Gets the actual array of the table's values.
            /// </summary>
            private uint[] Table { get; }

            /// <summary>
            /// Gets the polynomial the table corresponds to.
            /// </summary>
            public uint Polynomial { get; }

            /// <summary>
            /// Instantiates a new <see cref="CrcTable"/>.
            /// </summary>
            /// <param name="polynomial">The polynomial the table corresponds to.</param>
            public CrcTable(uint polynomial)
            {
                Polynomial = polynomial;

                // https://www.w3.org/TR/PNG-CRCAppendix.html

                var polynomialReversed = BitHelpers.ReverseBits(polynomial);

                Table = new uint[256];
                for (var n = 0; n < 256; n++)
                {
                    var c = (uint)n;
                    for (var k = 0; k < 8; k++)
                    {
                        if ((c & 1) != 0)
                            c = (c >> 1) ^ polynomialReversed;
                        else
                            c = c >> 1;
                    }
                    Table[n] = c;
                }
            }

            /// <summary>
            /// Gets the pre-calculated value at the given index.
            /// </summary>
            /// <param name="index">The index of the value to get.</param>
            /// <returns>The pre-calculated value at the given index.</returns>
            public uint this[int index] => Table[index];

            /// <summary>
            /// Gets the pre-calculated value at the given index.
            /// </summary>
            /// <param name="index">The index of the value to get.</param>
            /// <returns>The pre-calculated value at the given index.</returns>
            public uint this[uint index] => Table[index];
        }

        /// <summary>
        /// Gets the dictionary of all CRC tables that have been calculated so far.
        /// </summary>
        private static readonly Dictionary<uint, CrcTable> CrcTables;

        /// <summary>
        /// The initial CRC value used for the standard CRC-32 setup.
        /// </summary>
        public const uint StandardInitial = 0xFFFFFFFF;
        /// <summary>
        /// The polynomial used for the standard CRC-32 setup.
        /// </summary>
        public const uint StandardPolynomial = 0x04C11DB7;
        /// <summary>
        /// The final XOR value used for the standard CRC-32 setup.
        /// </summary>
        public const uint StandardFinalXor = 0xFFFFFFFF;

        /// <summary>
        /// Gets a starting CRC-32 value with 0xFFFFFFFF as the initial value and final XOR, and 0x04C11DB7 as the polynomial.
        /// </summary>
        public static readonly Crc32 Standard;

        /// <summary>
        /// Initializes static values in <see cref="Crc32"/>.
        /// We do this explicitly here since <see cref="CrcTables"/> must be initialized before <see cref="Standard"/>.
        /// </summary>
        static Crc32()
        {
            CrcTables = new Dictionary<uint, CrcTable>();
            Standard = new Crc32(StandardInitial, StandardPolynomial, StandardFinalXor, true);
        }

        /// <summary>
        /// The actual value of the CRC.
        /// </summary>
        private uint Crc { get; }

        /// <summary>
        /// Gets the value of the CRC after the final XOR is applied.
        /// </summary>
        public uint Value => Crc ^ FinalXor;

        /// <summary>
        /// Gets the CRC table that will be used, based on its polynomial.
        /// </summary>
        private CrcTable CorrespondingCrcTable { get; }

        /// <summary>
        /// Gets the final XOR value to apply to the CRC before returned.
        /// </summary>
        private uint FinalXor { get; }

        /// <summary>
        /// Instantiates a new <see cref="Crc32"/>.
        /// </summary>
        /// <param name="initial">The initial value of the CRC.</param>
        /// <param name="polynomal">The polynomial to use for calculating the CRC.</param>
        /// <param name="finalXor">The final XOR value to apply to the CRC before returned.</param>
        /// <param name="cacheCrcTable">Whether or not to cache the generated CRC table in a static value in the type.
        ///     If this CRC is used as the starting point for others, the CRC table will be carried over from it.
        ///     The cache is only used if this constructor is called again with the same polynomial.</param>
        public Crc32(uint initial, uint polynomal, uint finalXor, bool cacheCrcTable)
            : this(initial, GetCrcTable(polynomal, cacheCrcTable), finalXor)
        { }

        /// <summary>
        /// Instantiates a new <see cref="Crc32"/>.
        /// </summary>
        /// <param name="crc">The value of the CRC.</param>
        /// <param name="correspondingCrcTable">The CRC table that will be used, based on its polynomial.</param>
        /// <param name="finalXor">The final XOR value to apply to the CRC before returned.</param>
        private Crc32(uint crc, CrcTable correspondingCrcTable, uint finalXor)
        {
            Crc = crc;
            CorrespondingCrcTable = correspondingCrcTable;
            FinalXor = finalXor;
        }

        /// <summary>
        /// Instantiates a new <see cref="Crc32"/> from an existing CRC-32 value.
        /// </summary>
        /// <param name="crc">The value to instntiate from. It is assumed that that
        ///     final XOR value has already been applied.</param>
        /// <param name="polynomal">The polynomial to use for calculating the CRC.</param>
        /// <param name="finalXor">The final XOR value to apply to the CRC before returned.</param>
        /// <param name="cacheCrcTable">Whether or not to cache the generated CRC table in a static value in the type.
        ///     If this CRC is used as the starting point for others, the CRC table will be carried over from it.
        ///     The cache is only used if this constructor is called again with the same polynomial.</param>
        /// <returns>The created <see cref="Crc32"/>.</returns>
        public static Crc32 FromCrc(uint crc, uint polynomial, uint finalXor, bool cacheCrcTable)
        {
            return new Crc32(crc ^ finalXor, polynomial, finalXor, cacheCrcTable);
        }

        /// <summary>
        /// Gets the corresponding CRC table for the given polynomial.
        /// </summary>
        /// <param name="polynomial">The polynomial to get a CRC table for.</param>
        /// <param name="cacheCrcTable">Whether or not to cache the generated CRC table in <see cref="CrcTables"/>.
        ///     If this CRC is used as the starting point for others, the CRC table will be carried over from it.
        ///     The cache is only used if this constructor is called again with the same polynomial.</param>
        /// <returns></returns>
        private static CrcTable GetCrcTable(uint polynomial, bool cacheCrcTable)
        {
            if (!CrcTables.TryGetValue(polynomial, out var crcTable))
            {
                crcTable = new CrcTable(polynomial);

                if (cacheCrcTable)
                    CrcTables[polynomial] = crcTable;
            }

            return crcTable;
        }

        /// <summary>
        /// Updates the CRC with the given byte and returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <param name="value">The byte to update the CRC with.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        public unsafe Crc32 Update(byte value)
        {
            var newCrc = UpdateCrc(Crc, value, CorrespondingCrcTable);
            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the bytes from the given unmanaged value and
        /// returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <typeparam name="T">The type of value to update the CRC with. Must be unmanaged.</typeparam>
        /// <param name="value">The value to update the CRC with.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        public unsafe Crc32 Update<T>(T value)
            where T : unmanaged
        {
            var ptr = (byte*)&value;
            var newCrc = Crc;

            for (var i = 0; i < sizeof(T); i++)
                newCrc = UpdateCrc(newCrc, ptr[i], CorrespondingCrcTable);

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the bytes from the given unmanaged value and
        /// returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <typeparam name="T">The type of value to update the CRC with. Must be unmanaged.</typeparam>
        /// <param name="value">The value to update the CRC with.</param>
        /// <param name="endianness">The intended endianness of the bytes. If it does not match the
        ///     system's endianness, the bytes will be used to update the CRC in reverse order, such
        ///     that their endianness would match the requested endianness.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="endianness"/> is not defined in <see cref="Endianness"/>.</exception>
        public unsafe Crc32 Update<T>(T value, Endianness endianness)
            where T : unmanaged
        {
            var ptr = (byte*)&value;
            var newCrc = Crc;

            // Wrap the call to Endianness.MatchesSystemEndianness() to catch any exceptions if it is invalid
            bool MatchesSystemEndianness()
            {
                try
                {
                    return endianness.MatchesSystemEndianness();
                }
                catch (ArgumentException e)
                {
                    throw new ArgumentException(e.Message, nameof(endianness), e);
                }
            }

            if (MatchesSystemEndianness())
            {
                for (var i = 0; i < sizeof(T); i++)
                    newCrc = UpdateCrc(newCrc, ptr[i], CorrespondingCrcTable);
            }
            else
            {
                for (var i = 0; i < sizeof(T); i++)
                    newCrc = UpdateCrc(newCrc, ptr[sizeof(T) - i - 1], CorrespondingCrcTable);
            }

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the given array of bytes and returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <param name="bytes">The array of bytes to update the CRC with.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public Crc32 Update(byte[] bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var newCrc = Crc;

            for (var i = 0; i < bytes.Length; i++)
                newCrc = UpdateCrc(newCrc, bytes[i], CorrespondingCrcTable);

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the given array of bytes and returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <param name="bytes">The array of bytes to update the CRC with.</param>
        /// <param name="offset">The offset in the array from which to start pulling bytes to update the CRC.</param>
        /// <param name="count">The number of bytes to pull from the array to update the CRC.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is invalid.</exception>
        public Crc32 Update(byte[] bytes, int offset, int count)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));
            if (offset < 0 || offset >= bytes.Length)
                throw new ArgumentOutOfRangeException("Must be within bounds of the byte array!", nameof(offset));
            if (count < 0)
                throw new ArgumentOutOfRangeException("Must be >= 0!", nameof(count));
            if (offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException("Buffer offset + byte count must be within the bounds of the buffer!", nameof(count));

            var newCrc = Crc;

            for (var i = 0; i < count; i++)
                newCrc = UpdateCrc(newCrc, bytes[i + offset], CorrespondingCrcTable);

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the given span of bytes and returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <param name="bytes">The span of bytes to update the CRC with.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        public Crc32 Update(Span<byte> bytes)
        {
            var newCrc = Crc;

            for (var i = 0; i < bytes.Length; i++)
                newCrc = UpdateCrc(newCrc, bytes[i], CorrespondingCrcTable);

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates the CRC with the given enumerable of bytes and returns a new <see cref="Crc32"/> with the updated CRC value.
        /// </summary>
        /// <param name="bytes">The enumerable of bytes to update the CRC with.</param>
        /// <returns>The updated <see cref="Crc32"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public Crc32 Update(IEnumerable<byte> bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var newCrc = Crc;

            foreach (var b in bytes)
                newCrc = UpdateCrc(newCrc, b, CorrespondingCrcTable);

            return new Crc32(newCrc, CorrespondingCrcTable, FinalXor);
        }

        /// <summary>
        /// Updates a CRC value directly.
        /// </summary>
        /// <param name="crc">The CRC to update.</param>
        /// <param name="b">The byte to update the CRC with.</param>
        /// <param name="crcTable">The CRC table to use when updating the CRC.</param>
        /// <returns></returns>
        private static uint UpdateCrc(uint crc, byte b, CrcTable crcTable)
        {
            return crc = (crc >> 8) ^ crcTable[(crc ^ b) & 0xFF];
        }

        /// <summary>
        /// Gets if the two <see cref="Crc32"/> values are equal by comparing their values.
        /// </summary>
        /// <param name="a">The first CRC to compare.</param>
        /// <param name="b">The second CRC to compare.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool Equals(Crc32 a, Crc32 b) => a.Crc == b.Crc;

        /// <summary>
        /// Gets if the <see cref="Crc32"/> is equal to the given CRC.
        /// </summary>
        /// <param name="other">The CRC to test against.</param>
        /// <returns>True if equal, false if not.</returns>
        public bool Equals(Crc32 other) => Equals(this, other);

        /// <summary>
        /// Gets if the <see cref="Crc32"/> is equal to the given object.
        /// </summary>
        /// <param name="obj">The object to test against.</param>
        /// <returns>True if equal, false if not.</returns>
        public override bool Equals(object? obj) => obj is Crc32 other && Equals(this, other);

        /// <summary>
        /// Gets a hash code for the <see cref="Crc32"/>.
        /// </summary>
        /// <returns>The created hash code.</returns>
        public override int GetHashCode() => Crc.GetHashCode();

        /// <summary>
        /// Gets if the two <see cref="Crc32"/> values are equal by comparing their values.
        /// </summary>
        /// <param name="a">The first CRC to compare.</param>
        /// <param name="b">The second CRC to compare.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator ==(Crc32 a, Crc32 b) => Equals(a, b);

        /// <summary>
        /// Gets if the two <see cref="Crc32"/> values are unequal by comparing their values.
        /// </summary>
        /// <param name="a">The first CRC to compare.</param>
        /// <param name="b">The second CRC to compare.</param>
        /// <returns>True if unequal, false if not.</returns>
        public static bool operator !=(Crc32 a, Crc32 b) => !Equals(a, b);

        /// <summary>
        /// Gets a string representation of the CRC, padded to 8 characters.
        /// </summary>
        /// <returns>The created string representation.</returns>
        public override string ToString() => Value.ToString("X8");
    }
}
