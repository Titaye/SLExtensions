using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace SLExtensions.Xaml.Rtf
{
    /// <summary>
    /// Converts Rich Text Format streams to XAML.
    /// </summary>
    public class RtfConverter
        : StringConverterBase
    {
        /// <summary>
        /// Converts the data from the input string to XAML and writes the output to specified writer.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        public override void ToXaml(TextReader input, XmlWriter output)
        {
            WriteStartElement(output, "TextBlock");
            WriteAttribute(output, "TextWrapping", "Wrap");
            WriteAttribute(output, "FontFamily", "Courier New");

            RtfParser parser = new RtfParser(input);
            RtfToken token;

            do {
                token = parser.NextToken();

                if (token.Type == RtfTokenType.Text) {
                    WriteStartElement(output, "Run");
                    WriteAttribute(output, "FontFamily", parser.FontFamily);
                    WriteAttribute(output, "FontSize", parser.FontSize);
                    WriteAttribute(output, "FontStyle", parser.FontStyle);
                    WriteAttribute(output, "FontWeight", parser.FontWeight);
                    WriteAttribute(output, "Foreground", parser.Foreground);
                    if (parser.TextDecorations != null){
                        WriteAttribute(output, "TextDecorations", parser.TextDecorations);
                    }
                    WriteAttribute(output, "Text", token.Value);
                    WriteEndElement(output);
                }
                else if (token.Type == RtfTokenType.ControlWord && token.Value == "par") {
                    WriteStartElement(output, "LineBreak");
                    WriteEndElement(output);
                }
            }
            while (!token.IsEof);

            WriteEndElement(output);
        }

        /// <summary>
        /// Creates the XML writer settings.
        /// </summary>
        /// <returns></returns>
        protected override XmlWriterSettings CreateXmlWriterSettings()
        {
            XmlWriterSettings settings = base.CreateXmlWriterSettings();
            settings.Indent = false;

            return settings;
        }
    }
}
