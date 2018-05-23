using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace HotLib.Reflection
{
    /// <summary>
    /// An <see cref="AssemblyLoadContext"/> implementation which probes the current application domain's
    /// <see cref="AppDomain.BaseDirectory"/> for assemblies. If none are found there, it relies on the base
    /// behavior of <see cref="AssemblyLoadContext.Default"/>. Stateless and thread-safe.
    /// </summary>
    public class WorkingDirectoryProbingAssemblyLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// Gets a singleton instance of <see cref="WorkingDirectoryProbingAssemblyLoadContext"/>.
        /// </summary>
        public static WorkingDirectoryProbingAssemblyLoadContext Instance { get; } = new WorkingDirectoryProbingAssemblyLoadContext();

        /// <summary>
        /// Instantiates a new <see cref="WorkingDirectoryProbingAssemblyLoadContext"/>.
        /// </summary>
        protected WorkingDirectoryProbingAssemblyLoadContext()
        { }

        /// <summary>
        /// Loads an <see cref="Assembly"/> by its <see cref="AssemblyName"/>.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to load.</param>
        /// <returns>The found <see cref="Assembly"/>.</returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName.Name + ".dll");
            if (File.Exists(path))
                return Default.LoadFromAssemblyPath(path);
            else
                return Default.LoadFromAssemblyName(assemblyName);
        }
    }
}
