﻿namespace HotLib.Equality
{
    /// <summary>
    /// Defines public members for testing equality generically between objects of the same type.
    /// </summary>
    public interface IUniversalEqualityComparer
    {
        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <typeparam name="T">The type of the objects being compared.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the objects are equal, <see langword="false"/> if not.</returns>
        bool Equals<T>(T x, T y);

        /// <summary>
        /// Gets a hash code for an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to get a hash code for.</typeparam>
        /// <param name="obj">The object to get a hash code for.</param>
        /// <returns>The object's hash code.</returns>
        int GetHashCode<T>(T obj);
    }
}
