using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class DelegateArgumentsBuilderFromValues : DelegateArgumentsBuilder<object?>
    {
        public DelegateArgumentsBuilderFromValues(MethodInfo method, object?[] values)
            : base(method, values)
        { }

        protected override Expression BuildExpressionFromSource(object? source) =>
            Expression.Constant(source);

        protected override Exception GetIncompatibleTypeException(object? source, ParameterInfo parameter) =>
            new IncompatibleArgumentTypeException(Method, parameter, source);
    }
}
