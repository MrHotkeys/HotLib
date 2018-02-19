using System;
using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// An file path comparer which queries the running OS to determine the comparison method.
    /// </summary>
    public sealed class OsSensitivePathComparer : IPathComparer, IEqualityComparer<string>
    {
        /// <summary>
        /// Gets the comparison method for paths.
        /// </summary>
        private StringComparison PathComparison { get; }

        /// <summary>
        /// Instantiates a new <see cref="OsSensitivePathComparer"/>.
        /// </summary>
        public OsSensitivePathComparer()
        {
            PathComparison = GetPathComparison();
        }

        /// <summary>
        /// Tests equality between the two paths, given the OS's path case sensitivity.
        /// </summary>
        /// <param name="pathA">The first path.</param>
        /// <param name="pathB">The second path.</param>
        /// <returns>True if equal, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pathA"/> or <paramref name="pathB"/> is null.</exception>
        public bool Equals(string pathA, string pathB)
        {
            if (pathA == null)
                throw new ArgumentNullException(nameof(pathA));
            if (pathB == null)
                throw new ArgumentNullException(nameof(pathB));

            return string.Equals(pathA, pathB, PathComparison);
        }

        /// <summary>
        /// Determines the string comparison method to use for paths on this system.
        /// </summary>
        /// <returns>The proper string comparison method for the system.</returns>
        private static StringComparison GetPathComparison()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return StringComparison.OrdinalIgnoreCase;
                case PlatformID.Unix:
                    return StringComparison.Ordinal;
                case PlatformID.MacOSX: // Can be either way and requires more testing, unfortunately...
                case PlatformID.Xbox:
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a hashcode for the given path.
        /// </summary>
        /// <param name="path">The path to hash.</param>
        /// <returns>The hashcode.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        int IEqualityComparer<string>.GetHashCode(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            return path.GetHashCode();
        }
    }
}
