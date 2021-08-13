using System;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="Exception"/> instances.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Executes the given action with this exception as the argument, then returns the exception.
        /// Intended to fluently allow an exception to have some action performed (e.g. logging) in
        /// the same statement in which it is thrown.
        /// <br /><br />
        /// Example: <br />
        /// <c>throw new Exception().AndDo(e => Log(e))</c>
        /// </summary>
        /// <typeparam name="T">The type of exception being worked with.</typeparam>
        /// <param name="exception">The exception being worked with.</param>
        /// <param name="action">The action to invoke with the exception.</param>
        /// <returns>The same exception instance passed to this method.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception"/> or <paramref name="action"/> is null.</exception>
        public static T AndDo<T>(this T exception, Action<T> action)
            where T : Exception
        {
            if (exception is null)
                throw new ArgumentNullException(nameof(exception));
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            action.Invoke(exception);
            return exception;
        }
    }
}
