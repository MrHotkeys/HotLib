using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HotLib.Bits
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DoubleUInt64
    {
        /// <summary>
        /// The less significant <see langword="ulong"/>.
        /// </summary>
        [FieldOffset(sizeof(ulong) * 0)]
        public ulong ULong1;
        /// <summary>
        /// The more significant <see langword="ulong"/>.
        /// </summary>
        [FieldOffset(sizeof(ulong) * 1)]
        public ulong ULong2;

        public DoubleUInt64(ulong ul1, ulong ul2)
        {
            ULong1 = ul1;
            ULong2 = ul2;
        }

        public static DoubleUInt64 operator <<(DoubleUInt64 dul, int bits)
        {
            if (bits < 0)
                return dul >> Math.Abs(bits);
            if (bits == 0)
                return dul;

            // We only care about the low order seven bits (consistent with bit shifting ulongs using only 6)
            const int ShiftMask = (1 << 7) - 1;
            bits &= ShiftMask;

            // Shift the more significant ulong - the overflow bits have nowhere to go and are just getting tossed
            if (bits < 64)
                dul.ULong2 <<= bits;
            else
                dul.ULong2 = 0;

            // Move the overflow bits from the less significant ulong to the more significant ulong
            // 
            // Start with a mask for the least significant overflow bit, subtract one to get a mask
            // for all non-overflow bits, and use a bitwise NOT to get a mask for all overflow bits
            var bitsNotMasked = bits < 64 ? 64 - bits : 0;
            var leastSignificantOverflowBitMask = (ulong)1 << bitsNotMasked;
            var mask = ~(leastSignificantOverflowBitMask - 1);
            dul.ULong2 |= (dul.ULong1 & mask) >> bitsNotMasked;

            // If we're shifting by more than 64, we need to shift the receiving ulong again
            if (bits > 64)
                dul.ULong2 <<= bits - 64;

            // Shift the less significant ulong
            if (bits < 64)
                dul.ULong1 <<= bits;
            else
                dul.ULong1 = 0;

            return dul;
        }

        public static DoubleUInt64 operator >>(DoubleUInt64 dul, int bits)
        {
            if (bits < 0)
                return dul << Math.Abs(bits);
            if (bits == 0)
                return dul;

            // We only care about the low order seven bits (consistent with bit shifting ulongs using only 6)
            const int ShiftMask = (1 << 7) - 1;
            bits &= ShiftMask;

            // Shift the less significant ulong - the overflow bits have nowhere to go and are just getting tossed
            if (bits < 64)
                dul.ULong1 >>= bits;
            else
                dul.ULong1 = 0;

            // Move the overflow bits from the more significant ulong to the less significant ulong
            //
            // Start with a mask for the least significant non-overflow bit and subtract one to get a mask
            // for all overflow bits, unless we are shifting by 64 or more in which case just use ulong.MaxValue
            var bitsNotMasked = bits < 64 ? 64 - bits : 0;
            var mask = bits < 64 ?
                       ((ulong)1 << bits) - 1 :
                       ulong.MaxValue;
            dul.ULong1 |= (dul.ULong2 & mask) << bitsNotMasked;
            
            // If we're shifting by more than 64, we need to shift the receiving ulong again
            if (bits > 64)
                dul.ULong1 >>= bits - 64;

            // Shift the more significant ulong
            if (bits < 64)
                dul.ULong2 >>= bits;
            else
                dul.ULong2 = 0;

            return dul;
        }

        public static DoubleUInt64 operator |(DoubleUInt64 dul, ulong ul) => new DoubleUInt64(dul.ULong1 | ul, dul.ULong2);
        public static DoubleUInt64 operator &(DoubleUInt64 dul, ulong ul) => new DoubleUInt64(dul.ULong1 & ul, dul.ULong2);
        public static DoubleUInt64 operator ~(DoubleUInt64 dul) => new DoubleUInt64(~dul.ULong1, ~dul.ULong2);

        public static explicit operator ulong(DoubleUInt64 dul) => dul.ULong1;
        public static explicit operator long(DoubleUInt64 dul) => (long)dul.ULong1;

        public static explicit operator uint(DoubleUInt64 dul) => (uint)dul.ULong1;
        public static explicit operator int(DoubleUInt64 dul) => (int)dul.ULong1;

        public static explicit operator ushort(DoubleUInt64 dul) => (ushort)dul.ULong1;
        public static explicit operator short(DoubleUInt64 dul) => (short)dul.ULong1;

        public static explicit operator byte(DoubleUInt64 dul) => (byte)dul.ULong1;
        public static explicit operator sbyte(DoubleUInt64 dul) => (sbyte)dul.ULong1;

        public static explicit operator float(DoubleUInt64 dul) => dul.ULong1;
        public static explicit operator double(DoubleUInt64 dul) => dul.ULong1;
    }
}
