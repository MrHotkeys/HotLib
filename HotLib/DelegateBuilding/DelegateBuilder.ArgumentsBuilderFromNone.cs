using System;
using System.Linq.Expressions;

namespace HotLib.DelegateBuilding
{
    public partial class DelegateBuilder
    {
        public sealed class ArgumentsBuilderFromNone : IArgumentsBuilder
        {
            public static readonly ArgumentsBuilderFromNone Instance = new ArgumentsBuilderFromNone();

            private ArgumentsBuilderFromNone()
            { }

            public Expression[] Build(TypeCheckOptions typeCheckOptions) =>
                Array.Empty<Expression>();
        }
    }
}
