using System;
using System.Linq.Expressions;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib.DelegateBuilding
{
    public abstract class DelegateArgumentsBuilder<TSource> : IDelegateArgumentsBuilder
    {
        protected MethodInfo Method { get; }

        protected TSource[] Sources { get; }

        protected DelegateArgumentsBuilder(MethodInfo method, TSource[] sources)
        {
            Method = method ?? throw new ArgumentNullException(nameof(method));
            Sources = sources ?? throw new ArgumentNullException(nameof(sources));
        }

        public Expression[] Build()
        {
            var parameters = Method.GetParameters();

            if (parameters.Length != Sources.Length)
                throw new ArgumentCountMismatchException(Method, parameters.Length, Sources.Length);

            var argExprs = new Expression[Sources.Length];

            for (var i = 0; i < Sources.Length; i++)
            {
                var source = Sources[i];
                var sourceExpr = BuildExpressionFromSource(source);
                var parameter = parameters[i];

                argExprs[i] =
                    sourceExpr.Type == parameter.ParameterType ?
                    sourceExpr :
                    BuildConvertExpression(source, sourceExpr, parameter);
            }

            return argExprs;
        }

        protected abstract Expression BuildExpressionFromSource(TSource source);

        protected Expression BuildConvertExpression(TSource source, Expression sourceExpr, ParameterInfo parameter)
        {
            var sourceType = sourceExpr.Type;
            var parameterType = parameter.ParameterType;

            if (sourceType == parameterType)
                return sourceExpr;

            if (sourceType == typeof(void) || !sourceType.CanCastTo(parameterType))
                throw GetIncompatibleTypeException(source, parameter);

            return Expression.Convert(sourceExpr, parameterType);
        }

        protected abstract Exception GetIncompatibleTypeException(TSource source, ParameterInfo parameter);
    }
}
