namespace SLExtensions
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Xml.Linq;

    public static class XElementExtensions
    {
        #region Methods

        public static string GetAttribute(this XElement elem, XName attributeName)
        {
            XAttribute attribute = elem.Attribute(attributeName);
            if (attribute != null)
                return attribute.Value;
            return null;
        }

        public static string GetAttribute(this XContainer elem, XName elementName, XName attributeName)
        {
            XElement childElement = elem.Element(elementName);
            if (childElement == null)
                return null;

            XAttribute attribute = childElement.Attribute(attributeName);
            if (attribute != null)
                return attribute.Value;
            return null;
        }

        public static bool? GetBoolAttribute(this XElement elem, XName attributeName)
        {
            string val = GetAttribute(elem, attributeName);
            bool result;
            if (bool.TryParse(val, out result))
                return result;
            return null;
        }

        public static bool? GetBoolAttribute(this XElement elem, XName elementName, XName attributeName)
        {
            string val = GetAttribute(elem, elementName, attributeName);
            bool result;
            if (bool.TryParse(val, out result))
                return result;
            return null;
        }

        public static double? GetDoubleAttribute(this XElement elem, XName attributeName)
        {
            string val = GetAttribute(elem, attributeName);
            double result;
            if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            return null;
        }

        public static double? GetDoubleAttribute(this XElement elem, XName elementName, XName attributeName)
        {
            string val = GetAttribute(elem, elementName, attributeName);
            double result;
            if (double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                return result;
            return null;
        }

        public static string GetElementValue(this XContainer elem, XName elementName)
        {
            XElement childElement = elem.Element(elementName);
            if (childElement != null)
                return childElement.Value;
            return null;
        }

        public static int? GetIntAttribute(this XElement elem, XName attributeName)
        {
            string val = GetAttribute(elem, attributeName);
            int result;
            if (int.TryParse(val, out result))
                return result;
            return null;
        }

        public static int? GetIntAttribute(this XContainer elem, XName elementName, XName attributeName)
        {
            string val = GetAttribute(elem, elementName, attributeName);
            int result;
            if (int.TryParse(val, out result))
                return result;
            return null;
        }

        public static TimeSpan? GetTimeSpanAttribute(this XElement elem, XName attributeName)
        {
            string val = GetAttribute(elem, attributeName);
            TimeSpan result;
            if (TimeSpan.TryParse(val, out result))
                return result;
            return null;
        }

        public static TimeSpan? GetTimeSpanAttribute(this XContainer elem, XName elementName, XName attributeName)
        {
            string val = GetAttribute(elem, elementName, attributeName);
            TimeSpan result;
            if (TimeSpan.TryParse(val, out result))
                return result;
            return null;
        }

        #endregion Methods
    }
}