using System.Collections.Generic;

namespace HotLib
{
    public interface IHashCodeGenerator
    {
        int GetHashCode<T>(T target);
    }
}
