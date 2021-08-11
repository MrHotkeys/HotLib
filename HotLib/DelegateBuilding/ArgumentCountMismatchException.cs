using System;
using System.Reflection;

namespace HotLib.DelegateBuilding
{
    public class ArgumentCountMismatchException : Exception
    {
        public MethodInfo Method { get; }
        public int Expected { get; }
        public int Actual { get; }

        public ArgumentCountMismatchException(MethodInfo method, int expected, int actual)
            : this(method, expected, actual, null)
        { }

        public ArgumentCountMismatchException(MethodInfo method, int expected, int actual, Exception? innerException)
            : base($"Incorrect number of parameters given for method {method} (expected {expected}, got {actual})!", innerException)
        {
            Method = method;
            Expected = expected;
            Actual = actual;
        }
    }
}
