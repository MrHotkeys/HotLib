using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class IncompatibleArgumentTypeException : Exception
    {
        public MethodInfo Method { get; }
        public ParameterInfo Parameter { get; }
        public object? Argument { get; }

        public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument)
            : this(method, parameter, argument, null)
        { }

        public IncompatibleArgumentTypeException(MethodInfo method, ParameterInfo parameter, object? argument, Exception? innerException)
            : base($"Unable to cast object {argument ?? "NULL"} of type {argument?.GetType()?.ToString() ?? "NULL"} to " +
                  $"{parameter.ParameterType} for parameter {parameter.Name} at position {parameter.Position} in method {method}!",
                  innerException)
        {
            Method = method;
            Parameter = parameter;
            Argument = argument;
        }
    }
}
