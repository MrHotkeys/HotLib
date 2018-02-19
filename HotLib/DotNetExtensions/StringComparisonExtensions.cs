using System;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="StringComparison"/>.
    /// </summary>
    public static class StringComparisonExtensions
    {
        /// <summary>
        /// Gets an equality comparer that uses the given comparison method for comparing strings.
        /// </summary>
        /// <param name="stringComparison">The StringComparison to translate.</param>
        /// <returns>The corresponding equality comparer.</returns>
        public static StringComparer GetEqualityComparer(this StringComparison stringComparison)
        {
            switch (stringComparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
