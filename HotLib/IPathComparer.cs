using System.Collections.Generic;

namespace HotLib
{
    /// <summary>
    /// Defines public members for types used to compare file and directory paths.
    /// This interface exists so that it can be registered as a service during loading and saving.
    /// </summary>
    public interface IPathComparer : IEqualityComparer<string>
    { }
}