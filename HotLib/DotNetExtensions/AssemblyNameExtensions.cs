using System;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static class AssemblyNameExtensions
    {

        public static bool IsNameFor(this AssemblyName name, Assembly assembly)
        {
            return string.Equals(name.FullName, assembly.FullName, StringComparison.Ordinal);
        }
    }
}
