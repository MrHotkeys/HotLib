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
    }
}
