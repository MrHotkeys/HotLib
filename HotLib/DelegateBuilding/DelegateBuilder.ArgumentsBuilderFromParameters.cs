using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public partial class DelegateBuilder
    {
        public class ArgumentsBuilderFromParameters : ArgumentsBuilder<ParameterExpression>
        {
            public ArgumentsBuilderFromParameters(MethodInfo method, ParameterExpression[] parameterExpressions)
                : base(method, parameterExpressions)
            { }

            protected override Expression BuildExpressionFromSource(ParameterExpression source) =>
                source;

            protected override Exception GetIncompatibleTypeException(ParameterExpression source, ParameterInfo parameter) =>
                new DotNetExtensions.MethodInfoExtensions.IncompatibleParameterTypeException(Method, parameter, source.Type);
        }
    }
}
