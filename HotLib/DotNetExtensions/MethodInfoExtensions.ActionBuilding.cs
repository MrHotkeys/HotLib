using System;
using System.Reflection;

using HotLib.DelegateBuilding;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        private static readonly Action<DelegateBuilder> BuilderSetupNoOp = (b) => { };

        /// <summary>
        /// Builds this method into an <see cref="Action"/> that invokes the method, configured with the
        /// given <paramref name="builderSetup"/> to set the instance, arguments, and other aspects.
        /// </summary>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static Action BuildAction(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            BuildDelegate<Action>(method, builderSetup);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance> BuildActionInstanced<TInstance>(this MethodInfo method) =>
            BuildActionInstanced<TInstance>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance> BuildActionInstanced<TInstance>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance>>(method, builderSetup, typeof(TInstance), Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T> BuildActionInstanced<TInstance, T>(this MethodInfo method) =>
            BuildActionInstanced<TInstance, T>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T> BuildActionInstanced<TInstance, T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance, T>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2> BuildActionInstanced<TInstance, T1, T2>(this MethodInfo method) =>
            BuildActionInstanced<TInstance, T1, T2>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="BuildActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2> BuildActionInstanced<TInstance, T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance, T1, T2>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3}"/> that invokes the method using the argument
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
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3> BuildActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method) =>
            BuildActionInstanced<TInstance, T1, T2, T3>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3}"/> that invokes the method using the argument
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
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3> BuildActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4}"/> that invokes the method using the argument
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
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> BuildActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method) =>
            BuildActionInstanced<TInstance, T1, T2, T3, T4>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4}"/> that invokes the method using the argument
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
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> BuildActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3, T4>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4, T5}"/> that invokes the method using the argument
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
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> BuildActionInstanced<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method) =>
            BuildActionInstanced<TInstance, T1, T2, T3, T4, T5>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4, T5}"/> that invokes the method using the argument
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
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting delegate.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> BuildActionInstanced<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3, T4, T5>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TDelegate BuildActionInstanced<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type instanceType, Type[] parameterTypes)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            BuildDelegate<TDelegate>(method, b =>
            {
                b.UseInstanceParameter(instanceType);
                b.UseArgumentsFromParameters(parameterTypes);
                
                builderSetup(b);
            });

        public static Action<T> BuildActionStatic<T>(this MethodInfo method) =>
            BuildActionStatic<T>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T> BuildActionStatic<T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionStatic<Action<T>>(method, builderSetup, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> BuildActionStatic<T1, T2>(this MethodInfo method) =>
            BuildActionStatic<T1, T2>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> BuildActionStatic<T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionStatic<Action<T1, T2>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
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
            BuildActionStatic<T1, T2, T3>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3> BuildActionStatic<T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionStatic<Action<T1, T2, T3>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
            BuildActionStatic<T1, T2, T3, T4>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T4">The type of the fourth argument passed to the method call.
        ///     Will attempt to convert to the method's fourth parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4> BuildActionStatic<T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionStatic<Action<T1, T2, T3, T4>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
            BuildActionStatic<T1, T2, T3, T4, T5>(method, BuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="BuildActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> BuildActionStatic<T1, T2, T3, T4, T5>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            BuildActionStatic<Action<T1, T2, T3, T4, T5>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TDelegate BuildActionStatic<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type[] parameterTypes)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            BuildDelegate<TDelegate>(method, b =>
            {
                b.UseArgumentsFromParameters(parameterTypes);

                builderSetup(b);
            });
    }
}
