using System;
using System.Linq;

using HotLib.DotNetExtensions;

namespace HotLib.Helpers
{
    /// <summary>
    /// A static class used to validate Xml node names.
    /// </summary>
    public static class XmlNodeNameValidator
    {
        /// <summary>
        /// The exception thrown when <see cref="XmlNodeNameValidator"/> finds a problem with a node name.
        /// </summary>
        [Serializable]
        public sealed class InvalidNameException : Exception
        {
            /// <summary>
            /// Instantiates a new <see cref="InvalidNameException"/>.
            /// </summary>
            /// <param name="message">The message to include with the exception.</param>
            public InvalidNameException(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// Gets/Sets whether names will be forced to lowercase when validated. For best results this should be changed before any
        /// work is done with the class and should not be changed between versions so mixed results are not yielded.
        /// </summary>
        public static bool ForceLowercase { get; set; } = false;

        /// <summary>
        /// Validates the given name, returning it with any necessary minor changes (eg forcing lowercase) and throwing
        /// <see cref="InvalidNameException"/> exceptions when there are bigger issues (eg invalid characters).
        /// </summary>
        /// <param name="name">The name to parse.</param>
        /// <returns>The name with any necessary minor changes (eg forcing lowercase).</returns>
        /// <exception cref="ArgumentNullException">name is null.</exception>
        /// <exception cref="InvalidNameException">name is empty, contains invalid characters, doesn't start with a letter/underscore, starts with "xml".</exception>
        public static string ValidateName(string name)
        {
            // Make sure name is not null
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            // Make sure name is not empty
            if (name.Length == 0)
                throw new InvalidNameException("Node name cannot be empty!");
            name = name.Trim();
            if (name.Length == 0)
                throw new InvalidNameException("Node name cannot consist solely of whitespace!");

            // Force lower if requested
            if (ForceLowercase)
                name = name.ToLower();

            // Make sure it starts with a letter or underscore
            if (!(char.IsLetter(name, 0) || name[0] == '_'))
                throw new InvalidNameException($"Node name starts with invalid character {name[0]}!");

            // Make sure it does not start with "xml" (case-insensitive)
            if (name.StartsWith("xml", StringComparison.OrdinalIgnoreCase))
                throw new InvalidNameException($"Node name cannot start with {name.Substring(0, 3)} (no matter the case)!");

            // Make sure there are no invalid characters
            var invalidCharacters = name.Where(c => !IsValidInNodeName(c));
            if (invalidCharacters.Any())
            {
                var invalidCharacterList = invalidCharacters.Distinct()
                                                            .Select(c => $"'{c}'")
                                                            .AggregateWithJoin(", ");
                throw new InvalidNameException($"Node name contains invalid characters ({invalidCharacterList})!");
            }

            return name;
        }

        /// <summary>
        /// Returns whether the given character is valid for use in a node name.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if valid, false if not.</returns>
        public static bool IsValidInNodeName(char c)
        {
            switch (char.ToLowerInvariant(c))
            {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '_':
                case '.':
                case '-':
                    return true;
                default:
                    return false;
            }
        }
    }
}
