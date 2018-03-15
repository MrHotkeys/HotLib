using System;
using System.Collections.Generic;
using System.Reflection;

namespace HotLib
{
    public class AssemblyNameComparer : IEqualityComparer<AssemblyName>
    {
        public static AssemblyNameComparer Instance { get; } = new AssemblyNameComparer();

        private AssemblyNameComparer()
        { }
        
        public static bool Equals(AssemblyName x, AssemblyName y) => string.Equals(x.FullName, y.FullName,
                                                                                   StringComparison.Ordinal);

        bool IEqualityComparer<AssemblyName>.Equals(AssemblyName x, AssemblyName y) => Equals(x, y);

        public int GetHashCode(AssemblyName obj) => obj.FullName.GetHashCode();
    }
}
