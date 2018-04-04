using System.Collections.Generic;

namespace HotLib
{
    public sealed class DefaultComparerHashCodeGenerator : IHashCodeGenerator
    {
        /// <summary>
        /// Gets an immutable singleton instance of <see cref="DefaultComparerHashCodeGenerator"/>.
        /// </summary>
        public static DefaultComparerHashCodeGenerator Instance { get; } = new DefaultComparerHashCodeGenerator();

        /// <summary>
        /// Instantiates a new <see cref="DefaultComparerHashCodeGenerator"/>.
        /// </summary>
        private DefaultComparerHashCodeGenerator()
        { }

        public new int GetHashCode<T>(T target) => EqualityComparer<T>.Default.GetHashCode(target);
    }
}
