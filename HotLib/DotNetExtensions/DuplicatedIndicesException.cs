using System;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// The exception type thrown when  comes across a pair of items with the same index.
    /// </summary>
    [Serializable]
    public sealed class DuplicatedIndicesException : Exception
    {
        /// <summary>
        /// Instantiates a new <see cref="DuplicatedIndicesException"/>.
        /// </summary>
        /// <param name="first">The first object in the collision.</param>
        /// <param name="second">The second object in the collision.</param>
        /// <param name="index">The index where the collision occurred.</param>
        public DuplicatedIndicesException(object first, object second, int index)
            : base($"Given enumerable has two or more items ({first}, {second}) with same index {index}!")
        { }
    }
}
