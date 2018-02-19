using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static class AppDomainExtensions
    {
        public class AssemblyNameComparer : IEqualityComparer<AssemblyName>
        {
            public static AssemblyNameComparer Instance { get; } = new AssemblyNameComparer();

            private AssemblyNameComparer()
            { }

            public bool Equals(AssemblyName x, AssemblyName y) => string.Equals(x.FullName, y.FullName,
                                                                                StringComparison.Ordinal);

            public int GetHashCode(AssemblyName obj) => obj.FullName.GetHashCode();
        }

        public static IEnumerable<Assembly> GetAssembliesAndReferenced(this AppDomain appDomain)
        {
            var queue = new Queue<Assembly>(appDomain.GetAssemblies());
            var foundAssemblyFullNames = new HashSet<string>(queue.Select(a => a.FullName));

            while (queue.Count > 0)
            {
                var assembly = queue.Dequeue();

                yield return assembly;

                var newReferenced = assembly.GetReferencedAssemblies()
                                            .Where(an => !foundAssemblyFullNames.Contains(an.FullName));

                foreach (var name in newReferenced)
                {
                    foundAssemblyFullNames.Add(name.FullName);
                    queue.Enqueue(Assembly.Load(name));
                }
            }

            //var assemblies = appDomain.GetAssemblies();
            
            //return assemblies.SelectMany(a => a.GetReferencedAssemblies())
            //                 .Distinct(AssemblyNameComparer.Instance)
            //                 .Where(an => !assemblies.Any(a => an.IsNameFor(a)))
            //                 .Select(an => Assembly.Load(an)) 
            //                 .Concat(assemblies);
        }
    }
}
