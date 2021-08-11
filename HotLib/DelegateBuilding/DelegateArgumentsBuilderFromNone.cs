using System;
using System.Linq.Expressions;

namespace HotLib.DelegateBuilding
{
    public sealed class DelegateArgumentsBuilderFromNone : IDelegateArgumentsBuilder
    {
        public static readonly DelegateArgumentsBuilderFromNone Instance = new DelegateArgumentsBuilderFromNone();

        private DelegateArgumentsBuilderFromNone()
        { }

        public Expression[] Build() =>
            Array.Empty<Expression>();
    }
}
