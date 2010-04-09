namespace SLExtensions.Xaml
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    using SLExtensions.Text;

    /// <summary>
    /// Provides the basic implementation of a converter.
    /// </summary>
    public abstract class ConverterBase
    {
        #region Fields

        private bool treatWarningAsError = false;
        private bool writeDefaultValues = false;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when a converter warning is encountered.
        /// </summary>
        public event EventHandler<ConverterWarningEventArgs> Warning;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to treat warnings as errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if treat warnings as errors; otherwise, <c>false</c>.
        /// </value>
        public bool TreatWarningAsError
        {
            get { return this.treatWarningAsError; }
            set { this.treatWarningAsError = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to write default values.
        /// </summary>
        /// <value><c>true</c> if default values should be written; otherwise, <c>false</c>.</value>
        public bool WriteDefaultValues
        {
            get { return this.writeDefaultValues; }
            set { this.writeDefaultValues = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Converts the data from the input stream to XAML using specified format options.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns></returns>
        public string ToXaml(Stream input)
        {
            StringBuilder output = new StringBuilder();
            XmlWriterSettings settings = CreateXmlWriterSettings();
            using (XmlWriter writer = XmlWriter.Create(output, settings)) {
                ToXaml(input, writer);
            }

            return output.ToString();
        }

        /// <summary>
        /// Converts the data from the input stream to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public abstract void ToXaml(Stream input, XmlWriter output);

        /// <summary>
        /// Writes a string name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, string value)
        {
            if (value.StartsWith("{")) {
                // accolade is control character for binding
                value = "{}" + value;
            }
            output.WriteAttributeString(name, value);
        }

        /// <summary>
        /// Writes a boolean name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, bool value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a short name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, short value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes an int name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, int value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a double name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, double value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a color name/value attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, Color value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a point attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, Point value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a size attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, Size value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a font style attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, FontStyle value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a font weight attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, FontWeight value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Writes a font weight attribute.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected static void WriteAttribute(XmlWriter output, string name, TextDecorationCollection value)
        {
            output.WriteAttributeString(name, value.Format());
        }

        /// <summary>
        /// Creates the XML writer settings.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlWriterSettings CreateXmlWriterSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;

            return settings;
        }

        /// <summary>
        /// Raises the Warning event or ConverterException.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="o">The o.</param>
        protected void OnWarning(string message, params object[] o)
        {
            message = StringUtility.Format(message, o);

            if (treatWarningAsError) {
                throw new ConverterException(message);
            }

            if (this.Warning != null) {
                this.Warning(this, new ConverterWarningEventArgs(message));
            }
        }

        /// <summary>
        /// Writes the end element tag.
        /// </summary>
        /// <param name="output">The output.</param>
        protected void WriteEndElement(XmlWriter output)
        {
            output.WriteEndElement();
        }

        /// <summary>
        /// Writes the start element tag.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="name">The name.</param>
        protected void WriteStartElement(XmlWriter output, string name)
        {
            output.WriteStartElement(name, XamlWriter.NamespaceClient);
        }

        #endregion Methods
    }
}