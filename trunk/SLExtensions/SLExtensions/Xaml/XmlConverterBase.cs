using System;
using System.IO;
using System.Xml;

namespace SLExtensions.Xaml
{
    /// <summary>
    /// Represents a base converter capable of converting XML input into XAML.
    /// </summary>
    public abstract class XmlConverterBase
        : StringConverterBase
    {
        /// <summary>
        /// Converts the data from the input stream to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public override sealed void ToXaml(Stream input, XmlWriter output)
        {
            XmlReaderSettings settings = CreateXmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(input, settings)) {
                ToXaml(reader, output);
            }
        }

        /// <summary>
        /// Converts the data from the input string to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public override sealed void ToXaml(TextReader input, XmlWriter output)
        {
            XmlReaderSettings settings = CreateXmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(input, settings)) {
                ToXaml(reader, output);
            }
        }

        /// <summary>
        /// Converts the data from the XML reader to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public abstract void ToXaml(XmlReader input, XmlWriter output);

        /// <summary>
        /// Creates the XML reader settings.
        /// </summary>
        /// <returns></returns>
        protected static XmlReaderSettings CreateXmlReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;

            return settings;
        }
    }
}
