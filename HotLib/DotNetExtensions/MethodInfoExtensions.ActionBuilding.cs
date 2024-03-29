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
        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action CreateAction(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            CreateDelegate<Action>(method, builderSetup);

        /// <inheritdoc cref="CreateAction{T}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T> CreateAction<T>(this MethodInfo method) =>
            CreateAction<T>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T}"/> that invokes the method.
        /// </summary>
        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T> CreateAction<T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateAction<Action<T>>(method, builderSetup, TypeHelpers.TypeArray<T>());

        /// <inheritdoc cref="CreateAction{T1, T2}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2> CreateAction<T1, T2>(this MethodInfo method) =>
            CreateAction<T1, T2>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2}"/> that invokes the method.
        /// </summary>
        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2> CreateAction<T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateAction<Action<T1, T2>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2>());

        /// <inheritdoc cref="CreateAction{T1, T2, T3}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(this MethodInfo method) =>
            CreateAction<T1, T2, T3>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3}"/> that invokes the method.
        /// </summary>
        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2, T3> CreateAction<T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateAction<Action<T1, T2, T3>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3>());

        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(this MethodInfo method) =>
            CreateAction<T1, T2, T3, T4>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4}"/> that invokes the method.
        /// </summary>
        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2, T3, T4> CreateAction<T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateAction<Action<T1, T2, T3, T4>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <inheritdoc cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static Action<T1, T2, T3, T4, T5> CreateAction<T1, T2, T3, T4, T5>(this MethodInfo method) =>
            CreateAction<T1, T2, T3, T4, T5>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="Action{T1, T2, T3, T4, T5}"/> that invokes the method.
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
        /// <exception cref="ArgumentNullException"><paramref name="method"/> is null.</exception>
        public static Action<T1, T2, T3, T4, T5> CreateAction<T1, T2, T3, T4, T5>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateAction<Action<T1, T2, T3, T4, T5>>(method, builderSetup, TypeHelpers.TypeArray<T1, T2, T3, T4, T5>());

        private static TDelegate CreateAction<TDelegate>(MethodInfo method, Action<DelegateBuilder> builderSetup, Type[] parameterTypes)
            where TDelegate : Delegate =>
            method is null ? throw new ArgumentNullException(nameof(method)) :
            builderSetup is null ? throw new ArgumentNullException(nameof(builderSetup)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseArgumentsFromParameters(parameterTypes);

                builderSetup(b);
            });

        /// <inheritdoc cref="CreateActionInstanced{TInstance}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance> CreateActionInstanced<TInstance>(this MethodInfo method) =>
            CreateActionInstanced<TInstance>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// </summary>
        /// <remarks><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction(MethodInfo, Action{DelegateBuilder})"/>.</remarks>
        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance> CreateActionInstanced<TInstance>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance>>(method, builderSetup, typeof(TInstance), Type.EmptyTypes);

        /// <inheritdoc cref="CreateActionInstanced{TInstance, T}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T> CreateActionInstanced<TInstance, T>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// <para><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction{T}(MethodInfo)"/>.</para>
        /// </summary>
        /// <typeparam name="T">The type of the argument passed to the method call.
        ///     Will attempt to convert to the method's first parameter type if not the exact same type.</typeparam>
        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T> CreateActionInstanced<TInstance, T>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T>());

        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2> CreateActionInstanced<TInstance, T1, T2>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// <para><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction{T1, T2}(MethodInfo)"/>.</para>
        /// </summary>
        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2> CreateActionInstanced<TInstance, T1, T2>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2>());

        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2, T3> CreateActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// <para><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction{T1, T2, T3}(MethodInfo)"/>.</para>
        /// </summary>
        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2, T3> CreateActionInstanced<TInstance, T1, T2, T3>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3>());

        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3, T4}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> CreateActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3, T4>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// <para><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction{T1, T2, T3, T4}(MethodInfo)"/>.</para>
        /// </summary>
        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4> CreateActionInstanced<TInstance, T1, T2, T3, T4>(this MethodInfo method, Action<DelegateBuilder> builderSetup) =>
            CreateActionInstanced<InstanceMethodAction<TInstance, T1, T2, T3, T4>>(method, builderSetup, typeof(TInstance), TypeHelpers.TypeArray<T1, T2, T3, T4>());

        /// <inheritdoc cref="CreateActionInstanced{TInstance, T1, T2, T3, T4, T5}(MethodInfo, Action{DelegateBuilder})"/>
        public static InstanceMethodAction<TInstance, T1, T2, T3, T4, T5> CreateActionInstanced<TInstance, T1, T2, T3, T4, T5>(this MethodInfo method) =>
            CreateActionInstanced<TInstance, T1, T2, T3, T4, T5>(method, DelegateBuilderSetupNoOp);

        /// <summary>
        /// Builds this method into an <see cref="InstanceMethodAction{TInstance, T1, T2, T3, T4, T5}"/> that invokes the method using the argument
        /// to the first parameter of the action as the target object.
        /// <para><b>Must be non-static method.</b> For static methods, see <see cref="CreateAction{T1, T2, T3, T4, T5}(MethodInfo)"/>.</para>
        /// </summary>
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
            method.IsStatic ? throw new ArgumentException($"Given method may not be static (see {nameof(CreateAction)} for static methods)!", nameof(method)) :
            CreateDelegate<TDelegate>(method, b =>
            {
                b.UseInstanceParameter(instanceType);
                b.UseArgumentsFromParameters(parameterTypes);

                builderSetup(b);
            });
    }
}
