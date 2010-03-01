namespace SLExtensions
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml;
    using System.Xml.Linq;

    public static class XmlReaderExtensions
    {
        #region Methods

        public static XNode GetXNode(this XmlReader reader)
        {
            return XDocument.ReadFrom(reader);
        }

        #endregion Methods
    }
}