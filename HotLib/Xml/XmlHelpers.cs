﻿using System;
using System.Xml;

namespace HotLib.Helpers
{
    /// <summary>
    /// Contains static helper methods for working with Xml.
    /// </summary>
    public static class XmlHelpers
    {
        /// <summary>
        /// Gets the path to the file containing an <see cref="XmlNode"/> from the <see cref="Uri"/> property on
        /// an its containing <see cref="XmlDocument"/>. Returns null if the <see cref="Uri"/> is not to a file.
        /// </summary>
        /// <param name="node">The node to get the source file for.</param>
        /// <returns>The source file path, or null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        public static string GetSourceFile(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var uri = new Uri(node.OwnerDocument.BaseURI);
            if (uri.IsFile)
                return uri.LocalPath;
            else
                return null;
        }

        /// <summary>
        /// Finds the root child element that contains the given node.
        /// </summary>
        /// <param name="node">The node to start with.</param>
        /// <returns>The found element.</returns>
        /// <exception cref="ArgumentException">The node is either the document node or the document root node, or it is a document root non-element.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is null.</exception>
        public static XmlElement FindRootChildParent(XmlNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.NodeType == XmlNodeType.Document || node.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new ArgumentException("Given node is the document node or a document root node!");
            }
            if (node.ParentNode.ParentNode.NodeType == XmlNodeType.Document)
            {
                if (node.NodeType == XmlNodeType.Element)
                    return (XmlElement)node;
                else
                    throw new ArgumentException("Given node is a document root child non-element!");
            }
            else
            {
                var current = node;
                do
                {
                    current = current.ParentNode;
                }
                while (current.ParentNode.ParentNode.NodeType != XmlNodeType.Document);
                return (XmlElement)current;
            }
        }
    }
}
