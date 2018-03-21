using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// Helps with creating hash codes from multiple values for overridden
    /// <see cref="object.GetHashCode"/> implementations. Immutable and
    /// implicitly casts to <see cref="int"/>.
    /// </summary>
    public struct HashCode
    {
        /// <summary>
        /// The starting value for the hashing algorithm.
        /// </summary>
        private const int StartHashValue = unchecked((int)2166136261);

        /// <summary>
        /// The starting hash to build from when using <see cref="For{T}(T)"/>.
        /// </summary>
        private static HashCode StartHash { get; } = new HashCode(StartHashValue);

        /// <summary>
        /// Gets the <see cref="int"/> value of the <see cref="HashCode"/>.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Instantiates a new <see cref="HashCode"/> from the given value.
        /// </summary>
        /// <param name="value">The value of the <see cref="HashCode"/>.</param>
        private HashCode(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a <see cref="HashCode"/> for the given value.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value">The value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value) => StartHash.And(value);

        /// <summary>
        /// Creates a new <see cref="HashCode"/> based off the current value and the given value.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value">the value to append the hash code with.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public HashCode And<T>(T value)
        {
            unchecked
            {
                var valueHash = EqualityComparer<T>.Default.GetHashCode(value);
                var fullHash = (Value * 16777619) ^ valueHash;
                return new HashCode(fullHash);
            }
        }

        /// <summary>
        /// Implicitly casts a <see cref="HashCode"/> to <see cref="int"/>
        /// by returning its the value of its <see cref="Value"/> property.
        /// </summary>
        /// <param name="hashCode">The <see cref="HashCode"/> to cast as an <see cref="int"/>.</param>
        public static implicit operator int(HashCode hashCode) => hashCode.Value;
    }
}
