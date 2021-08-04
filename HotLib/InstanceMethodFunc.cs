using System;

namespace HotLib
{
    /// <summary>
    /// Encapsulates an instance method with no parameters, much like <see cref="Func{TResult}"/>,
    /// with the exception that it allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    public delegate void InstanceMethodFunc<in TInstance, out TResult>(TInstance instance);

    /// <summary>
    /// Encapsulates an instance method with one parameter, much like <see cref="Func{T, TResult}"/>,
    /// with the exception that it also allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="T">The type of the argument to pass to the method call.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    /// <param name="arg">The argument to pass to the method call.</param>
    public delegate void InstanceMethodFunc<in TInstance, in T, out TResult>(TInstance instance, T arg);

    /// <summary>
    /// Encapsulates an instance method with two parameters, much like <see cref="Func{T1, T2, TResult}"/>,
    /// with the exception that it also allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="T1">The type of the first argument to pass to the method call.</typeparam>
    /// <typeparam name="T2">The type of the second argument to pass to the method call.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    /// <param name="arg1">The first argument to pass to the method call.</param>
    /// <param name="arg2">The second argument to pass to the method call.</param>
    public delegate void InstanceMethodFunc<in TInstance, in T1, in T2, out TResult>(TInstance instance, T1 arg1, T2 arg2);

    /// <summary>
    /// Encapsulates an instance method with three parameters, much like <see cref="Func{T1, T2, T3, TResult}"/>,
    /// with the exception that it also allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="T1">The type of the first argument to pass to the method call.</typeparam>
    /// <typeparam name="T2">The type of the second argument to pass to the method call.</typeparam>
    /// <typeparam name="T3">The type of the third argument to pass to the method call.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    /// <param name="arg1">The first argument to pass to the method call.</param>
    /// <param name="arg2">The second argument to pass to the method call.</param>
    /// <param name="arg3">The third argument to pass to the method call.</param>
    public delegate void InstanceMethodFunc<in TInstance, in T1, in T2, in T3, out TResult>(TInstance instance, T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Encapsulates an instance method with four parameters, much like <see cref="Func{T1, T2, T3, T4, TResult}"/>,
    /// with the exception that it also allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="T1">The type of the first argument to pass to the method call.</typeparam>
    /// <typeparam name="T2">The type of the second argument to pass to the method call.</typeparam>
    /// <typeparam name="T3">The type of the third argument to pass to the method call.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument to pass to the method call.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    /// <param name="arg1">The first argument to pass to the method call.</param>
    /// <param name="arg2">The second argument to pass to the method call.</param>
    /// <param name="arg3">The third argument to pass to the method call.</param>
    /// <param name="arg4">The fourth argument to pass to the method call.</param>
    public delegate void InstanceMethodFunc<in TInstance, in T1, in T2, in T3, in T4, out TResult>(TInstance instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Encapsulates an instance method with five parameters, much like <see cref="Func{T1, T2, T3, T4, T5, TResult}"/>,
    /// with the exception that it also allows for the target instance to be specified when called.
    /// </summary>
    /// <typeparam name="TInstance">The type of object to invoke the encapsulated method on.</typeparam>
    /// <typeparam name="T1">The type of the first argument to pass to the method call.</typeparam>
    /// <typeparam name="T2">The type of the second argument to pass to the method call.</typeparam>
    /// <typeparam name="T3">The type of the third argument to pass to the method call.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument to pass to the method call.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument to pass to the method call.</typeparam>
    /// <typeparam name="TResult">The value returned when invoked.</typeparam>
    /// <param name="instance">The instance to invoke the encapsulated method on.</param>
    /// <param name="arg1">The first argument to pass to the method call.</param>
    /// <param name="arg2">The second argument to pass to the method call.</param>
    /// <param name="arg3">The third argument to pass to the method call.</param>
    /// <param name="arg4">The fourth argument to pass to the method call.</param>
    /// <param name="arg5">The fifth argument to pass to the method call.</param>
    public delegate void InstanceMethodFunc<in TInstance, in T1, in T2, in T3, in T4, in T5, out TResult>(TInstance instance, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}
