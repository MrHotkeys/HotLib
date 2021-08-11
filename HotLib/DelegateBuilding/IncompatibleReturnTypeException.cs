using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    /// <summary>
    /// The exception thrown when a method encapsulating delegate is built with a return type that is
    /// incompatible with the return type as defined by the encapsulated method's signature.
    /// </summary>
    public class IncompatibleReturnTypeException : Exception
    {
        /// <summary>
        /// Gets the method being encapsulated and built into a delegate.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the encapsulated method's actual return type.
        /// </summary>
        public Type Expected { get; }

        /// <summary>
        /// Gets the return type the delegate was attempted to be built with.
        /// </summary>
        public Type Actual { get; }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleReturnTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="expected">The encapsulated method's actual return type.</param>
        /// <param name="actual">The return type the delegate was attempted to be built with.</param>
        public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual)
            : this(method, expected, actual, null)
        { }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleReturnTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="expected">The encapsulated method's actual return type.</param>
        /// <param name="actual">The return type the delegate was attempted to be built with.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual, Exception? innerException)
            : base($"Unable to cast return type {actual} to type {expected} in method {method}!", innerException)
        {
            Method = method;
            Expected = expected;
            Actual = actual;
        }
    }
}
