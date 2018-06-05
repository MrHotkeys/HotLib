using System;

namespace HotLib.Bits
{
    /// <summary>
    /// The different byte orders a system may use.
    /// </summary>
    public enum Endianness
    {
        BigEndian,
        LittleEndian,
    }

    /// <summary>
    /// Contains public static extension methods for <see cref="Endianness"/>.
    /// </summary>
    public static class EndiannessExtensions
    {
        /// <summary>
        /// Gets if the given <see cref="Endianness"/> value matches the endianness the current system running the code uses.
        /// </summary>
        /// <param name="endianness">The endianness value to check.</param>
        /// <returns>True if the value matches the system's endianness, false if not.</returns>
        /// <exception cref="ArgumentException"><paramref name="endianness"/> is not defined in <see cref="Endianness"/>.</exception>
        public static bool MatchesSystemEndianness(this Endianness endianness)
        {
            switch (endianness)
            {
                case Endianness.BigEndian:
                    return !BitConverter.IsLittleEndian;
                case Endianness.LittleEndian:
                    return BitConverter.IsLittleEndian;
                default:
                    throw new ArgumentException($"Invalid {typeof(Endianness)} value {endianness}!", nameof(endianness));
            }
        }
    }
}
