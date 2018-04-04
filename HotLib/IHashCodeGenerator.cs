namespace HotLib
{
    /// <summary>
    /// Defines public members for generically generating hash codes from objects.
    /// </summary>
    public interface IHashCodeGenerator
    {
        /// <summary>
        /// Gets a hash code from the given target object.
        /// </summary>
        /// <typeparam name="T">The type of target object to get a hash code from.</typeparam>
        /// <param name="target">The target object to get a hash code from.</param>
        /// <returns>The object's hash code.</returns>
        int GetHashCode<T>(T target);
    }
}
