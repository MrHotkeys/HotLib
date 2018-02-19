using System.ComponentModel.DataAnnotations;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="ValidationResult"/>.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Returns if the given validation result is a successful result or not.
        /// </summary>
        /// <param name="validationResult">The validation result to test.</param>
        /// <returns>True if success, false if not.</returns>
        public static bool IsSuccess(this ValidationResult validationResult)
        {
            return validationResult == ValidationResult.Success;
        }
    }
}
