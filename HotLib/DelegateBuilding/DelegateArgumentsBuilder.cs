using System;
using System.Linq.Expressions;
using System.Reflection;

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

        public Expression[] Build(DelegateBuilderTypeCheck typeCheckOptions)
        {
            var parameters = Method.GetParameters();

            if (parameters.Length != Sources.Length)
                throw new DotNetExtensions.MethodInfoExtensions.ArgumentCountMismatchException(Method, parameters.Length, Sources.Length);

            var argExprs = new Expression[Sources.Length];

            for (var i = 0; i < Sources.Length; i++)
            {
                var source = Sources[i];
                var sourceExpr = BuildExpressionFromSource(source);
                var parameter = parameters[i];

                argExprs[i] =
                    sourceExpr.Type == parameter.ParameterType ?
                    sourceExpr :
                    BuildConvertExpression(source, sourceExpr, parameter, typeCheckOptions);
            }

            return argExprs;
        }

        protected abstract Expression BuildExpressionFromSource(TSource source);

        protected Expression BuildConvertExpression(TSource source, Expression sourceExpr, ParameterInfo parameter, DelegateBuilderTypeCheck typeCheckOptions)
        {
            switch (typeCheckOptions)
            {
                case DelegateBuilderTypeCheck.AtCompileTime:
                    {
                        return parameter.ParameterType.IsAssignableFrom(sourceExpr.Type) ?
                            Expression.Convert(sourceExpr, parameter.ParameterType) :
                            throw GetIncompatibleTypeException(source, parameter);
                    }
                case DelegateBuilderTypeCheck.AtCallTime:
                    {
                        var exceptionLocalExpr = Expression.Parameter(typeof(InvalidCastException), "e");
                        return Expression
                            .TryCatch(
                                Expression.Convert(sourceExpr, parameter.ParameterType),
                                Expression.MakeCatchBlock(
                                    type: typeof(InvalidCastException),
                                    variable: exceptionLocalExpr,
                                    body: ExpressionHelpers.Throw(
                                        exceptionExpr: (object o, InvalidCastException e) =>
                                            new DotNetExtensions.MethodInfoExtensions.IncompatibleArgumentTypeException(Method, parameter, o, e),
                                        expr1: sourceExpr,
                                        expr2: exceptionLocalExpr,
                                        type: parameter.ParameterType),
                                    filter: null));
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        protected abstract Exception GetIncompatibleTypeException(TSource source, ParameterInfo parameter);
    }
}
