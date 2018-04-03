using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// Defines public members for retrieving <see cref="IEqualityComparer{T}"/> instances for given types.
    /// </summary>
    public interface IEqualityComparerProvider
    {
        /// <summary>
        /// Gets an equality comparer for the given type.
        /// </summary>
        /// <returns>The equality comparer.</returns>
        IEqualityComparer<T> GetEqualityComparerFor<T>();
    }
}
