namespace SLExtensions.Xaml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Represents a base converter capable of converting string input into XAML.
    /// </summary>
    public abstract class StringConverterBase : ConverterBase
    {
        #region Methods

        /// <summary>
        /// Converts the data from the input string to XAML using specified format options.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns></returns>
        public string ToXaml(string input)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = CreateXmlWriterSettings();
            using (XmlWriter writer = XmlWriter.Create(output, settings)) {
                using (StringReader reader = new StringReader(input)) {
                    ToXaml(reader, writer);
                }
            }

            return output.ToString();
        }

        /// <summary>
        /// Converts the data from the input stream to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public override void ToXaml(Stream input, XmlWriter output)
        {
            using (StreamReader reader = new StreamReader(input)) {
                ToXaml(reader, output);
            }
        }

        /// <summary>
        /// Converts the data from the input string to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public abstract void ToXaml(TextReader input, XmlWriter output);

        #endregion Methods
    }
}