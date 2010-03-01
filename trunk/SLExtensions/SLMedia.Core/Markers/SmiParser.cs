namespace SLMedia.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    using SLExtensions;

    using SLMedia.Core;

    public class SmiParser : IMarkerParser
    {
        #region Fields

        private static readonly StringComparer strComp = StringComparer.OrdinalIgnoreCase;

        private const string BRMarkup = "br";
        private const string BodyMarkup = "body";
        private const string CommentMarkup = "!--";
        private const char CurlyBraceClose = '}';
        private const char CurlyBraceOpen = '{';
        private const string HeadMarkup = "head";
        private const string PMarkup = "p";
        private const string SamiMarkup = "sami";
        private const string StyleMarkup = "style";
        private const string SyncMarkup = "sync";

        #endregion Fields

        #region Methods

        public static IDictionary<string, SmiMarker[]> ParseSmiFile(string content)
        {
            content = Regex.Replace(content, @"[\s\r\n]{2,}", " ", RegexOptions.Multiline);
            Dictionary<string, SmiMarker[]> smiContent = new Dictionary<string, SmiMarker[]>(strComp);
            StringReader reader = new StringReader(content);
            reader.ReadToMarkup();

            if (!reader.PeekIsMarkup())
                return smiContent;

            if (strComp.Compare(reader.ReadMarkup(), SamiMarkup) != 0)
                return smiContent;

            Dictionary<string, Dictionary<string, string>> styles = new Dictionary<string, Dictionary<string, string>>(strComp);
            ReadHead(reader, styles);

            return ReadBody(reader, styles);
        }

        public IEnumerable<IMarkerSelector> Parse(string content, IDictionary<string, object> metadata)
        {
            List<IMarkerSelector> selectors = new List<IMarkerSelector>();
            foreach (var item in ParseSmiFile(content))
            {
                var selector = new SmiMarkerSelector { Language = item.Key };
                foreach (var md in metadata)
                {
                    selector.Metadata.Add(md.Key, md.Value);
                }

                selector.Metadata[MarkerMetadata.Language] = item.Key;
                selectors.Add(selector);
                foreach (var mrk in item.Value)
                {
                    selector.Markers.Add(mrk);
                }
            }

            object givenLanguage;
            if (selectors.Count == 1 && metadata.TryGetValue(MarkerMetadata.Language, out givenLanguage))
            {
                selectors[0].Metadata[MarkerMetadata.Language] = givenLanguage;
            }

            return selectors;
        }

        internal static Dictionary<string, SmiMarker[]> ReadBody(StringReader reader, Dictionary<string, Dictionary<string, string>> styles)
        {
            // Be sure to be at the start of a markup tag
            string data = reader.ReadToMarkup();
            if (reader.Peek() == -1
                || !reader.PeekIsMarkup())
                return null;

            // Check if the next tag is Head
            MarkupContent markupContent = null;
            var markupType = reader.ReadMarkup(out markupContent, strComp);
            if (!(markupType == MarkupType.StartNode && strComp.Compare(markupContent.Name, BodyMarkup) == 0))
                return null;

            Dictionary<string, List<SmiMarker>> markersByLanguage = new Dictionary<string, List<SmiMarker>>(strComp);

            while (reader.Peek() != -1)
            {
                if (!reader.PeekIsMarkup())
                    reader.ReadToMarkup();

                if (!reader.PeekIsMarkup())
                    break;

                markupType = reader.ReadMarkup(out markupContent, strComp);

                if (markupType == MarkupType.ClosingNode && strComp.Compare(markupContent.Name, BodyMarkup) == 0)
                    break;

                if (markupType == MarkupType.StartNode
                    && strComp.Compare(markupContent.Name, SyncMarkup) == 0)
                {
                    readSyncNode(reader, markupContent, styles, markersByLanguage);
                }
            }

            var result = new Dictionary<string, SmiMarker[]>(strComp);
            foreach (var item in markersByLanguage)
            {
                var markers = item.Value.OrderBy(m => m.Position).ToList();
                for (int i = markers.Count - 1; i >= 0; i--)
                {
                    SmiMarker mrk = markers[i];
                    if (mrk.Content == null)
                    {
                        markers.RemoveAt(i);
                        if (i > 0)
                        {
                            SmiMarker prevMarker = markers[i - 1];
                            prevMarker.Duration = mrk.Position - prevMarker.Position;
                        }
                    }
                }
                result[item.Key] = markers.ToArray();
            }

            return result;
        }

        internal static void ReadHead(StringReader reader,
            Dictionary<string, Dictionary<string, string>> styles)
        {
            // Be sure to be at the start of a markup tag
            string data = reader.ReadToMarkup();
            if (reader.Peek() == -1
                || !reader.PeekIsMarkup())
                return;

            // Check if the next tag is Head
            MarkupContent markupContent;
            var markupType = reader.ReadMarkup(out markupContent, strComp);
            if (!(markupType == MarkupType.StartNode && strComp.Compare(markupContent.Name, HeadMarkup) == 0))
                return;

            while (reader.Peek() != -1)
            {
                if (!reader.PeekIsMarkup())
                    reader.ReadToMarkup();

                if (!reader.PeekIsMarkup())
                    return;

                markupType = reader.ReadMarkup(out markupContent, strComp);
                if (markupType == MarkupType.ClosingNode && strComp.Compare(markupContent.Name, HeadMarkup) == 0)
                    return;

                if (markupType == MarkupType.StartNode
                    && strComp.Compare(markupContent.Name, StyleMarkup) == 0)
                {
                    var styleContent = reader.ReadToMarkup() ?? string.Empty;
                    if (!reader.PeekIsMarkup())
                        return;

                    while (reader.Peek() != -1)
                    {
                        markupType = reader.ReadMarkup(out markupContent, strComp);
                        if (markupType == MarkupType.Comment)
                            styleContent += markupContent.Content;
                        else if (markupType == MarkupType.ClosingNode)
                            break;

                        reader.ReadToMarkup();
                        //if(!isClosingMarkup && strComp.Compare(markup, StyleMarkup))
                        //    sty
                    }

                    ParseStyle(styleContent, styles);
                }
                //if (reader.PeekIsMarkup())
                //{

                //    if ((isClosingMarkup && strComp.Compare(markup, HeadMarkup) == 0)
                //        || )
                //        return;
                //}
            }
        }

        private static void AssignPropertiesFromStyle(SmiMarker marker, Dictionary<string, string> styleValues)
        {
            if (styleValues != null)
            {
                string val;
                if (styleValues.TryGetValue("name", out val))
                    marker.Name = val;
                if (styleValues.TryGetValue("lang", out val))
                    marker.Language = val;
            }
        }

        private static List<SmiMarker> GetMarkerList(Dictionary<string, List<SmiMarker>> markersByLanguage, string language)
        {
            List<SmiMarker> markers;
            if (!markersByLanguage.TryGetValue(language, out markers))
            {
                markers = new List<SmiMarker>();
                markersByLanguage[language] = markers;
            }

            return markers;
        }

        private static void ParseStyle(string styleContent, Dictionary<string, Dictionary<string, string>> styles)
        {
            StringReader reader = new StringReader(styleContent);
            while (reader.Peek() != -1)
            {
                string names = reader.ReadToBeforeChar(CurlyBraceOpen).Trim();
                // Consume {
                reader.Read();

                string block = reader.ReadToBeforeChar(CurlyBraceClose);
                Dictionary<string, string> styleValues = new Dictionary<string, string>(strComp);
                foreach (var keyValue in block.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var idxColumn = keyValue.IndexOf(':');
                    if (idxColumn == -1)
                        continue;

                    var name = keyValue.Substring(0, idxColumn).Trim();
                    var value = idxColumn + 1 < keyValue.Length ? keyValue.Substring(idxColumn + 1).Trim() : string.Empty;
                    styleValues[name] = value;
                }
                // Consume }
                reader.Read();

                foreach (var n in names.Split(','))
                {
                    var name = n.Trim();
                    Dictionary<string, string> currentValues;
                    if (styles.TryGetValue(n, out currentValues))
                    {
                        foreach (var item in styleValues)
                        {
                            currentValues[item.Key] = item.Value;
                        }
                    }
                    else
                    {
                        styles[n] = styleValues;
                    }
                }
            }
        }

        private static void ReadSyncPNode(StringReader reader, Dictionary<string, Dictionary<string, string>> styles,
            MarkupContent markupContent,
            TimeSpan position, Dictionary<string, List<SmiMarker>> markersByLanguage)
        {
            MarkupType markupType;
            var cssList = new List<string>();
            cssList.Add("p");
            var cssClass = markupContent.Parameters.TryGetValue("class");
            if (!string.IsNullOrEmpty(cssClass))
            {
                cssList.AddRange(from css in cssClass.Split(' ')
                                 select String.Concat("." + css));
            }
            SmiMarker marker = new SmiMarker();
            marker.Position = position;
            StringBuilder markerContent = new StringBuilder();

            foreach (var styleRules in cssList)
            {
                AssignPropertiesFromStyle(marker, styles.TryGetValue(styleRules));
            }

            GetMarkerList(markersByLanguage, marker.Language ?? string.Empty).Add(marker);

            while (reader.Peek() != -1)
            {
                var newContent = reader.ReadToMarkup().Trim();
                if (!string.IsNullOrEmpty(newContent))
                {
                    if (markerContent.Length > 0
                        && markerContent[markerContent.Length - 1] != '\n')
                    {
                        markerContent.Append(' ');
                    }

                    markerContent.Append(newContent);
                }

                if (reader.PeekIsMarkup())
                {
                    markupType = reader.ReadMarkup(out markupContent, strComp);
                    if ((markupType == MarkupType.Node || markupType == MarkupType.StartNode)
                        && strComp.Compare(markupContent.Name, "br") == 0)
                    {
                        markerContent.Append('\n');
                    }

                    if (markupType == MarkupType.ClosingNode
                        && strComp.Compare(markupContent.Name, PMarkup) == 0)
                    {
                        break;
                    }
                }
            }

            if (markerContent.ToString() != "&nbsp;")
                marker.Content = HttpUtility.HtmlDecode(markerContent.ToString()).Trim();
            else
            {
                marker.Content = null;
            }
        }

        private static void readSyncNode(StringReader reader, MarkupContent markupContent, Dictionary<string, Dictionary<string, string>> styles, Dictionary<string, List<SmiMarker>> markersByLanguage)
        {
            MarkupType markupType;

            TimeSpan position = TimeSpan.Zero;
            var start = markupContent.Parameters.TryGetValue("start");
            var startInt = 0;
            if (!int.TryParse(start, out startInt))
                return;

            position = TimeSpan.FromMilliseconds(startInt);

            while (reader.Peek() != -1)
            {
                reader.ReadToMarkup();
                if (!reader.PeekIsMarkup())
                    return;

                markupType = reader.ReadMarkup(out markupContent, strComp);

                if (markupType == MarkupType.ClosingNode
                    && strComp.Compare(markupContent.Name, SyncMarkup) == 0)
                    break;

                if (markupType == MarkupType.StartNode
                    && strComp.Compare(markupContent.Name, PMarkup) == 0)
                {
                    ReadSyncPNode(reader, styles,
                        markupContent,
                        position, markersByLanguage);
                }
            }
        }

        #endregion Methods
    }
}