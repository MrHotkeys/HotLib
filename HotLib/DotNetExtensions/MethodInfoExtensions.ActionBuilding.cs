using System;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        /// <summary>
        /// Builds this method into an <see cref="Action"/> that invokes the method using the given target object and arguments for each call.
        /// </summary>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on, or null if the method is static.</param>
        /// <param name="args">The arguments to supply to the method call each time the action is invoked.
        ///     For no arguments, pass an empty array, or omit any arguments.
        ///     <br/><br/><b>If passing a single <see langword="null"/>, it will need cast as an <see cref="object"/>
        ///     or else it will get implicity cast into a null array!</b></param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="instance"/> is null and <paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="args"/> is null.</exception>
        public static Action BuildActionWithSetCall(this MethodInfo method, object? instance, params object?[] args) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            args is null ? throw new ArgumentNullException(nameof(args)) :
            !method.IsStatic && instance is null ? throw new ArgumentException("Must provide an instance for non-static methods!", nameof(instance)) :
            BuildDelegateWithSetCall<Action>(method, instance, args);

        /// <summary>
        /// Builds this method into an <see cref="Action"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action BuildActionWithSetInstance(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action>(method, instance, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic{T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic{T}(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T> BuildActionWithSetInstance<T>(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action<T>>(method, instance, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic{T1, T2}(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> BuildActionWithSetInstance<T1, T2>(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action<T1, T2>>(method, instance, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic{T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic{T1, T2, T3}(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3> BuildActionWithSetInstance<T1, T2, T3>(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action<T1, T2, T3>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic{T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic{T1, T2, T3, T4}(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4> BuildActionWithSetInstance<T1, T2, T3, T4>(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action<T1, T2, T3, T4>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method using the given target object.
        /// </summary>
        /// <remarks><b>Must be non-static method, and an instance must be provided.</b> For static methods,
        ///     see <see cref="BuildActionStatic{T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an action.</param>
        /// <param name="instance">The target object to invoke the method on. May not be null.
        ///     <br/><br/>For static methods, see <see cref="BuildActionStatic{T1, T2, T3, T4, T5}(MethodInfo)"/>.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> BuildActionWithSetInstance<T1, T2, T3, T4, T5>(this MethodInfo method, object instance) =>
            BuildActionWithSetInstance<Action<T1, T2, T3, T4, T5>>(method, instance, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TAction BuildActionWithSetInstance<TAction>(MethodInfo method, object instance, Type[] parameterTypes)
            where TAction : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(BuildActionStatic)} for static methods)!", nameof(method)) :
            BuildDelegateWithSetInstance<TAction>(method, instance, parameterTypes);

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance> BuildAction<TInstance>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance>>(method, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T> BuildAction<TInstance, T>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance, T>>(method, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2> BuildAction<TInstance, T1, T2>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance, T1, T2>>(method, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3> BuildAction<TInstance, T1, T2, T3>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance, T1, T2, T3>>(method, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> BuildAction<TInstance, T1, T2, T3, T4>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance, T1, T2, T3, T4>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5, T6}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> BuildAction<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method) =>
            BuildAction<TInstance, InstanceMethodAction<TInstance, T1, T2, T3, T4, T5>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TAction BuildAction<TInstance, TAction>(MethodInfo method, Type[] paramTypes)
            where TAction : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(BuildActionStatic)} for static methods)!", nameof(method)) :
            BuildDelegate<TAction>(method, typeof(TInstance), paramTypes);

        /// <summary>
        /// Builds this method into an <see cref="Action"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance}(MethodInfo)"/>.</remarks>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action BuildActionStatic(this MethodInfo method) =>
            BuildActionStatic<Action>(method, Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance, T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T> BuildActionStatic<T>(this MethodInfo method) =>
            BuildActionStatic<Action<T>>(method, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> BuildActionStatic<T1, T2>(this MethodInfo method) =>
            BuildActionStatic<Action<T1, T2>>(method, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3> BuildActionStatic<T1, T2, T3>(this MethodInfo method) =>
            BuildActionStatic<Action<T1, T2, T3>>(method, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4> BuildActionStatic<T1, T2, T3, T4>(this MethodInfo method) =>
            BuildActionStatic<Action<T1, T2, T3, T4>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildAction{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> BuildActionStatic<T1, T2, T3, T4, T5>(this MethodInfo method) =>
            BuildActionStatic<Action<T1, T2, T3, T4, T5>>(method, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        public static TAction BuildActionStatic<TAction>(MethodInfo method, Type[] parameterTypes)
            where TAction : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            method.IsStatic ? throw new ArgumentException($"Given method must be static (see {nameof(BuildAction)} for non-static methods)!", nameof(method)) :
            BuildDelegateStatic<TAction>(method, parameterTypes);
    }
}
