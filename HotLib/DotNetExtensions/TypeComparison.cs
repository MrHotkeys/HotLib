namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Different methods that can be used to compare <see cref="Type"/> objects.
    /// </summary>
    public enum TypeComparison
    {
        /// <summary>
        /// The default comparison is used (typically by comparing references).
        /// </summary>
        Default,
        /// <summary>
        /// The types are compared by the values of their <see cref="Type.AssemblyQualifiedName"/>.
        /// </summary>
        AssemblyQualifiedName,
    }
}
