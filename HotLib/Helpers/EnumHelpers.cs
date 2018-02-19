using System;

using HotLib.DotNetExtensions;

using static HotLib.Helpers.BitHelpers;

namespace HotLib.Helpers
{
    /// <summary>
    /// Contains public static helpers for <see cref="Enum"/> instances.
    /// </summary>
    public static class EnumHelpers
    {
        /// <summary>
        /// Gets if the enum value has a single bit set in it.
        /// </summary>
        /// <param name="enumValue">The enum value to test.</param>
        /// <returns>True if only one bit is set, false if otherwise.</returns>
        public static bool IsSingleBitFlag(Enum enumValue)
        {
            switch (enumValue.GetTypeCode())
            {
                case TypeCode.SByte:
                    return HasOneBitSet(enumValue.Unbox<sbyte>());
                case TypeCode.Byte:
                    return HasOneBitSet(enumValue.Unbox<byte>());
                case TypeCode.Int16:
                    return HasOneBitSet(enumValue.Unbox<short>());
                case TypeCode.UInt16:
                    return HasOneBitSet(enumValue.Unbox<ushort>());
                case TypeCode.Int32:
                    return HasOneBitSet(enumValue.Unbox<int>());
                case TypeCode.UInt32:
                    return HasOneBitSet(enumValue.Unbox<uint>());
                case TypeCode.Int64:
                    return HasOneBitSet(enumValue.Unbox<long>());
                case TypeCode.UInt64:
                    return HasOneBitSet(enumValue.Unbox<ulong>());

                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                default:
                    throw new ArgumentException($"Enum {enumValue.GetType()} has out of spec underlying type {Enum.GetUnderlyingType(enumValue.GetType())}!");
            }
        }
    }
}
