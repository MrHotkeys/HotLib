using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class IncompatibleReturnTypeException : Exception
    {
        public MethodInfo Method { get; }
        public Type Expected { get; }
        public Type Actual { get; }

        public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual)
            : this(method, expected, actual, null)
        { }

        public IncompatibleReturnTypeException(MethodInfo method, Type expected, Type actual, Exception? innerException)
            : base($"Unable to cast return type {actual} to type {expected} in method {method}!")
        {
            Method = method;
            Expected = expected;
            Actual = actual;
        }
    }
}
