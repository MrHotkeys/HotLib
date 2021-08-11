using System.Linq.Expressions;

namespace HotLib.DelegateBuilding
{
    public interface IDelegateArgumentsBuilder
    {
        Expression[] Build();
    }
}
