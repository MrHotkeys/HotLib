using System;
using System.Collections.Generic;
using System.Linq;

using HotLib.Bits;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="Enum"/> instances.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Enumerates all single-bit flags the enum value has set.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum the value belongs to.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TEnum"/> of all set flags.</returns>
        public static IEnumerable<TEnum> EnumerateFlags<TEnum>(this Enum enumValue)
             where TEnum : struct, Enum
        {
            return EnumerateFlags<TEnum>(enumValue, false);
        }

        /// <summary>
        /// Enumerates all flags the enum value has set.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum the value belongs to.</typeparam>
        /// <param name="enumValue">The enum value to check.</param>
        /// <param name="includeMultiBitFlags">If true, flags that cover multiple bits will be included in the enumerable.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="TEnum"/> of all set flags.</returns>
        public static IEnumerable<TEnum> EnumerateFlags<TEnum>(this Enum enumValue, bool includeMultiBitFlags)
             where TEnum : struct, Enum
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"Generic type must be an enum!", nameof(TEnum));

            return Enum.GetValues(typeof(TEnum))
                       .Cast<Enum>()
                       .Where(value => includeMultiBitFlags || value.IsSingleBitFlag())
                       .Where(enumValue.HasFlag)
                       .Cast<TEnum>();
        }

        /// <summary>
        /// Upcasts the given enum value as the given type.
        /// </summary>
        /// <typeparam name="TUnderlying">The type to unbox as. Must match the enum value's underlying type.</typeparam>
        /// <param name="enumValue">The enum value to unbox.</param>
        /// <returns>The unboxed value.</returns>
        public static TUnderlying Upcast<TUnderlying>(this Enum enumValue)
            where TUnderlying : struct
        {
            try
            {
                return (TUnderlying)(object)enumValue;
            }
            catch (InvalidCastException e)
            {
                var enumValueType = enumValue.GetType();
                var underlyingType = Enum.GetUnderlyingType(enumValue.GetType());
                var targetType = typeof(TUnderlying);
                var msg = $"Cannot unbox enum value of type {enumValueType} (with underlying type " +
                          $"{underlyingType}) as {targetType}!";
                throw new InvalidCastException(msg, e);
            }
        }

        /// <summary>
        /// Gets if the enum value has a single bit set in it.
        /// </summary>
        /// <param name="enumValue">The enum value to test.</param>
        /// <returns>True if only one bit is set, false if otherwise.</returns>
        public static bool IsSingleBitFlag(this Enum enumValue)
        {
            switch (enumValue.GetTypeCode())
            {
                case TypeCode.SByte:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<sbyte>());
                case TypeCode.Byte:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<byte>());
                case TypeCode.Int16:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<short>());
                case TypeCode.UInt16:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<ushort>());
                case TypeCode.Int32:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<int>());
                case TypeCode.UInt32:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<uint>());
                case TypeCode.Int64:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<long>());
                case TypeCode.UInt64:
                    return BitHelpers.HasOneBitSet(enumValue.Upcast<ulong>());

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
                    throw new ArgumentException($"Enum {enumValue.GetType()} has out of spec underlying " +
                                                $"type {Enum.GetUnderlyingType(enumValue.GetType())}!",
                                                nameof(enumValue));
            }
        }
    }
}
