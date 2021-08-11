using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    /// <summary>
    /// The exception thrown when a method encapsulating delegate is built with an instance that
    /// is incompatible with the encapsulated method's defining type or signature.
    /// </summary>
    public class IncompatibleInstanceException : Exception
    {
        /// <summary>
        /// Gets the method being encapsulated and built into a delegate.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the type of instance the delegate was attempted to be built with.
        /// </summary>
        public Type? InstanceType { get; }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleInstanceException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="instanceType">The type of instance the delegate was attempted to be built with.</param>
        /// <param name="message">The message to include with the exception describing the error.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        public IncompatibleInstanceException(MethodInfo method, Type? instanceType, string message, Exception? innerException)
            : base(message, innerException)
        {
            Method = method;
            InstanceType = instanceType;
        }

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a non-static method, but the instance is null.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForNullWithNonStatic(MethodInfo method) =>
            ForNullWithNonStatic(method, null);

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a non-static method, but the instance is null.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForNullWithNonStatic(MethodInfo method, Exception? innerException) =>
            new IncompatibleInstanceException(
                method: method,
                instanceType: null,
                message: $"Unable to build delegate invoking non-static method {method} with no instance!",
                innerException: innerException);

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a static method, but the instance is not null.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForNonNullWithStatic(MethodInfo method, Type instanceType) =>
            ForNonNullWithStatic(method, instanceType, null);

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a static method, but the instance is not null.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForNonNullWithStatic(MethodInfo method, Type instanceType, Exception? innerException) =>
            new IncompatibleInstanceException(
                method: method,
                instanceType: null,
                message: $"Unable to build delegate invoking static method {method} on instance of type {instanceType}!",
                innerException: innerException);

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a non-static method, but the method is not defined for the given instance type.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForIncompatibleType(MethodInfo method, Type instanceType) =>
            ForIncompatibleType(method, instanceType, null);

        /// <summary>
        /// Instantiats a new <see cref="IncompatibleInstanceException"/> for the case where a delegate
        /// is built for a non-static method, but the method is not defined for the given instance type.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        /// <returns>The created exception.</returns>
        public static IncompatibleInstanceException ForIncompatibleType(MethodInfo method, Type instanceType, Exception? innerException) =>
            new IncompatibleInstanceException(
                method: method,
                instanceType: null,
                message: $"Unable to build delegate invoking method {method} on instance of incompatible type {instanceType}!",
                innerException: innerException);
    }
}
