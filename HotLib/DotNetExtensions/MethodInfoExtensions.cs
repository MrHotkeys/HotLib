using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static class MethodInfoExtensions
    {
        public static bool IsOverrideOf(this MethodInfo current, MethodInfo method)
        {
            if (current == null)
                throw new ArgumentNullException(nameof(current));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (!StringComparer.Ordinal.Equals(current.Name, method.Name))
                return false;

            return current.EnumerateBaseDefinitions()
                          .Contains(method);
        }

        public static IEnumerable<MethodInfo> EnumerateBaseDefinitions(this MethodInfo method)
        {
            var current = method;
            var next = current.GetBaseDefinition();
            while (next != current)
            {
                current = next;
                yield return current;
                next = current.GetBaseDefinition();
            }
        }
    }
}
