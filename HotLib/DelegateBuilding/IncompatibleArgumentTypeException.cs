using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    /// <summary>
    /// The exception thrown when a method encapsulating delegate is built with an argument value whose type that is
    /// incompatible with its matched parameter type in the encapsulated method's signature.
    /// </summary>
    public class IncompatibleArgumentTypeException : Exception
    {
        /// <summary>
        /// Gets the method being encapsulated and built into a delegate.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the parameter definition from the encapsulated method signature.
        /// </summary>
        public ParameterInfo Parameter { get; }

        /// <summary>
        /// Gets the actual argument value the delegate was attempted to be built with.
        /// </summary>
        public object? Argument { get; }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleArgumentTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="parameterActual">The parameter definition from the encapsulated method signature.</param>
        /// <param name="argument">The actual argument value the delegate was attempted to be built with.</param>
        public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument)
            : this(method, parameter, argument, null)
        { }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleArgumentTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="parameterActual">The parameter definition from the encapsulated method signature.</param>
        /// <param name="argument">The actual argument value the delegate was attempted to be built with.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument, Exception? innerException)
            : base($"Unable to cast object {argument ?? "NULL"} of type {argument?.GetType()?.ToString() ?? "NULL"} to " +
                  $"{parameter.ParameterType} for parameter {parameter.Name} at position {parameter.Position} in method {method}!", innerException)
        {
            Method = method;
            Parameter = parameter;
            Argument = argument;
        }
    }
}
