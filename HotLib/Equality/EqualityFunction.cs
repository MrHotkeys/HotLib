namespace HotLib.Equality
{
    /// <summary>
    /// The delegate type for equality functions used to test the equality of objects by comparing specific members.
    /// </summary>
    /// <typeparam name="T">The type of target object that the function can compare for equality.</typeparam>
    /// <param name="x">The first target object to compare.</param>
    /// <param name="y">The second target object to compare.</param>
    /// <param name="includeInherited">Whether or not to include members defined in base types.</param>
    /// <param name="equalityComparer">Used to hash member values.</param>
    /// <returns>The hash code for the given target object.</returns>
    public delegate bool EqualityFunction<T>(T x, T y, bool includeInherited, IUniversalEqualityComparer equalityComparer);
}
