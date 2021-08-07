using System.Linq.Expressions;

namespace HotLib.DelegateBuilding
{
    public partial class DelegateBuilder
    {
        public interface IArgumentsBuilder
        {
            Expression[] Build(TypeCheckOptions typeCheckOptions);
        }
    }
}
