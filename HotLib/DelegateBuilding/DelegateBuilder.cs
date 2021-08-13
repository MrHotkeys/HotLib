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

        protected Type ReturnType { get; set; } = typeof(void);

        public DelegateBuilder(MethodInfo method)
        {
            Method = method;
        }

        /// <summary>
        /// Sets the built delegate to use the given object as the instance the encapsulated method is invoked on.
        /// For static methods, null should be used, which is also the default value, so this call can also be skipped.
        /// </summary>
        /// <param name="o">The instance to invoke the method on, or null if the method is static.</param>
        /// <returns>This builder, for chaining.</returns>
        public DelegateBuilder UseInstanceValue(object? o)
        {
            InstanceExpression = o is null ? null : Expression.Constant(o);

            return this;
        }

        /// <summary>
        /// Sets the built delegate to use the value from the first parameter to the delegate as the instance
        /// the encapsulated method is invoked on. Must not be called for static methods.
        /// </summary>
        /// <param name="type">The type of instance the parameter will be created to accept.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public DelegateBuilder UseInstanceParameter(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            InstanceExpression = Expression.Parameter(type, "instance");

            return this;
        }

        /// <summary>
        /// Sets the built delegate to use the values passed as arguments to its call as arguments to the encapsulated method.
        /// The given array of types will dictate the type of each parameter, as well as order.
        /// </summary>
        /// <remarks>If the delegate is set to use parameter for its instance as well, that will come first, so every parameter
        ///     that corresponds to a type in this array will be offset by 1.</remarks>
        /// <param name="types">The types the parameters will be set for.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/> is null.</exception>
        public DelegateBuilder UseArgumentsFromParameters(params Type[] types)
        {
            if (types is null)
                throw new ArgumentNullException(nameof(types));

            NonInstanceParameterExpressions = types
                .SelectWithIndex(t => Expression.Parameter(t.Item, $"arg{t.Index}"))
                .ToArray();
            Arguments = new DelegateArgumentsBuilderFromParameters(Method, NonInstanceParameterExpressions);

            return this;
        }

        /// <summary>
        /// Sets the built delegate to use the values from the given array as the arguments when calling the encapsulated method.
        /// The same number of values must be given as parameters on the encapsulated method.
        /// </summary>
        /// <param name="args">The array of values to pass as arguments.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        public DelegateBuilder UseArgumentsFromValues(params object?[] args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            NonInstanceParameterExpressions = Array.Empty<ParameterExpression>();
            Arguments = new DelegateArgumentsBuilderFromValues(Method, args);

            return this;
        }

        /// <summary>
        /// Sets the built delegate to return a value of the given type.
        /// For void, <see langword="typeof"/>(<see langword="void"/>) can be passed, which is also the default value, so this call can also be skipped.
        /// </summary>
        /// <param name="type">The return type of the delegate.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is null.</exception>
        public DelegateBuilder UseReturnType(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            ReturnType = type;

            return this;
        }

        public TDelegate Build<TDelegate>()
            where TDelegate : Delegate
        {
            EnsureInstanceValid();

            var parameterExpressions =
                InstanceExpression is ParameterExpression instanceParameterExpr ?
                NonInstanceParameterExpressions.Prepend(instanceParameterExpr):
                NonInstanceParameterExpressions;

            var argumentExpressions = Arguments.Build();
            EnsureArgumentsValid(argumentExpressions);

            Expression body = Expression.Call(
                instance: InstanceExpression,
                method: Method,
                arguments: argumentExpressions);

            if (ReturnType != typeof(void))
                EnsureReturnCompatible(ref body);

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

        protected void EnsureArgumentsValid(Expression[] argumentExprs)
        {
            var parameters = Method.GetParameters();

            if (argumentExprs.Length != parameters.Length)
                throw new ArgumentCountMismatchException(Method, parameters.Length, argumentExprs.Length);
        }

        protected void EnsureReturnCompatible(ref Expression body)
        {
            if (body.Type == ReturnType)
                return;

            if (body.Type == typeof(void) || !body.Type.CanCastTo(ReturnType))
                throw new IncompatibleReturnTypeException(Method, ReturnType, body.Type);

            body = Expression.Convert(body, ReturnType);
        }
    }
}
