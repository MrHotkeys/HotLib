using System;
using System.Xml;
using System.Xml.Linq;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="XmlDocument"/> and <see cref="XmlElement"/>.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Takes the <see cref="XDocument"/> and creates a new <see cref="XmlDocument"/> with matching content.
        /// </summary>
        /// <param name="xDoc">The <see cref="XDocument"/> to convert.</param>
        /// <returns>The created <see cref="XmlDocument"/> object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="xDoc"/> is null.</exception>
        public static XmlDocument CreateXmlDocument(this XDocument xDoc)
        {
            if (xDoc == null)
                throw new ArgumentNullException(nameof(xDoc));

            using (var reader = xDoc.CreateReader())
            {
                var document = new XmlDocument();
                document.Load(reader);
                return document;
            }
        }

        /// <summary>
        /// Tries to get an <see cref="XmlAttribute"/> by name.
        /// </summary>
        /// <param name="attributes">The collection of attributes to attempt to get the attribute from.</param>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="attribute">Will be set to the attribute if found, or null if not.</param>
        /// <returns><see langword="true"/> if the attribute is found, <see langword="false"/> if not.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="attributes"/> or <paramref name="name"/> is null.</exception>
        public static bool TryGetAttribute(this XmlAttributeCollection attributes, string name, out XmlAttribute? attribute)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof(attributes));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            attribute = attributes[name];
            return attribute != null;
        }
    }
}
