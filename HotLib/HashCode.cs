using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// Helps with creating hash codes from multiple values for overridden
    /// <see cref="object.GetHashCode"/> implementations. Immutable and
    /// implicitly casts to <see cref="int"/>.
    /// </summary>
    /// <remarks>From Jon Skeet's StackOverflow answer (https://stackoverflow.com/a/263416)</remarks>
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
        /// Creates a <see cref="HashCode"/> from the given value.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value">The value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value) => StartHash.And(value);

        /// <summary>
        /// Creates a <see cref="HashCode"/> from the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value1">The first value to get a hash code for.</param>
        /// <param name="value2">The second value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value1, T value2) => StartHash.And(value1, value2);

        /// <summary>
        /// Creates a <see cref="HashCode"/> from the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value1">The first value to get a hash code for.</param>
        /// <param name="value2">The second value to get a hash code for.</param>
        /// <param name="value3">The third value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value1, T value2, T value3) => StartHash.And(value1, value2, value3);

        /// <summary>
        /// Creates a <see cref="HashCode"/> from the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value1">The first value to get a hash code for.</param>
        /// <param name="value2">The second value to get a hash code for.</param>
        /// <param name="value3">The third value to get a hash code for.</param>
        /// <param name="value4">The fourth value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value1, T value2, T value3, T value4) => StartHash.And(value1, value2, value3, value4);

        /// <summary>
        /// Creates a <see cref="HashCode"/> from the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to create the hash code from.</typeparam>
        /// <param name="value1">The first value to get a hash code for.</param>
        /// <param name="value2">The second value to get a hash code for.</param>
        /// <param name="value3">The third value to get a hash code for.</param>
        /// <param name="value4">The fourth value to get a hash code for.</param>
        /// <param name="value5">The fifth value to get a hash code for.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public static HashCode For<T>(T value1, T value2, T value3, T value4, T value5) => StartHash.And(value1, value2, value3, value4, value5);

        /// <summary>
        /// Creates a new <see cref="HashCode"/> based off the current value and the given value.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value">The value to append the hash code with.</param>
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
        /// Creates a new <see cref="HashCode"/> based off the current value and the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value1">The first value to append the hash code with.</param>
        /// <param name="value2">The second value to append the hash code with.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public HashCode And<T>(T value1, T value2)
        {
            unchecked
            {
                var value1Hash = EqualityComparer<T>.Default.GetHashCode(value1);
                var value2Hash = EqualityComparer<T>.Default.GetHashCode(value2);
                var fullHash = (Value * 16777619) ^ value1Hash;
                fullHash = (fullHash * 16777619) ^ value2Hash;
                return new HashCode(fullHash);
            }
        }

        /// <summary>
        /// Creates a new <see cref="HashCode"/> based off the current value and the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value1">The first value to append the hash code with.</param>
        /// <param name="value2">The second value to append the hash code with.</param>
        /// <param name="value3">The third value to append the hash code with.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public HashCode And<T>(T value1, T value2, T value3)
        {
            unchecked
            {
                var value1Hash = EqualityComparer<T>.Default.GetHashCode(value1);
                var value2Hash = EqualityComparer<T>.Default.GetHashCode(value2);
                var value3Hash = EqualityComparer<T>.Default.GetHashCode(value3);
                var fullHash = (Value * 16777619) ^ value1Hash;
                fullHash = (fullHash * 16777619) ^ value2Hash;
                fullHash = (fullHash * 16777619) ^ value3Hash;
                return new HashCode(fullHash);
            }
        }

        /// <summary>
        /// Creates a new <see cref="HashCode"/> based off the current value and the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value1">The first value to append the hash code with.</param>
        /// <param name="value2">The second value to append the hash code with.</param>
        /// <param name="value3">The third value to append the hash code with.</param>
        /// <param name="value4">The fourth value to append the hash code with.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public HashCode And<T>(T value1, T value2, T value3, T value4)
        {
            unchecked
            {
                var value1Hash = EqualityComparer<T>.Default.GetHashCode(value1);
                var value2Hash = EqualityComparer<T>.Default.GetHashCode(value2);
                var value3Hash = EqualityComparer<T>.Default.GetHashCode(value3);
                var value4Hash = EqualityComparer<T>.Default.GetHashCode(value4);
                var fullHash = (Value * 16777619) ^ value1Hash;
                fullHash = (fullHash * 16777619) ^ value2Hash;
                fullHash = (fullHash * 16777619) ^ value3Hash;
                fullHash = (fullHash * 16777619) ^ value4Hash;
                return new HashCode(fullHash);
            }
        }

        /// <summary>
        /// Creates a new <see cref="HashCode"/> based off the current value and the given values.
        /// </summary>
        /// <typeparam name="T">The type of value to append the hash code with.</typeparam>
        /// <param name="value1">The first value to append the hash code with.</param>
        /// <param name="value2">The second value to append the hash code with.</param>
        /// <param name="value3">The third value to append the hash code with.</param>
        /// <param name="value4">The fourth value to append the hash code with.</param>
        /// <param name="value5">The fifth value to append the hash code with.</param>
        /// <returns>The created <see cref="HashCode"/>.</returns>
        public HashCode And<T>(T value1, T value2, T value3, T value4, T value5)
        {
            unchecked
            {
                var value1Hash = EqualityComparer<T>.Default.GetHashCode(value1);
                var value2Hash = EqualityComparer<T>.Default.GetHashCode(value2);
                var value3Hash = EqualityComparer<T>.Default.GetHashCode(value3);
                var value4Hash = EqualityComparer<T>.Default.GetHashCode(value4);
                var value5Hash = EqualityComparer<T>.Default.GetHashCode(value5);
                var fullHash = (Value * 16777619) ^ value1Hash;
                fullHash = (fullHash * 16777619) ^ value2Hash;
                fullHash = (fullHash * 16777619) ^ value3Hash;
                fullHash = (fullHash * 16777619) ^ value4Hash;
                fullHash = (fullHash * 16777619) ^ value5Hash;
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
