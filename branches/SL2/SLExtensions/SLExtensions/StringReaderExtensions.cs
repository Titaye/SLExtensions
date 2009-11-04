namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public static class StringReaderExtensions
    {
        #region Fields

        private static readonly Regex attributesRegex = new Regex(@"(?<key>\w+)=('(?<value>[^']*)'|""(?<value>[^""]*)""|(?<value>[\w_$]+))", RegexOptions.Multiline);

        private const char gt = '>';
        private const char lt = '<';
        private const char slash = '/';
        private const char space = ' ';

        #endregion Fields

        #region Methods

        public static bool PeekIsMarkup(this StringReader reader)
        {
            int intChar = reader.Peek();
            return intChar != -1 && ((char)intChar) == lt;
        }

        public static string ReadMarkup(this StringReader reader)
        {
            bool b;
            IDictionary<string, string> prms;
            return reader.ReadMarkup(out b, out prms, StringComparer.OrdinalIgnoreCase);
        }

        public static string ReadMarkup(this StringReader reader, out bool isClosingMarkup, out IDictionary<string, string> parameters,  StringComparer comparer)
        {
            isClosingMarkup = false;
            parameters = null;

            if (!reader.PeekIsMarkup())
            {
                return null;
            }

            //Skip Markup
            reader.Read();
            int intChar = reader.Peek();
            if(intChar == -1)
                return null;

            isClosingMarkup = intChar == slash;
            if (isClosingMarkup)
            {
                // Consume /
                reader.Read();

                string content = reader.ReadToBeforeChar(gt);
                // Consume >
                reader.Read();
                return content;
            }
            else
            {
                string content = reader.ReadToBeforeChar(gt);
                // Consume >
                reader.Read();

                StringReader elementReader = new StringReader(content);
                string elementName = elementReader.ReadToBeforeChar(space);
                parameters = attributesRegex.Matches(elementReader.ReadToEnd()).OfType<Match>().ToDictionary(
                    (e) => e.Groups["key"].Value,
                    (e) => e.Groups["value"].Value,
                    comparer);

                return elementName;
            }
        }

        public static string ReadToBeforeChar(this StringReader reader, char charStop)
        {
            StringBuilder sb = new StringBuilder();
            int intChar;
            while((intChar = reader.Peek()) != -1)
            {
                char c = (char) intChar;
                if(c == charStop)
                    return sb.ToString();
                sb.Append(c);
                reader.Read();
            }

            return sb.ToString();
        }

        public static string ReadToMarkup(this StringReader reader)
        {
            return reader.ReadToBeforeChar(lt);
        }

        #endregion Methods
    }
}