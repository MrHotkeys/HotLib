using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    /// <summary>
    /// The exception thrown when a method encapsulating delegate is built with either too many arguments, or too few.
    /// </summary>
    public class ArgumentCountMismatchException : Exception
    {
        /// <summary>
        /// Gets the method being encapsulated and built into a delegate.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the number of parameters defined by the encapsulated method's signature.
        /// </summary>
        public int Expected { get; }

        /// <summary>
        /// Gets the number of arguments the delegate was attempted to be built with.
        /// </summary>
        public int Actual { get; }

        /// <summary>
        /// Instantiates a new <see cref="ArgumentCountMismatchException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="expected">The number of parameters defined by the encapsulated method's signature.</param>
        /// <param name="actual">The number of arguments the delegate was attempted to be built with.</param>
        public ArgumentCountMismatchException(MethodInfo method, int expected, int actual)
            : this(method, expected, actual, null)
        { }

        /// <summary>
        /// Instantiates a new <see cref="ArgumentCountMismatchException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="expected">The number of parameters defined by the encapsulated method's signature.</param>
        /// <param name="actual">The number of arguments the delegate was attempted to be built with.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        public ArgumentCountMismatchException(MethodInfo method, int expected, int actual, Exception? innerException)
            : base($"Incorrect number of parameters given for method {method} (expected {expected}, got {actual})!", innerException)
        {
            Method = method;
            Expected = expected;
            Actual = actual;
        }
    }
}
