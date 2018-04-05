using System.Reflection;

namespace HotLib.Equality
{
    /// <summary>
    /// Defines public members for generating <see cref="HashCodeFunction{T}"/> delegates based off of given types.
    /// </summary>
    public interface IHashCodeFunctionGenerator
    {
        /// <summary>
        /// Gets a <see cref="HashCodeFunction{T}"/> for hashing objects of the given <see cref="System.Type"/>.
        /// </summary>
        /// <param name="declaredMembers">An array of all the members declared in the given type
        ///     to include in calculating the hash code.</param>
        /// <param name="inheritedMembers">An array of all the members inherited by the given type
        ///     from base types to include in calculating the hash code.</param>
        /// <returns>The created hash code function.</returns>
        HashCodeFunction<T> BuildHashCodeFunction<T>(MemberInfo[] declaredMembers, MemberInfo[] inheritedMembers);
    }
}