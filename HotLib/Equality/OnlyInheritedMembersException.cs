using System;
using System.Linq;
using System.Reflection;

namespace HotLib.Equality
{
    /// <summary>
    /// The exception by an <see cref="EqualityFunction{T}"/> or <see cref="HashCodeFunction{T}"/> when it is
    /// instructed not to include a type's inherited members, but there are no declared members in that type.
    /// </summary>
    public class OnlyInheritedMembersException : Exception
    {
        /// <summary>
        /// Gets the <see cref="ConstructorInfo"/> for the constructor to <see cref="OnlyInheritedMembersException"/>.
        /// </summary>
        public static ConstructorInfo ConstructorInfo { get; } = typeof(OnlyInheritedMembersException).GetConstructors()
                                                                                                      .Single();

        /// <summary>
        /// Instantiates a new <see cref="OnlyInheritedMembersException"/>.
        /// </summary>
        /// <param name="type">The type of object that the function was built for.</param>
        public OnlyInheritedMembersException(Type type)
            : base($"{type} has no declared members set to be included, so inherited members must be included!")
        { }
    }
}
