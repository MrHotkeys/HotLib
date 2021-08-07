using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        public class ArgumentCountMismatchException : Exception
        {
            public MethodInfo Method { get; }
            public int Expected { get; }
            public int Actual { get; }

            public ArgumentCountMismatchException(MethodInfo method, int expected, int actual)
                : this(method, expected, actual, null)
            { }

            public ArgumentCountMismatchException(MethodInfo method, int expected, int actual, Exception? innerException)
                : base($"Incorrect number of parameters given for method {method} (expected {expected}, got {actual})!", innerException)
            {
                Method = method;
                Expected = expected;
                Actual = actual;
            }
        }

        public class IncompatibleArgumentTypeException : Exception
        {
            public MethodInfo Method { get; }
            public ParameterInfo Parameter { get; }
            public object? Argument { get; }

            public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument)
                : this(method, parameter, argument, null)
            { }

            public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument, Exception? innerException)
                : base($"Unable to cast object {argument ?? "NULL"} of type {argument?.GetType()?.ToString() ?? "NULL"} to " +
                      $"{parameter.ParameterType} for parameter {parameter.Name} at position {parameter.Position} in method {method}!",
                      innerException)
            {
                Method = method;
                Parameter = parameter;
                Argument = argument;
            }
        }

        public class IncompatibleParameterTypeException : Exception
        {
            public MethodInfo Method { get; }
            public ParameterInfo ParameterActual { get; }
            public Type ParameterTypeGiven { get; }

            public IncompatibleParameterTypeException(MethodInfo method, ParameterInfo parameterActual, Type parameterTypeGiven)
                : this(method, parameterActual, parameterTypeGiven, null)
            { }

            public IncompatibleParameterTypeException(MethodInfo method, ParameterInfo parameterActual, Type parameterTypeGiven, Exception? innerException)
                : base($"Unable to cast parameter of type {parameterTypeGiven} to " +
                      $"{parameterActual.ParameterType} for parameter {parameterActual.Name} at position {parameterActual.Position} in method {method}!", innerException)
            {
                Method = method;
                ParameterActual = parameterActual;
                ParameterTypeGiven = parameterTypeGiven;
            }
        }

        public class IncompatibleReturnTypeException : Exception
        {
            public MethodInfo Method { get; }
            public Type Expected { get; }
            public Type Actual { get; }

            public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual)
                : this(method, expected, actual, null)
            { }

            public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual, Exception? innerException)
                : base($"Unable to cast return type {actual} to type {expected} in method {method}!")
            {
                Method = method;
                Expected = expected;
                Actual = actual;
            }
        }

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the given target object and arguments for each call and returns void.
        /// </summary>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <param name="method">The method to build into a delegate.</param>
        /// <param name="instance">The target object to invoke the method on, or null if the method is static.</param>
        /// <param name="args">The arguments to supply to the method call each time the action is invoked.
        ///     <br/><br/><b>If passing a single <see langword="null"/>, it will need cast as an <see cref="object"/>
        ///     or else it will get implicity cast into a null array!</b></param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is null and <paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static TDelegate BuildDelegateWithSetCall<TDelegate>(this MethodInfo method, object? instance, params object?[] args)
            where TDelegate : Delegate =>
            BuildDelegateWithSetCall<TDelegate>(method, instance, args, null);

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the given target object and arguments for
        /// each call and returns a value.
        /// </summary>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a delegate.</param>
        /// <param name="instance">The target object to invoke the method on, or null if the method is static.</param>
        /// <param name="args">The arguments to supply to the method call each time the action is invoked.
        ///     <br/><br/><b>If passing a single <see langword="null"/>, it will need cast as an <see cref="object"/>
        ///     or else it will get implicity cast into a null array!</b></param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the number of
        ///     parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be cast to be compatible
        ///     with the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is null and <paramref name="method"/> is non-static
        ///     -or-<paramref name="method"/> returns void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static TDelegate BuildDelegateWithSetCall<TDelegate, TResult>(this MethodInfo method, object? instance, params object?[] args)
            where TDelegate : Delegate =>
            BuildDelegateWithSetCall<TDelegate>(method, instance, args, typeof(TResult));

        private static TDelegate BuildDelegateWithSetCall<TDelegate>(MethodInfo method, object? instance, object?[] args, Type? returnType)
            where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (args is null)
                throw new ArgumentNullException(nameof(args));
            if (!method.IsStatic && instance is null)
                throw new ArgumentException("Must provide an instance for non-static methods!", nameof(instance));
            if (returnType is not null && method.ReturnType == typeof(void))
                throw new ArgumentException("Cannot build a delegate with a return value for methods that return void!");

            var instanceExpr = instance is null ? null : Expression.Constant(instance);

            return BuildDelegate<TDelegate>(
                method: method,
                instanceExpr: instanceExpr,
                argExprs: BuildArgumentExpressions(method, args),
                paramExprs: Enumerable.Empty<ParameterExpression>(),
                returnType: returnType);
        }

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the given target object and returns void.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildDelegateStatic{TDelegate}(MethodInfo, Type[])"/>.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegateWithSetInstance<TDelegate>(this MethodInfo method, object? instance, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegateWithSetInstance<TDelegate>(method, instance, parameterTypes, null);

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the given target object and returns a value.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildDelegateStatic{TDelegate, TReturn}(MethodInfo, Type[])"/>.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be cast to be compatible
        ///     with the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static or returns void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegateWithSetInstance<TDelegate, TResult>(this MethodInfo method, object? instance, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegateWithSetInstance<TDelegate>(method, instance, parameterTypes, typeof(TResult));

        private static TDelegate BuildDelegateWithSetInstance<TDelegate>(MethodInfo method, object? instance, Type[] parameterTypes, Type? returnType)
            where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));
            if (method.IsStatic)
                throw new ArgumentException($"Given method may not be static (see {nameof(BuildDelegateStatic)} for static methods)!", nameof(method));
            if (returnType is not null && method.ReturnType == typeof(void))
                throw new ArgumentException("Cannot build a delegate with a return value for methods that return void!");

            var instanceExpr = instance is null ? null : Expression.Constant(instance);
            var paramExprs = parameterTypes.Select(pt => Expression.Parameter(pt)).ToArray();
            var argExprs = BuildArgumentExpressions(method, paramExprs.AsSpan());

            return BuildDelegate<TDelegate>(
                method: method,
                instanceExpr: instanceExpr,
                argExprs: argExprs,
                paramExprs: paramExprs,
                returnType: returnType);
        }

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the argument to the first
        /// parameter of the action as the target object and returns void.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildDelegateStatic{TDelegate}(MethodInfo, Type[])"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instanceType">The type of object to invoke the encapsulated method on.
        ///     Will determine the type of the first parameter to the delegate.</param>
        /// <param name="parameterTypes">The types of all parameters after the first parameter, in order.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/>, <paramref name="instanceType"/>,
        ///     or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegate<TDelegate>(this MethodInfo method, Type instanceType, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegate<TDelegate>(method, instanceType, parameterTypes, null);

        /// <summary>
        /// Builds this method into a delegate that invokes the method using the argument to the first
        /// parameter of the action as the target object and returns a value.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildDelegateStatic{TDelegate}(MethodInfo, Type[])"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instanceType">The type of object to invoke the encapsulated method on.
        ///     Will determine the type of the first parameter to the delegate.</param>
        /// <param name="parameterTypes">The types of all parameters after the first parameter, in order.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be cast to be compatible
        ///     with the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static or returns void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/>, <paramref name="instanceType"/>,
        ///     or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegate<TDelegate, TResult>(this MethodInfo method, Type instanceType, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegate<TDelegate>(method, instanceType, parameterTypes, typeof(TResult));

        private static TDelegate BuildDelegate<TDelegate>(MethodInfo method, Type instanceType, Type[] parameterTypes, Type? returnType)
            where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (instanceType is null)
                throw new ArgumentNullException(nameof(instanceType));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));
            if (method.IsStatic)
                throw new ArgumentException($"Given method may not be static (see {nameof(BuildDelegateStatic)} for static methods)!", nameof(method));
            if (returnType is not null && method.ReturnType == typeof(void))
                throw new ArgumentException("Cannot build a delegate with a return value for methods that return void!");

            var instanceExpr = Expression.Parameter(instanceType, "instance");
            var paramExprs = Enumerable
                .Repeat(instanceExpr, 1)
                .Concat(parameterTypes.SelectWithIndex(t => Expression.Parameter(t.Item, $"arg{t.Index}")))
                .ToArray();
            var argExprs = BuildArgumentExpressions(method, paramExprs.AsSpan(1));

            return BuildDelegate<TDelegate>(
                method: method,
                instanceExpr: instanceExpr,
                argExprs: argExprs,
                paramExprs: paramExprs,
                returnType: returnType);
        }

        /// <summary>
        /// Builds this method into a delegate that invokes the method statically and returns void.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildDelegate{TDelegate}(MethodInfo, Type, Type[])"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegateStatic<TDelegate>(this MethodInfo method, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegateStatic<TDelegate>(method, parameterTypes, null);

        /// <summary>
        /// Builds this method into a delegate that invokes the method statically and returns a value.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildDelegate{TDelegate}(MethodInfo, Type, Type[])"/>.</remarks>
        /// <typeparam name="TDelegate">The type of <see cref="Delegate"/> to build the method into.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleParameterTypeException">A given parameter type cannot be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be cast to be compatible
        ///     with the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static or returns void.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="parameterTypes"/> is null.</exception>
        public static TDelegate BuildDelegateStatic<TDelegate, TResult>(this MethodInfo method, params Type[] parameterTypes)
            where TDelegate : Delegate =>
            BuildDelegateStatic<TDelegate>(method, parameterTypes, typeof(TResult));

        private static TDelegate BuildDelegateStatic<TDelegate>(MethodInfo method, Type[] parameterTypes, Type? returnType)
            where TDelegate : Delegate
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (parameterTypes is null)
                throw new ArgumentNullException(nameof(parameterTypes));
            if (!method.IsStatic)
                throw new ArgumentException($"Given method must be static (see {nameof(BuildDelegate)} for non-static methods)!", nameof(method));
            if (returnType is not null && method.ReturnType == typeof(void))
                throw new ArgumentException("Cannot build a delegate with a return value for methods that return void!");

            var paramExprs = parameterTypes.Select(pt => Expression.Parameter(pt)).ToArray();
            var argExprs = BuildArgumentExpressions(method, paramExprs.AsSpan());

            return BuildDelegate<TDelegate>(
                method: method,
                instanceExpr: null,
                argExprs: argExprs,
                paramExprs: paramExprs,
                returnType: returnType);
        }

        private static TDelegate BuildDelegate<TDelegate>(MethodInfo method, Expression? instanceExpr,
            IEnumerable<Expression> argExprs, IEnumerable<ParameterExpression> paramExprs, Type? returnType)
            where TDelegate : Delegate
        {
            Expression body = Expression.Call(
                instance: instanceExpr,
                method: method,
                arguments: argExprs);

            if (returnType is not null && body.Type != returnType)
            {
                if (returnType.IsAssignableFrom(body.Type))
                    body = Expression.Convert(body, returnType);
                else
                    throw new IncompatibleReturnTypeException(method, returnType, body.Type);
            }

            return Expression
                .Lambda<TDelegate>(body, paramExprs)
                .Compile();
        }

        private static Expression[] BuildArgumentExpressions(MethodInfo method, Span<ParameterExpression> paramExprs)
        {
            var parameters = method.GetParameters();

            if (parameters.Length != paramExprs.Length)
                throw new ArgumentCountMismatchException(method, parameters.Length, paramExprs.Length);

            var argExprs = new Expression[paramExprs.Length];

            for (var i = 0; i < paramExprs.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var current = paramExprs[i];

                argExprs[i] =
                    current.Type == parameterType ? current :
                    parameterType.IsAssignableFrom(current.Type) ? Expression.Convert(current, parameterType) :
                    throw new IncompatibleParameterTypeException(method, parameters[i], current.Type);
            }

            return argExprs;
        }

        private static Expression[] BuildArgumentExpressions(MethodInfo method, object?[] args)
        {
            var parameters = method.GetParameters();

            if (parameters.Length != args.Length)
                throw new ArgumentCountMismatchException(method, parameters.Length, args.Length);

            var argExprs = new Expression[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var current = args[i];
                var currentExpr = Expression.Constant(args[i]);

                argExprs[i] =
                    currentExpr.Type == parameterType ? currentExpr :
                    parameterType.IsAssignableFrom(currentExpr.Type) ? Expression.Convert(currentExpr, parameterType) :
                    throw new IncompatibleArgumentTypeException(method, parameters[i], current);
            }

            return argExprs;
        }
    }
}
