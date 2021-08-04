using System;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        /// <summary>
        /// Builds this method into an <see cref="Func{TResult}"/> that invokes the method using the given target object and arguments for each call.
        /// </summary>
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on, or null if the method is static.</param>
        /// <param name="args">The arguments to supply to the method call each time the Func is invoked.
        ///     <br/><br/><b>If passing a single <see langword="null"/>, it will need cast as an <see cref="object"/>
        ///     or else it will get implicity cast into a null array!</b></param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is null and <paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<TResult> BuildFuncWithSetCall<TResult>(this MethodInfo method, object? instance, params object?[] args) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            args is null ? throw new ArgumentNullException(nameof(args)) :
            !method.IsStatic && instance is null ? throw new ArgumentException("Must provide an instance for non-static methods!", nameof(instance)) :
            BuildDelegateWithSetCall<Func<TResult>, TResult>(method, instance, args);

        /// <summary>
        /// Builds this method into an <see cref="Func{TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{TResult}(MethodInfo)"/>.</remarks>
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<TResult> BuildFuncWithSetInstance<TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<TResult>>(method, instance, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Func{T, TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{T, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic{T, TResult}(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<T, TResult> BuildFuncWithSetInstance<T, TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<T, TResult>>(method, instance, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{T1, T2, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic{T1, T2, TResult}(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<T1, T2, TResult> BuildFuncWithSetInstance<T1, T2, TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<T1, T2, TResult>>(method, instance, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{T1, T2, T3, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, TResult}(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<T1, T2, T3, TResult> BuildFuncWithSetInstance<T1, T2, T3, TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<T1, T2, T3, TResult>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{T1, T2, T3, T4, TResult}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, T4, TResult}(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<T1, T2, T3, T4, TResult> BuildFuncWithSetInstance<T1, T2, T3, T4, TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<T1, T2, T3, T4, TResult>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildFuncStatic{T1, T2, T3, T4, T5, TResult}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, T4, T5, TResult}(MethodInfo)"/>.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Func<T1, T2, T3, T4, T5, TResult> BuildFuncWithSetInstance<T1, T2, T3, T4, T5, TResult>(this MethodInfo method, object instance) =>
            BuildFuncWithSetInstance<Func<T1, T2, T3, T4, T5, TResult>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TFunc BuildFuncWithSetInstance<TFunc>(MethodInfo method, object instance, Type[] paramTypes)
            where TFunc : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(BuildFuncStatic)} for static methods)!", nameof(method)) :
            BuildDelegateWithSetInstance<TFunc>(method, instance, paramTypes);

        /// <summary>
        /// Builds this method into an <see cref="Func{T}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{TReturn}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, TResult> BuildFunc<TInstance, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, TResult>>(method, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{T, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T, TResult> BuildFunc<TInstance, T, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, T, TResult>>(method, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{T1, T2, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, TResult> BuildFunc<TInstance, T1, T2, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, T1, T2, TResult>>(method, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, TResult}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, TResult> BuildFunc<TInstance, T1, T2, T3, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, T1, T2, T3, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, T4, TResult}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, TResult> BuildFunc<TInstance, T1, T2, T3, T4, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, T1, T2, T3, T4, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5, T6}"/> that invokes the method using the argument
        /// to the first parameter of the Func as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildFuncStatic{T1, T2, T3, T4, T5, TResult}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodFunc<TInstance, T1, T2, T3, T4, T5, TResult> BuildFunc<TInstance, T1, T2, T3, T4, T5, TResult>(this MethodInfo method) =>
            BuildFunc<TInstance, InstanceMethodFunc<TInstance, T1, T2, T3, T4, T5, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TFunc BuildFunc<TInstance, TFunc>(MethodInfo method, Type[] paramTypes)
            where TFunc : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(BuildFuncStatic)} for static methods)!", nameof(method)) :
            BuildDelegate<TFunc>(method, typeof(TInstance), paramTypes);

        /// <summary>
        /// Builds this method into an <see cref="Func"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance}(MethodInfo)"/>.</remarks>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<TReturn> BuildFuncStatic<TReturn>(this MethodInfo method) =>
            BuildFuncStatic<Func<TReturn>>(method, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Func{T}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance, T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T, TResult> BuildFuncStatic<T, TResult>(this MethodInfo method) =>
            BuildFuncStatic<Func<T, TResult>>(method, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, TResult> BuildFuncStatic<T1, T2, TResult>(this MethodInfo method) =>
            BuildFuncStatic<Func<T1, T2, TResult>>(method, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="TResult">The type of result returned by the method call. The actual type
        ///     returned will be attempted to be converted to this value if it is not the same type.</typeparam>
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, TResult> BuildFuncStatic<T1, T2, T3, TResult>(this MethodInfo method) =>
            BuildFuncStatic<Func<T1, T2, T3, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, TResult> BuildFuncStatic<T1, T2, T3, T4, TResult>(this MethodInfo method) =>
            BuildFuncStatic<Func<T1, T2, T3, T4, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Func{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildFunc{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an Func.</param>
        /// <returns>The resulting Func.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Func<T1, T2, T3, T4, T5, TResult> BuildFuncStatic<T1, T2, T3, T4, T5, TResult>(this MethodInfo method) =>
            BuildFuncStatic<Func<T1, T2, T3, T4, T5, TResult>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        public static TFunc BuildFuncStatic<TFunc>(MethodInfo method, Type[] parameterTypes)
            where TFunc : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method must be static (see {nameof(BuildFunc)} for non-static methods)!", nameof(method)) :
            BuildDelegateStatic<TFunc>(method, parameterTypes);
    }
}
