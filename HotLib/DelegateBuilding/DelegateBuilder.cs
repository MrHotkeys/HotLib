using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib.DelegateBuilding
{
    public class DelegateBuilder
    {
        public MethodInfo Method { get; }

        protected Expression? InstanceExpression { get; set; }

        protected ParameterExpression[] NonInstanceParameterExpressions { get; set; } = Array.Empty<ParameterExpression>();

        protected IDelegateArgumentsBuilder Arguments { get; set; } = DelegateArgumentsBuilderFromNone.Instance;

        protected Type? ReturnType { get; set; }

        protected DelegateBuilderTypeCheck ArgumentTypeCheck { get; set; } = DelegateBuilderTypeCheck.AtCompileTime;

        public DelegateBuilder(MethodInfo method)
        {
            Method = method;
        }

        /// <summary>
        /// Builds a delegate with the given object as the instance the method is invoked on. Required for instance methods, but 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public DelegateBuilder UseInstanceValue(object? o)
        {
            InstanceExpression = o is null ? null : Expression.Constant(o);

            return this;
        }

        public DelegateBuilder UseInstanceParameter(Type type)
        {
            InstanceExpression = Expression.Parameter(type, "instance");

            return this;
        }

        public DelegateBuilder UseArgumentsFromParameters(params Type[] types)
        {
            NonInstanceParameterExpressions = types
                .SelectWithIndex(t => Expression.Parameter(t.Item, $"arg{t.Index}"))
                .ToArray();
            Arguments = new DelegateArgumentsBuilderFromParameters(Method, NonInstanceParameterExpressions);

            return this;
        }

        public DelegateBuilder UseArgumentsFromValues(params object?[] args)
        {
            NonInstanceParameterExpressions = Array.Empty<ParameterExpression>();
            Arguments = new DelegateArgumentsBuilderFromValues(Method, args);

            return this;
        }

        public DelegateBuilder UseReturnType(Type type)
        {
            ReturnType = type;

            return this;
        }

        public DelegateBuilder CheckArguments(DelegateBuilderTypeCheck check)
        {
            ArgumentTypeCheck = check;

            return this;
        }

        public TDelegate Build<TDelegate>()
            where TDelegate : Delegate
        {
            EnsureInstanceValid();

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
                    throw new IncompatibleReturnTypeException(Method, ReturnType, body.Type);
            }

            return Expression
                .Lambda<TDelegate>(body, parameterExpressions)
                .Compile();
        }

        protected void EnsureInstanceValid()
        {
            if (Method.IsStatic)
            {
                if (InstanceExpression is not null)
                    throw IncompatibleInstanceException.ForNonNullWithStatic(Method, InstanceExpression.Type);
            }
            else // Non-static
            {
                if (InstanceExpression is null)
                    throw IncompatibleInstanceException.ForNullWithNonStatic(Method);

                if (Method.DeclaringType is null)
                    throw new NotSupportedException();

                if (!Method.DeclaringType.IsAssignableFrom(InstanceExpression.Type))
                    throw IncompatibleInstanceException.ForIncompatibleType(Method, InstanceExpression.Type);
            }
        }
    }
}
