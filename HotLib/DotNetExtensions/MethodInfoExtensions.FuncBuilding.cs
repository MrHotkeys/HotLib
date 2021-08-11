using System;
using System.Reflection;

using HotLib.DelegateBuilding;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        /// <summary>
        /// Builds this method into an <see cref="Func{TResult}"/> that invokes the method, configured with the
        /// given <paramref name="builderSetup"/> to set the instance, arguments, and other aspects.
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static Func<TResult> CreateFunc<TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            CreateDelegate<Func<TResult>>(method, b =>
            {
                b.UseReturnType(typeof(TResult));

                builderSetup?.Invoke(b);
            });

        /// <summary>
        /// Builds this method into an <see cref="Func{T, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T, TResult> CreateFunc<T, TResult>(this MethodInfo method) =>
            CreateFunc<T, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Func{T, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T, TResult> CreateFunc<T, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFunc<Func<T, TResult>>(method, builderSetup, TypeHelpers.TypeArray<T>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, TResult> CreateFunc<T1, T2, TResult>(this MethodInfo method) =>
            CreateFunc<T1, T2, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, TResult> CreateFunc<T1, T2, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFunc<Func<T1, T2, TResult>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, TResult> CreateFunc<T1, T2, T3, TResult>(this MethodInfo method) =>
            CreateFunc<T1, T2, T3, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, TResult> CreateFunc<T1, T2, T3, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFunc<Func<T1, T2, T3, TResult>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, TResult> CreateFunc<T1, T2, T3, T4, TResult>(this MethodInfo method) =>
            CreateFunc<T1, T2, T3, T4, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, TResult> CreateFunc<T1, T2, T3, T4, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFunc<Func<T1, T2, T3, T4, TResult>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T5">The type of the fifth argument passed to the method call.
        ///     Will attempt to convert to the method's fifth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, T5, TResult> CreateFunc<T1, T2, T3, T4, T5, TResult>(this MethodInfo method) =>
            CreateFunc<T1, T2, T3, T4, T5, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> that invokes the method.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T5">The type of the fifth argument passed to the method call.
        ///     Will attempt to convert to the method's fifth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, T5, TResult> CreateFunc<T1, T2, T3, T4, T5, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFunc<Func<T1, T2, T3, T4, T5, TResult>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>(), typeof(TResult));

        private static TDelegate CreateFunc<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type[] parameterTypes, Type returnType)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseArgumentsFromParameters(parameterTypes);
                b.UseReturnType(returnType);

                builderSetup(b);
            });

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{TResult}(MethodInfo, Action{DelegateBuilder})"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, TResult> CreateFuncInstanced<TInstance, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{TResult}(MethodInfo, Action{DelegateBuilder})"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, TResult> CreateFuncInstanced<TInstance, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, TResult>>(method, builderSetup, typeof(TInstance), Type.EmptyTypes, typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T, TResult> CreateFuncInstanced<TInstance, T, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, T, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T, TResult> CreateFuncInstanced<TInstance, T, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, T, TResult>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, TResult> CreateFuncInstanced<TInstance, T1, T2, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, T1, T2, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, TResult> CreateFuncInstanced<TInstance, T1, T2, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, T1, T2, TResult>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, T1, T2, T3, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, T1, T2, T3, TResult>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, T4, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, T4, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, T4, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, T1, T2, T3, T4, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, T4, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, T4, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, T4, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, T1, T2, T3, T4, TResult>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4>(), typeof(TResult));

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, T4, T5, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, T4, T5, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T5">The type of the fifth argument passed to the method call.
        ///     Will attempt to convert to the method's fifth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, T5, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, T4, T5, TResult>(this MethodInfo method) =>
            CreateFuncInstanced<TInstance, T1, T2, T3, T4, T5, TResult>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodFunc{TInstance, T1, T2, T3, T4, T5, TResult}"/> that invokes the method using the argument
        /// to the first parameter of the func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateFunc{T1, T2, T3, T4, T5, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T5">The type of the fifth argument passed to the method call.
        ///     Will attempt to convert to the method's fifth parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into a func.</param>
        /// <param name="builderSetup">An func which can be used to further configure the built delegate.</param>
        /// <returns>The resulting func.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleReturnTypeException">The return type of the method cannot be converted into the given return type.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, T5, TResult> CreateFuncInstanced<TInstance, T1, T2, T3, T4, T5, TResult>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateFuncInstanced<InstanceMethodFunc<TInstance, T1, T2, T3, T4, T5, TResult>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4, T5>(), typeof(TResult));

        private static TDelegate CreateFuncInstanced<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type instanceType, Type[] parameterTypes, Type returnType)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(CreateFunc)} for static methods)!", nameof(method)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseInstanceParameter(instanceType);
                b.UseArgumentsFromParameters(parameterTypes);
                b.UseReturnType(returnType);

                builderSetup(b);
            });
    }
}
