using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class DelegateArgumentsBuilderFromParameters : DelegateArgumentsBuilder<ParameterExpression>
    {
        public DelegateArgumentsBuilderFromParameters(MethodInfo method, ParameterExpression[] parameterExpressions)
            : base(method, parameterExpressions)
        { }

        protected override Expression BuildExpressionFromSource(ParameterExpression source) =>
            source;

        protected override Exception GetIncompatibleTypeException(ParameterExpression source, ParameterInfo parameter) =>
            new IncompatibleParameterTypeException(Method, parameter, source.Type);
    }
}
