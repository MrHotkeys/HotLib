using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using HotLib.DotNetExtensions;

namespace HotLib.DelegateBuilding
{
    public partial class DelegateBuilder
    {
        public MethodInfo Method { get; }

        protected Expression? InstanceExpression { get; set; }

        protected ParameterExpression[] NonInstanceParameterExpressions { get; set; } = Array.Empty<ParameterExpression>();

        protected IArgumentsBuilder Arguments { get; set; } = ArgumentsBuilderFromNone.Instance;

        protected Type? ReturnType { get; set; }

        protected TypeCheckOptions ArgumentTypeCheck { get; set; } = TypeCheckOptions.AtCompileTime;

        public DelegateBuilder(MethodInfo method)
        {
            Method = method;
        }

        public DelegateBuilder WithInstanceValue(object o)
        {
            InstanceExpression = Expression.Constant(o);

            return this;
        }

        public DelegateBuilder WithInstanceParameter(Type type)
        {
            InstanceExpression = Expression.Parameter(type, "instance");

            return this;
        }

        public DelegateBuilder WithParameters(params Type[] types)
        {
            NonInstanceParameterExpressions = types
                .SelectWithIndex(t => Expression.Parameter(t.Item, $"arg{t.Index}"))
                .ToArray();
            Arguments = new ArgumentsBuilderFromParameters(Method, NonInstanceParameterExpressions);

            return this;
        }

        public DelegateBuilder WithArguments(params object?[] args)
        {
            NonInstanceParameterExpressions = Array.Empty<ParameterExpression>();
            Arguments = new ArgumentsBuilderFromValues(Method, args);

            return this;
        }

        public DelegateBuilder WithReturnType(Type type)
        {
            ReturnType = type;

            return this;
        }

        public DelegateBuilder WithArgumentsCheckedAtCallTime()
        {
            ArgumentTypeCheck = TypeCheckOptions.AtCallTime;

            return this;
        }

        public DelegateBuilder WithArgumentsCheckedAtCompile()
        {
            ArgumentTypeCheck = TypeCheckOptions.AtCompileTime;

            return this;
        }

        public TDelegate Build<TDelegate>()
            where TDelegate : Delegate
        {
            var parameterExpressions =
                InstanceExpression is ParameterExpression instanceParameterExpr ?
                Enumerable.Repeat(instanceParameterExpr, 1).Concat(NonInstanceParameterExpressions) :
                NonInstanceParameterExpressions;

            var argumentExpressions = Arguments.Build(ArgumentTypeCheck);

            Expression body = Expression.Call(
                instance: InstanceExpression,
                method: Method,
                arguments: argumentExpressions);

            if (ReturnType is not null && body.Type != ReturnType)
            {
                if (ReturnType.IsAssignableFrom(body.Type))
                    body = Expression.Convert(body, ReturnType);
                else
                    throw new DotNetExtensions.MethodInfoExtensions.IncompatibleReturnTypeException(Method, ReturnType, body.Type);
            }

            return Expression
                .Lambda<TDelegate>(body, parameterExpressions)
                .Compile();
        }

        private Expression[] BuildArgumentExpressions(ParameterExpression[] paramExprs)
        {
            var parameters = Method.GetParameters();

            if (parameters.Length != paramExprs.Length)
                throw new DotNetExtensions.MethodInfoExtensions.ArgumentCountMismatchException(Method, parameters.Length, paramExprs.Length);

            var argExprs = new Expression[paramExprs.Length];

            for (var i = 0; i < paramExprs.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var current = paramExprs[i];

                argExprs[i] =
                    current.Type == parameterType ? current :
                    parameterType.IsAssignableFrom(current.Type) ? Expression.Convert(current, parameterType) :
                    throw new DotNetExtensions.MethodInfoExtensions.IncompatibleParameterTypeException(Method, parameters[i], current.Type);
            }

            return argExprs;
        }

        private Expression[] BuildArgumentExpressions(object?[] args)
        {
            var parameters = Method.GetParameters();

            if (parameters.Length != args.Length)
                throw new DotNetExtensions.MethodInfoExtensions.ArgumentCountMismatchException(Method, parameters.Length, args.Length);

            var argExprs = new Expression[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var current = args[i];
                var currentExpr = Expression.Constant(args[i]);

                argExprs[i] =
                    currentExpr.Type == parameterType ? currentExpr :
                    parameterType.IsAssignableFrom(currentExpr.Type) ? Expression.Convert(currentExpr, parameterType) :
                    throw new DotNetExtensions.MethodInfoExtensions.IncompatibleArgumentTypeException(Method, parameters[i], current);
            }

            return argExprs;
        }
    }
}
