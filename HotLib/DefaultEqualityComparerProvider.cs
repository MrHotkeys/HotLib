using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// An implementation of <see cref="IEqualityComparerProvider"/> which always returns
    /// the <see cref="EqualityComparer{T}.Default"/> for the given type.
    /// </summary>
    public sealed class DefaultEqualityComparerProvider : IEqualityComparerProvider
    {
        /// <summary>
        /// Gets an immutable singleton instance of <see cref="DefaultEqualityComparerProvider"/>.
        /// </summary>
        public static DefaultEqualityComparerProvider Instance { get; } = new DefaultEqualityComparerProvider();

        /// <summary>
        /// Instantiates a new <see cref="DefaultEqualityComparerProvider"/>.
        /// </summary>
        private DefaultEqualityComparerProvider()
        { }

        /// <summary>
        /// Gets the default equality comparer for the given type.
        /// </summary>
        /// <returns>The default equality comparer.</returns>
        public IEqualityComparer<T> GetEqualityComparerFor<T>() => EqualityComparer<T>.Default;
    }
}
