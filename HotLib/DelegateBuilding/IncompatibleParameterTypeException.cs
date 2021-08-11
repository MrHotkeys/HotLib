using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    /// <summary>
    /// The exception thrown when a method encapsulating delegate is built with a parameter type that is
    /// incompatible with its matched parameter type in the encapsulated method's signature.
    /// </summary>
    public class IncompatibleParameterTypeException : Exception
    {
        /// <summary>
        /// Gets the method being encapsulated and built into a delegate.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Gets the parameter definition from the encapsulated method signature.
        /// </summary>
        public ParameterInfo ParameterActual { get; }

        /// <summary>
        /// Gets the type of parameter the delegate was attempted to be built with.
        /// </summary>
        public Type ParameterTypeGiven { get; }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleParameterTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="parameterActual">The parameter definition from the encapsulated method signature.</param>
        /// <param name="parameterTypeGiven">The type of parameter the delegate was attempted to be built with.</param>
        public IncompatibleParameterTypeException(MethodInfo method, ParameterInfo parameterActual, Type parameterTypeGiven)
            : this(method, parameterActual, parameterTypeGiven, null)
        { }

        /// <summary>
        /// Instantiates a new <see cref="IncompatibleParameterTypeException"/>.
        /// </summary>
        /// <param name="method">The method being encapsulated and built into a delegate.</param>
        /// <param name="parameterActual">The parameter definition from the encapsulated method signature.</param>
        /// <param name="parameterTypeGiven">The type of parameter the delegate was attempted to be built with.</param>
        /// <param name="innerException">An inner exception thrown that raised this exception.</param>
        public IncompatibleParameterTypeException(MethodInfo method, ParameterInfo parameterActual, Type parameterTypeGiven, Exception? innerException)
            : base($"Unable to cast parameter of type {parameterTypeGiven} to " +
                  $"{parameterActual.ParameterType} for parameter {parameterActual.Name} at position {parameterActual.Position} in method {method}!", innerException)
        {
            Method = method;
            ParameterActual = parameterActual;
            ParameterTypeGiven = parameterTypeGiven;
        }
    }
}
