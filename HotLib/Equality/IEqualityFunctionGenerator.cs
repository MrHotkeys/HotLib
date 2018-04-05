using System.Reflection;

namespace HotLib.Equality
{
    /// <summary>
    /// Defines public members for generateing <see cref="EqualityFunction{T}"/> delegates based off of given types.
    /// </summary>
    public interface IEqualityFunctionGenerator
    {
        /// <summary>
        /// Gets a <see cref="EqualityFunction{T}"/> for comparing objects of the given <see cref="System.Type"/> for equality.
        /// </summary>
        /// <param name="declaredMembers">An array of all the members declared in the given type
        ///     to include in determining equality.</param>
        /// <param name="inheritedMembers">An array of all the members inherited by the given type
        ///     from base types to include in determining equality.</param>
        /// <returns>The created equality function.</returns>
        EqualityFunction<T> BuildEqualityFunction<T>(MemberInfo[] declaredMembers, MemberInfo[] inheritedMembers);
    }
}