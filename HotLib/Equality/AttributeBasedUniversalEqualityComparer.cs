using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using HotLib.DotNetExtensions;

namespace HotLib.Equality
{
    /// <summary>
    /// Used to get hash codes for objects by hashing all members marked with <see cref="IncludeAttribute"/>.
    /// Contains classes for marking members as belong to hash codes and for hashing them.
    /// </summary>
    public class AttributeBasedUniversalEqualityComparer : IUniversalEqualityComparer
    {
        /// <summary>
        /// Marks a field or property to be included in calculating a hash code.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class IncludeAttribute : Attribute
        {
            /// <summary>
            /// Instantiates a new <see cref="IncludeAttribute"/>.
            /// </summary>
            public IncludeAttribute()
            { }
        }

        /// <summary>
        /// Backs the <see cref="EqualityFunctionGenerator"/> property.
        /// </summary>
        private IEqualityFunctionGenerator _equalityFunctionGenerator = new EqualityFunctionGenerator();
        /// <summary>
        /// Gets/Sets the <see cref="IEqualityFunctionGenerator"/> used to generate equality comparison functions.
        /// </summary>
        public IEqualityFunctionGenerator EqualityFunctionGenerator
        {
            get => _equalityFunctionGenerator;
            set => _equalityFunctionGenerator = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Backs the <see cref="HashCodeFunctionGenerator"/> property.
        /// </summary>
        private IHashCodeFunctionGenerator _hashCodeFunctionGenerator = new HashCodeFunctionGenerator();
        /// <summary>
        /// Gets/Sets the <see cref="IHashCodeFunctionGenerator"/> used to generate hash code functions.
        /// </summary>
        public IHashCodeFunctionGenerator HashCodeFunctionGenerator
        {
            get => _hashCodeFunctionGenerator;
            set => _hashCodeFunctionGenerator = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Gets the dictionary of cached equality comparison functions, with the types the functions correspond to as the keys.
        /// </summary>
        public ConcurrentDictionary<Type, object> EqualityFunctionCache { get; } = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Gets the dictionary of cached hash code functions, with the types the functions correspond to as the keys.
        /// </summary>
        public ConcurrentDictionary<Type, object> HashCodeFunctionCache { get; } = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Instantiates a new <see cref="AttributeBasedUniversalEqualityComparer"/>.
        /// </summary>
        public AttributeBasedUniversalEqualityComparer()
        { }

        /// <summary>
        /// Builds the <see cref="HashCodeFunction{T}"/> and <see cref="EqualityFunction{T}"/>
        /// for the given type, and caches them int the appropriate dictionaries.
        /// </summary>
        /// <typeparam name="T">The type of object to build the functions for.</typeparam>
        /// <exception cref="InvalidOperationException">Type <typeparamref name="T"/> has no
        ///     members marked with <see cref="IncludeAttribute"/>.</exception>
        public virtual void BuildFunctions<T>() => BuildFunctions<T>(out _, out _);

        /// <summary>
        /// Builds the <see cref="HashCodeFunction{T}"/> and <see cref="EqualityFunction{T}"/>
        /// for the given type, and caches them int the appropriate dictionaries.
        /// </summary>
        /// <typeparam name="T">The type of object to build the functions for.</typeparam>
        /// <param name="hashCodeFunction">Will be set to the generated <see cref="HashCodeFunction{T}"/>.</param>
        /// <param name="equalityFunction">Will be set to the generated <see cref="EqualityFunction{T}"/>.</param>
        /// <exception cref="InvalidOperationException">Type <typeparamref name="T"/> has no
        ///     members marked with <see cref="IncludeAttribute"/>.</exception>
        public virtual void BuildFunctions<T>(out HashCodeFunction<T> hashCodeFunction, out EqualityFunction<T> equalityFunction)
        {
            GetIncludedMembers<T>(out var declaredMembers, out var inheritedMembers);

            if (declaredMembers.Length == 0 && inheritedMembers.Length == 0)
                throw new InvalidOperationException($"{typeof(T)} has no members marked with {nameof(IncludeAttribute)}!");

            hashCodeFunction = HashCodeFunctionGenerator.BuildHashCodeFunction<T>(declaredMembers, inheritedMembers);
            HashCodeFunctionCache.TryAdd(typeof(T), hashCodeFunction);

            equalityFunction = EqualityFunctionGenerator.BuildEqualityFunction<T>(declaredMembers, inheritedMembers);
            EqualityFunctionCache.TryAdd(typeof(T), equalityFunction);
        }

        /// <summary>
        /// Checks for equality between two objects by comparing the values
        /// of all members marked with <see cref="IncludeAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of target object to get a hash code for.</typeparam>
        /// <param name="x">The first target object to get a hash code for.</param>
        /// <param name="y">The second target object to get a hash code for.</param>
        /// <param name="includeInherited">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeAttribute"/>.</param>
        /// <param name="equalityComparer">Used to hash member values.
        ///     If null, <see cref="DefaultUniversalEqualityComparer"/> will be used.</param>
        /// <returns>The generated hash code.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="includeInherited"/> is false, but there
        ///     are no declared members marked with <see cref="IncludeAttribute"/>.</exception>
        public virtual bool Equals<T>(T x, T y, bool includeInherited = true,
                                      IUniversalEqualityComparer equalityComparer = null)
        {
            if (x == null)
                return y == null;
            else if (y == null)
                return false;

            try
            {
                return GetEqualityFunction<T>().Invoke(x, y, includeInherited,
                                                       equalityComparer ?? DefaultUniversalEqualityComparer.Instance);
            }
            catch (OnlyInheritedMembersException e) when (!System.Diagnostics.Debugger.IsAttached)
            {
                var exceptionMessage = $"{typeof(T)} has no members marked with {typeof(IncludeAttribute)}, so " +
                                       $"inherited members must be included or there will be nothing to compare!";
                throw new InvalidOperationException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Checks if two objects are equal.
        /// </summary>
        /// <typeparam name="T">The type of the objects being compared.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><see langword="true"/> if the objects are equal, <see langword="false"/> if not.</returns>
        bool IUniversalEqualityComparer.Equals<T>(T x, T y) => Equals(x, y);

        /// <summary>
        /// Gets the <see cref="EqualityFunction{T}"/> for the given type. If it has not already been created,
        /// calls <see cref="BuildFunctions{T}(out HashCodeFunction{T}, out EqualityFunction{T})"/> to create it.
        /// </summary>
        /// <typeparam name="T">The type of object to get the equality comparison function for.</typeparam>
        /// <returns>The appropriate equality comparison function.</returns>
        protected virtual EqualityFunction<T> GetEqualityFunction<T>()
        {
            if (!EqualityFunctionCache.TryGetValue(typeof(T), out var boxedEqualityFunction))
            {
                BuildFunctions<T>(out _, out var equalityFunction);
                return equalityFunction;
            }
            else
            {
                return boxedEqualityFunction as EqualityFunction<T>;
            }
        }

        /// <summary>
        /// Gets a hash code for an object by hashing all members marked with <see cref="IncludeAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of target object to get a hash code for.</typeparam>
        /// <param name="target">The target object to get a hash code for.</param>
        /// <param name="includeInherited">Whether or not to include members defined
        ///     in base types marked with <see cref="IncludeAttribute"/>.</param>
        /// <param name="equalityComparer">Used to hash member values.
        ///     If null, <see cref="DefaultUniversalEqualityComparer"/> will be used.</param>
        /// <returns>The generated hash code.</returns>
        /// <exception cref="InvalidOperationException"><paramref name="includeInherited"/> is false, but there
        ///     are no declared members marked with <see cref="IncludeAttribute"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is null.</exception>
        public virtual int GetHashCode<T>(T target, bool includeInherited = true,
                                          IUniversalEqualityComparer equalityComparer = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            try
            {
                return GetHashCodeFunction<T>().Invoke(target, includeInherited,
                                                       equalityComparer ?? DefaultUniversalEqualityComparer.Instance);
            }
            catch (OnlyInheritedMembersException e) when (!System.Diagnostics.Debugger.IsAttached)
            {
                var exceptionMessage = $"{typeof(T)} has no members marked with {typeof(IncludeAttribute)}, so " +
                                       $"inherited members must be included or there will be nothing to hash!";
                throw new InvalidOperationException(exceptionMessage, e);
            }
        }

        /// <summary>
        /// Gets a hash code for an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to get a hash code for.</typeparam>
        /// <param name="obj">The object to get a hash code for.</param>
        /// <returns>The object's hash code.</returns>
        int IUniversalEqualityComparer.GetHashCode<T>(T obj) => GetHashCode(obj);

        /// <summary>
        /// Gets the <see cref="HashCodeFunction{T}"/> for the given type. If it has not already been created,
        /// calls <see cref="BuildFunctions{T}(out HashCodeFunction{T}, out EqualityFunction{T})"/> to create it.
        /// </summary>
        /// <typeparam name="T">The type of object to get the hash code function for.</typeparam>
        /// <returns>The appropriate hash code function.</returns>
        public virtual HashCodeFunction<T> GetHashCodeFunction<T>()
        {
            if (!HashCodeFunctionCache.TryGetValue(typeof(T), out var boxedHashCodeFunction))
            {
                BuildFunctions<T>(out var hashCodeFunction, out _);
                return hashCodeFunction;
            }
            else
            {
                return boxedHashCodeFunction as HashCodeFunction<T>;
            }
        }

        /// <summary>
        /// Gets all members marked with <see cref="IncludeAttribute"/> in the given type.
        /// </summary>
        /// <typeparam name="T">The type to get the members from.</typeparam>
        /// <param name="declared">Will be set to an array of all declared members marked
        ///     with <see cref="IncludeAttribute"/>.</param>
        /// <param name="inherited">Will be set to an array of all inherited members marked
        ///     with <see cref="IncludeAttribute"/>.</param>
        /// <returns>An enumerable of all found members.</returns>
        protected virtual void GetIncludedMembers<T>(out MemberInfo[] declared, out MemberInfo[] inherited)
        {
            if (typeof(T) == typeof(object))
            {
                declared = Array.Empty<MemberInfo>();
                inherited = Array.Empty<MemberInfo>();
                return;
            }
            else
            {
                declared = typeof(T).GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(m => m.GetCustomAttribute(typeof(IncludeAttribute)) != null)
                                    .ToArray();

                if (typeof(T).IsValueType)
                {
                    inherited = Array.Empty<MemberInfo>();
                    return;
                }

                var declaredProperties = declared.OfType<PropertyInfo>()
                                                 .ToArray();

                var inheritedSet = new HashSet<MemberInfo>();
                var currentType = typeof(T).BaseType;
                while (currentType != typeof(object))
                {
                    var membersWithAttribute = currentType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                                      BindingFlags.NonPublic | BindingFlags.Instance)
                                                          .Where(m => m.GetCustomAttribute(typeof(IncludeAttribute)) != null);

                    foreach (var member in membersWithAttribute)
                    {
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field when member is FieldInfo field:
                                {
                                    inheritedSet.Add(field);
                                    break;
                                }
                            case MemberTypes.Property when member is PropertyInfo property:
                                {
                                    if (property.GetMethod != null)
                                    {
                                        if (!declaredProperties.Any(p => p.GetMethod != null &&
                                            p.GetMethod.IsOverrideOf(property.GetMethod)))
                                        {
                                            inheritedSet.Add(property);
                                        }
                                    }
                                    else if (property.SetMethod != null)
                                    {
                                        if (!declaredProperties.Any(p => p.SetMethod != null &&
                                            p.SetMethod.IsOverrideOf(property.SetMethod)))
                                        {
                                            inheritedSet.Add(property);
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException();
                                    }

                                    break;
                                }
                            default:
                                throw new InvalidOperationException();
                        }
                    }

                    currentType = currentType.BaseType;
                }

                inherited = inheritedSet.ToArray();
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="UniversalEqualityComparerSpecificationWrapper{T}"/>
        /// that will allow this <see cref="AttributeBasedUniversalEqualityComparer"/> to act as an
        /// <see cref="IEqualityComparer{T}"/> for the given type.
        /// </summary>
        /// <typeparam name="T">The type to wrap as an equality comparer for.</typeparam>
        /// <returns>The new <see cref="UniversalEqualityComparerSpecificationWrapper{T}"/> instance.</returns>
        /// <remarks>The <see cref="UniversalEqualityComparerSpecificationWrapper{T}"/> instances are not cached;
        ///     they are created new with each call.</remarks>
        public UniversalEqualityComparerSpecificationWrapper<T> AsEqualityComparer<T>()
        {
            return new UniversalEqualityComparerSpecificationWrapper<T>(this);
        }
    }
}
