using System;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="AssemblyName"/>.
    /// </summary>
    public static class AssemblyNameExtensions
    {
        /// <summary>
        /// Gets if the <see cref="AssemblyName"/> is the name for the given <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assemblyName">The assembly name to check.</param>
        /// <param name="assembly">The assembly to check against.</param>
        /// <returns>True if the name belongs to the assembly, false if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyName"/>
        ///     or <paramref name="assembly"/> is null.</exception>
        public static bool IsNameFor(this AssemblyName assemblyName, Assembly assembly)
        {
            if (assemblyName == null)
                throw new ArgumentNullException(nameof(assemblyName));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            return string.Equals(assemblyName.FullName, assembly.FullName, StringComparison.Ordinal);
        }
    }
}
