using System;
using System.Collections.Generic;
using System.Linq;

using static HotLib.Helpers.EnumHelpers;

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
             where TEnum : struct
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
             where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException($"Generic type must be an enum!", nameof(TEnum));

            return Enum.GetValues(typeof(TEnum))
                       .Cast<Enum>()
                       .Where(value => includeMultiBitFlags || IsSingleBitFlag(value))
                       .Where(enumValue.HasFlag)
                       .Cast<TEnum>();
        }

        /// <summary>
        /// Unboxes the given enum value as the given type.
        /// </summary>
        /// <typeparam name="TUnderlying">The type to unbox as. Must match the enum value's underlying type.</typeparam>
        /// <param name="enumValue">The enum value to unbox.</param>
        /// <returns>The unboxed value.</returns>
        public static TUnderlying Unbox<TUnderlying>(this Enum enumValue)
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
    }
}
