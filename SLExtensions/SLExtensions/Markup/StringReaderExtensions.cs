namespace SLExtensions.Markup
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

        private const char gt = '>';
        private const char lt = '<';
        private const char slash = '/';
        private const char space = ' ';

        private static readonly Regex attributesRegex = new Regex(@"(?<key>\w+)=('(?<value>[^']*)'|""(?<value>[^""]*)""|(?<value>[\w_$]+))", RegexOptions.Multiline);

        #endregion Fields

        #region Methods

        public static bool PeekIsMarkup(this StringReader reader)
        {
            int intChar = reader.Peek();
            return intChar != -1 && ((char)intChar) == lt;
        }

        public static string ReadMarkup(this StringReader reader)
        {
            MarkupContent content;
            reader.ReadMarkup(out content, StringComparer.OrdinalIgnoreCase);
            if (content != null)
                return content.Name;

            return null;
        }

        public static MarkupType ReadMarkup(this StringReader reader, out MarkupContent content, StringComparer comparer)
        {
            content = null;

            if (!reader.PeekIsMarkup())
            {
                if (content != null)
                    content.Type = MarkupType.Raw;
                return MarkupType.Raw;
            }

            //Skip Markup
            reader.Read();
            int intChar = reader.Peek();
            if (intChar == -1)
            {
                if (content != null)
                    content.Type = MarkupType.Raw;
                return MarkupType.Raw;
            }

            var isClosingMarkup = intChar == slash;
            if (isClosingMarkup)
            {
                // Consume /
                reader.Read();

                var name = reader.ReadToBeforeChar(gt);
                content = new MarkupContent { Name = name };
                // Consume >
                reader.Read();
                content.Type = MarkupType.ClosingNode;
                return content.Type;
            }
            else
            {
                bool isComment = false;

                StringBuilder data = new StringBuilder();
                data.Append(reader.ReadToBeforeChar(gt));
                if (data.Length > 3 && data.ToString(0, 3) == "!--")
                {
                    isComment = true;

                    while (data.ToString(data.Length - 2, 2) != "--")
                    {
                        char[] cBuffer = new char[1];
                        if (reader.Read(cBuffer, 0, 1) == 0)
                            break;

                        data.Append(cBuffer[0]);
                        if (reader.Peek() != -1)
                            data.Append(reader.ReadToBeforeChar(gt));
                    }
                }
                // Consume >
                reader.Read();
                if (isComment)
                {
                    content = new MarkupContent { Content = data.ToString(3, data.Length - 5).Trim(), Name = "!--" };
                    content.Type = MarkupType.Comment;
                    return content.Type;
                }
                else
                {
                    bool isClosed = false;
                    if(data[data.Length - 1] == slash)
                    {
                        data.Remove(data.Length - 1, 1);
                        isClosed = true;
                    }

                    content = new MarkupContent { Content = data.ToString() };
                    StringReader elementReader = new StringReader(content.Content);
                    string elementName = elementReader.ReadToBeforeChar(space);
                    content.Name = elementName;

                    content.Parameters = attributesRegex.Matches(elementReader.ReadToEnd()).OfType<Match>().ToDictionary(
                        (e) => e.Groups["key"].Value,
                        (e) => e.Groups["value"].Value,
                        comparer);
                    content.Type = isClosed ? MarkupType.Node : MarkupType.StartNode;
                    return content.Type;
                }
            }
        }

        public static string ReadToBeforeChar(this StringReader reader, char charStop)
        {
            StringBuilder sb = new StringBuilder();
            int intChar;
            while ((intChar = reader.Peek()) != -1)
            {
                char c = (char)intChar;
                if (c == charStop)
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
