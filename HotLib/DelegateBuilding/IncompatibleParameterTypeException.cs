using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class IncompatibleParameterTypeException : Exception
    {
        public MethodInfo Method { get; }
        public ParameterInfo ParameterActual { get; }
        public Type ParameterTypeGiven { get; }

        public IncompatibleParameterTypeException(MethodInfo method, ParameterInfo parameterActual, Type parameterTypeGiven)
            : this(method, parameterActual, parameterTypeGiven, null)
        { }

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
