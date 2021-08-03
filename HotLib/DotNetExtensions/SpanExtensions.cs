using System;

namespace HotLib.DotNetExtensions
{
    public static class SpanExtensions
    {
        public static void CopyToReversed<T>(this ReadOnlySpan<T> source, Span<T> dest)
        {
            if (source.Length > dest.Length)
                throw new ArgumentException($"Dest span is too small (need room for {source.Length}, only have room for {dest.Length})!");

            var sourceIndex = source.Length - 1;
            var destIndex = 0;
            while (sourceIndex >= 0)
            {
                dest[destIndex] = source[sourceIndex];

                sourceIndex--;
                destIndex++;
            }
        }

        public static void CopyToReversed<T>(this Span<T> source, Span<T> dest) =>
            CopyToReversed((ReadOnlySpan<T>)source, dest);
    }
}
