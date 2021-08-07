using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public partial class DelegateBuilder
    {
        public class ArgumentsBuilderFromValues : ArgumentsBuilder<object?>
        {
            public ArgumentsBuilderFromValues(MethodInfo method, object?[] values)
                : base(method, values)
            { }

            protected override Expression BuildExpressionFromSource(object? source) =>
                Expression.Constant(source);

            protected override Exception GetIncompatibleTypeException(object? source, ParameterInfo parameter) =>
                new DotNetExtensions.MethodInfoExtensions.IncompatibleArgumentTypeException(Method, parameter, source);
        }
    }
}
