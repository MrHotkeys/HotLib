using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotLib.DotNetExtensions
{
    public static partial class MethodInfoExtensions
    {
        public static bool IsOverrideOf(this MethodInfo method, MethodInfo other)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (other is null)
                throw new ArgumentNullException(nameof(other));

            if (!StringComparer.Ordinal.Equals(method.Name, other.Name))
                return false;

            return method.EnumerateBaseDefinitions()
                          .Contains(other);
        }

        public static IEnumerable<MethodInfo> EnumerateBaseDefinitions(this MethodInfo method)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));

            var current = method;
            var next = current.GetBaseDefinition();
            while (next != current)
            {
                current = next;
                yield return current;
                next = current.GetBaseDefinition();
            }
        }

        /// <summary>
        /// Constructs this method with the given types as type parameters. If the method is already constructed, its generic type definition is retrieved and used.
        /// </summary>
        /// <param name="method">The method to construct.</param>
        /// <param name="typeArgs">The type args to use when constructing the method, in the same order they appear in the method's signature.</param>
        /// <returns>The constructed generic method.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="method"/> or <paramref name="typeArgs"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="method"/> is not a generic method.
        ///     -or-<paramref name="typeArgs"/> does not contain enough type arguments for the method.
        ///     -or-One of the types in <paramref name="typeArgs"/> does not satisfy a type constraint on the method's signature.</exception>
        public static MethodInfo WithTypeParams(this MethodInfo method, params Type[] typeArgs)
        {
            if (method is null)
                throw new ArgumentNullException(nameof(method));
            if (typeArgs is null)
                throw new ArgumentNullException(nameof(typeArgs));
            if (!method.IsGenericMethod)
                throw new ArgumentException($"Cannot use type arguments with non-generic method {method}!", nameof(method));

            var openMethod = method.IsGenericMethodDefinition ?
                method :
                method.GetGenericMethodDefinition();

            try
            {
                return openMethod.MakeGenericMethod(typeArgs);
            }
            catch (ArgumentException e) when (!System.Diagnostics.Debugger.IsAttached)
            {
                var typeParameters = openMethod.GetGenericArguments();

                if (typeParameters.Length != typeArgs.Length) // Not enough type args
                {
                    throw new ArgumentException(
                        $"Insufficient type arguments given for method {openMethod} (got {typeArgs.Length}, expected {typeParameters.Length})!",
                        nameof(typeArgs), e);
                }
                else if (e.InnerException is System.Security.VerificationException) // Type args don't satisfy constraints
                {
                    throw new ArgumentException(
                        $"Type arguments given do not satisfy type parameter constraints on method {openMethod} (see inner exception for more)!",
                        nameof(typeArgs), e);
                }
                else // Undocumented
                {
                    throw new ArgumentException(
                        $"Unknown exception occurred while constructing generic method from {openMethod} (see inner exception for more)!",
                        nameof(typeArgs), e);
                }
            }
        }

        /// <summary>
        /// Invokes the method using <see cref="MethodBase.Invoke(object?, object?[]?)"/>.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="obj">The target object to invoke the method on.</param>
        /// <param name="args">The arguments to supply to the method when invoked.</param>
        /// <returns>The value returned from invoking the method, or null if void.</returns>
        /// <exception cref="ArgumentException">The given arguments do not match the parameter list for <paramref name="method"/>.</exception>
        /// <exception cref="TargetException"><paramref name="method"/> is static and <paramref name="obj"/> is null.
        ///     -or-<paramref name="method"/> is not defined or inherited by <paramref name="obj"/>.
        ///     -or-<paramref name="method"/> is a static constructor and <paramref name="obj"/> is neither
        ///     null or an instance of the class declaring the constructor.</exception>
        /// <exception cref="TargetInvocationException"><paramref name="method"/> threw an exception when invoked.
        ///     -or-<paramref name="obj"/> is a <see cref="System.Reflection.Emit.DynamicMethod"/> with unverifiable code.</exception>
        /// <exception cref="TargetParameterCountException">The number of given arguments does not match the method signature.</exception>
        /// <exception cref="MethodAccessException">The caller does not have permission to invoke the <paramref name="method"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="method"/> is declared by an open generic type.</exception>
        /// <exception cref="NotSupportedException"><paramref name="obj"/> is a <see cref="System.Reflection.Emit.MethodBuilder"/>.</exception>
        public static object? Invoke(this MethodInfo method, object? obj, params object?[]? args) =>
            method.Invoke(obj, args);

        /// <summary>
        /// Invokes the method using <see cref="MethodBase.Invoke(object?, object?[]?)"/>.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the result to.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="obj">The target object to invoke the method on.</param>
        /// <param name="args">The arguments to supply to the method when invoked.</param>
        /// <returns>The value returned from invoking the method, cast as <typeparamref name="TReturn"/>.</returns>
        /// <exception cref="ArgumentException">The given arguments do not match the parameter list for <paramref name="method"/>.</exception>
        /// <exception cref="InvalidCastException">The method returned null and <typeparamref name="TReturn"/> cannot be null.
        ///     -or-The method returned a value which cannot be cast to <typeparamref name="TReturn"/>.</exception>
        /// <exception cref="TargetException"><paramref name="method"/> is static and <paramref name="obj"/> is null.
        ///     -or-<paramref name="method"/> is not defined or inherited by <paramref name="obj"/>.
        ///     -or-<paramref name="method"/> is a static constructor and <paramref name="obj"/> is neither
        ///     null or an instance of the class declaring the constructor.</exception>
        /// <exception cref="TargetInvocationException"><paramref name="method"/> threw an exception when invoked.
        ///     -or-<paramref name="obj"/> is a <see cref="System.Reflection.Emit.DynamicMethod"/> with unverifiable code.</exception>
        /// <exception cref="TargetParameterCountException">The number of given arguments does not match the method signature.</exception>
        /// <exception cref="MethodAccessException">The caller does not have permission to invoke the <paramref name="method"/>.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="method"/> is declared by an open generic type.</exception>
        /// <exception cref="NotSupportedException"><paramref name="obj"/> is a <see cref="System.Reflection.Emit.MethodBuilder"/>.</exception>
        public static TReturn? Invoke<TReturn>(this MethodInfo method, object? obj, params object?[]? args)
        {
            var result = method.Invoke(obj, args);

            if (result is null)
            {
                if (typeof(TReturn).CanBeSetToNull())
                    return default;
                else
                    throw new InvalidCastException($"Method {method} returned null when invoked, unable to cast to {typeof(TReturn)}!");
            }
            else
            {
                var resultType = result.GetType();
                if (typeof(TReturn).IsAssignableFrom(resultType))
                {
                    return (TReturn)result;
                }
                else
                {
                    throw new InvalidCastException($"Method {method} returned {result.GetType()} when invoked, unable to cast to {typeof(TReturn)}!");
                }
            }
        }
    }
}
