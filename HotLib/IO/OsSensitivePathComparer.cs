using System;
using System.Collections.Generic;
using System.IO;

namespace HotLib.IO
{
    /// <summary>
    /// An file path comparer which queries the running OS to determine the comparison method.
    /// </summary>
    public class OsSensitivePathComparer : IPathComparer, IEqualityComparer<string>
    {
        /// <summary>
        /// Gets if the current operating system is case sensitive as determined by creating a
        /// file with a lowercase name in the temp folder and then checking to see if it can be
        /// found when its path is converted to uppercase.
        /// </summary>
        public static bool IsOsCaseSensitive { get; } = GetOsFileSystemCaseSensitivity();
        
        /// <summary>
        /// Gets a singleton instance of <see cref="OsSensitivePathComparer"/>.
        /// </summary>
        public static OsSensitivePathComparer Instance { get; } = new OsSensitivePathComparer();

        /// <summary>
        /// Instantiates a new <see cref="OsSensitivePathComparer"/>.
        /// </summary>
        protected OsSensitivePathComparer()
        { }

        /// <summary>
        /// Tests equality between the two paths, given the OS's path case sensitivity.
        /// </summary>
        /// <param name="pathA">The first path.</param>
        /// <param name="pathB">The second path.</param>
        /// <returns>True if equal, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pathA"/> or <paramref name="pathB"/> is null.</exception>
        public virtual bool Equals(string pathA, string pathB)
        {
            if (pathA == null)
                throw new ArgumentNullException(nameof(pathA));
            if (pathB == null)
                throw new ArgumentNullException(nameof(pathB));

            return string.Equals(pathA, pathB, IsOsCaseSensitive ?
                                               StringComparison.CurrentCulture :
                                               StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Gets if the current operating system's file system is case sensitive by creating a
        /// file with a lowercase name in the temp folder and then checking to see if it can be
        /// found when its path is converted to uppercase.
        /// </summary>
        /// <returns>True if case sensitive, false if not.</returns>
        protected static bool GetOsFileSystemCaseSensitivity()
        {
            string dir;
            do
            {
                dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            }
            while (Directory.Exists(dir));

            var tempFilePath = Path.Combine(dir, "abc");

            Directory.CreateDirectory(dir);
            File.Create(tempFilePath).Dispose(); // Immediately dispose so it can be deleted

            var caseSensitive = !File.Exists(tempFilePath.ToUpper());

            Directory.Delete(dir, true);

            return caseSensitive;
        }

        /// <summary>
        /// Gets a hash code for the given path.
        /// </summary>
        /// <param name="path">The path to hash.</param>
        /// <returns>The hash code.</returns>
        public virtual int GetHashCode(string path) => EqualityComparer<string>.Default.GetHashCode(path);
    }
}
