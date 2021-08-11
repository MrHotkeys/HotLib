﻿using System;
using System.Reflection;

using HotLib.DelegateBuilding;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        /// <summary>
        /// Builds this method into an <see cref="Action"/> that invokes the method, configured with the
        /// given <paramref name="builderSetup"/> to set the instance, arguments, and other aspects.
        /// </summary>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static Action CreateAction(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            CreateDelegate<Action>(method, builderSetup);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction(MethodInfo, Action{DelegateBuilder})"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance> CreateActionInstanced<TInstance>(this MethodInfo method) =>
            CreateActionInstanced<TInstance>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction(MethodInfo, Action{DelegateBuilder})"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance> CreateActionInstanced<TInstance>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance>>(method, builderSetup, typeof(TInstance), Type.EmptyTypes);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T> CreateActionInstanced<TInstance, T>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T> CreateActionInstanced<TInstance, T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2> CreateActionInstanced<TInstance, T1, T2>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2> CreateActionInstanced<TInstance, T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3> CreateActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3> CreateActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> CreateActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3, T4>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> CreateActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3, T4>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4, T5}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> CreateActionInstanced<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3, T4, T5>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4, T5}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateActionStatic{T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="builderSetup"/> is null.</exception>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> CreateActionInstanced<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3, T4, T5>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TDelegate CreateActionInstanced<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type instanceType, Type[] parameterTypes)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(CreateActionStatic)} for static methods)!", nameof(method)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseInstanceParameter(instanceType);
                b.UseArgumentsFromParameters(parameterTypes);
                
                builderSetup(b);
            });

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T> CreateActionStatic<T>(this MethodInfo method) =>
            CreateActionStatic<T>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T> CreateActionStatic<T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionStatic<Action<T>>(method, builderSetup, TypeHelpers.TypeArray<T>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> CreateActionStatic<T1, T2>(this MethodInfo method) =>
            CreateActionStatic<T1, T2>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2> CreateActionStatic<T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionStatic<Action<T1, T2>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3> CreateActionStatic<T1, T2, T3>(this MethodInfo method) =>
            CreateActionStatic<T1, T2, T3>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3}(MethodInfo)"/>.</remarks>
        /// <typeparam name="T1">The type of the first argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T2">The type of the second argument passed to the method call.
        ///     Will attempt to convert to the method's second parameter type if not the exact same type.</typeparam>
        /// <typeparam name="T3">The type of the third argument passed to the method call.
        ///     Will attempt to convert to the method's third parameter type if not the exact same type.</typeparam>
        /// <param name="method">The method to build into an action.</param>
        /// <param name="builderSetup">An action which can be used to further configure the built delegate.</param>
        /// <returns>The resulting action.</returns>
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3> CreateActionStatic<T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionStatic<Action<T1, T2, T3>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4> CreateActionStatic<T1, T2, T3, T4>(this MethodInfo method) =>
            CreateActionStatic<T1, T2, T3, T4>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3, T4}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4> CreateActionStatic<T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionStatic<Action<T1, T2, T3, T4>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> CreateActionStatic<T1, T2, T3, T4, T5>(this MethodInfo method) =>
            CreateActionStatic<T1, T2, T3, T4, T5>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method statically.
        /// </summary>
        /// <remarks><b>Must be static method.</b> For non-static methods, see <see cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo)"/>.</remarks>
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
        /// <exception cref="IncompatibleInstanceException">The method is static and no instance is given.
        ///     -or-The method is non-static and an instance is given.
        ///     -or-The method is not defined for the instance given.</exception>
        /// <exception cref="ArgumentCountMismatchException">The number of arguments given does not match the
        ///     number of parameters on the given method.</exception>
        /// <exception cref="IncompatibleArgumentTypeException">An argument value given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="IncompatibleParameterTypeException">An argument-mapped parameter type given can't be cast to be compatible with the
        ///     corresponding parameter in the method signature.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is non-static.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> CreateActionStatic<T1, T2, T3, T4, T5>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionStatic<Action<T1, T2, T3, T4, T5>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TDelegate CreateActionStatic<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type[] parameterTypes)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            !method.IsStatic ? throw new ArgumentException($"Given method must be static (see {nameof(CreateActionInstanced)} for non-static methods)!", nameof(method)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseArgumentsFromParameters(parameterTypes);

                builderSetup(b);
            });
    }
}
