using System;

namespace HotLib
{
    public static class TypeHelpers
    {
        public static Type[] TypeArray<T1>() => new[]
        {
            typeof(T1),
        };

        public static Type[] TypeArray<T1, T2>() => new[]
        {
            typeof(T1),
            typeof(T2),
        };

        public static Type[] TypeArray<T1, T2, T3>() => new[]
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
        };

        public static Type[] TypeArray<T1, T2, T3, T4>() => new[]
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        };

        public static Type[] TypeArray<T1, T2, T3, T4, T5>() => new[]
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
            typeof(T5),
        };

        public static Type[] TypeArray<T1, T2, T3, T4, T5, T6>() => new[]
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
            typeof(T5),
            typeof(T6),
        };
    }
}
