using System;
using System.Collections.Generic;
using System.Text;

namespace HotLib.Bits
{
    internal static unsafe class GenericBitwiseOperationsHelper<T>
        where T : unmanaged
    {
        /// <summary>
        /// The number of bits in a byte.
        /// </summary>
        private const int BitsInByte = 8;

        private static int TotalBits { get; } = sizeof(T) * BitsInByte;

        private static int LeastSignificantByteIndex { get; } = BitConverter.IsLittleEndian ? 0 : sizeof(T) - 1;

        private static int LeastSignificantUShortIndex { get; } = BitConverter.IsLittleEndian ? 0 : sizeof(T) - 2;

        private static int DirectionFromLeastSignificant { get; } = BitConverter.IsLittleEndian ? 1 : -1;

        static GenericBitwiseOperationsHelper()
        { }

        public static T OrWithOffset(T a, byte b, uint offset)
        {
            if (offset >= TotalBits)
                throw new ArgumentException();

            uint byteOffset;
            byte bitOffset;
            long byteAddressModifier;
            if (offset > 8)
            {
                HotMath.DivRem8(offset, out byteOffset, out bitOffset);
                byteAddressModifier = byteOffset * DirectionFromLeastSignificant;
            }
            else
            {
                byteOffset = 0;
                bitOffset = (byte)offset;
                byteAddressModifier = 0;
            }

            if (sizeof(T) > 1)
            {
                var b16 = (ushort)(b >> bitOffset);

                var address = (byte*)&a + LeastSignificantUShortIndex + byteAddressModifier;

                *(ushort*)address |= b16;
            }
            else
            {
                b >>= bitOffset;

                var address = (byte*)&a + LeastSignificantByteIndex + byteAddressModifier;

                *address |= b;
            }

            return a;
        }

        public static byte MaskFrom(T a, byte mask, uint offset)
        {
            if (offset >= TotalBits)
                throw new ArgumentException();

            uint byteOffset;
            byte bitOffset;
            long byteAddressModifier;
            if (offset > 8)
            {
                HotMath.DivRem8(offset, out byteOffset, out bitOffset);
                byteAddressModifier = byteOffset * DirectionFromLeastSignificant;
            }
            else
            {
                byteOffset = 0;
                bitOffset = (byte)offset;
                byteAddressModifier = 0;
            }

            if (sizeof(T) > 1)
            {
                var mask16 = (ushort)(mask << bitOffset);

                var address = (byte*)&a + LeastSignificantUShortIndex + byteAddressModifier;

                var masked = *(ushort*)address & mask16;

                return (byte)(masked >> bitOffset);
            }
            else
            {
                mask <<= bitOffset;

                var address = (byte*)&a + LeastSignificantByteIndex + byteAddressModifier;

                var masked = *address & mask;

                return (byte)(masked >> bitOffset);
            }
        }

        //public static T AndWithOffset(T a, byte b, int offset)
        //{
        //    if (offset + 8 > TotalBits)
        //        throw new ArgumentException();

        //    HotMath.DivRem(offset, 8, out var byteOffset, out var bitOffset);

        //    var byteAddressModifier = byteOffset * DirectionFromLeastSignificant;

        //    if (sizeof(T) > 1)
        //    {
        //        var b16 = (ushort)(b << bitOffset);

        //        var address = (byte*)&a + LeastSignificantUShortIndex + byteAddressModifier;

        //        *(ushort*)address |= b16;
        //    }
        //    else
        //    {
        //        b <<= bitOffset;

        //        var address = (byte*)&a + LeastSignificantByteIndex + byteAddressModifier;

        //        *address |= b;
        //    }

        //    return a;
        //}
    }
}
