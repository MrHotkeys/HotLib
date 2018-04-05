namespace HotLib.Equality
{
    /// <summary>
    /// The delegate type for hash code functions which are used to get hash codes from objects by hashing specific members.
    /// </summary>
    /// <typeparam name="T">The type of target object that the function can create hash codes from.</typeparam>
    /// <param name="target">The target object to create a hash code from.</param>
    /// <param name="includeInherited">Whether or not to include members defined in base types.</param>
    /// <param name="equalityComparer">Used to hash member values.</param>
    /// <returns>The hash code for the given target object.</returns>
    public delegate int HashCodeFunction<T>(T target, bool includeInherited, IUniversalEqualityComparer equalityComparer);
}
