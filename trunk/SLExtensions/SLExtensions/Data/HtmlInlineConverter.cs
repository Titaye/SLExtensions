// Originaly from http://blogs.msdn.com/delay/archive/2007/09/10/bringing-a-bit-of-html-to-silverlight-htmltextblock-makes-rich-text-display-easy.aspx

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
using System.IO;
using System.Windows.Browser;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace SLExtensions.Data
{
    public class HtmlInlineConverter : IValueConverter
    {


        // Constants
        protected const string elementA = "A";
        protected const string elementB = "B";
        protected const string elementBR = "BR";
        protected const string elementEM = "EM";
        protected const string elementI = "I";
        protected const string elementP = "P";
        protected const string elementSTRONG = "STRONG";
        protected const string elementU = "U";

        public HtmlInlineConverter()
        {
        }

        public bool UseDomAsParser { get; set; }

        protected static IList<Inline> ParseAndSetText(string text, bool UseDomAsParser)
        {
            // Save the original text string

            // Try for a valid XHTML representation of text
            var success = false;
            try
            {
                // Try to parse as-is
                return ParseAndSetSpecifiedText(text);
            }
            catch (XmlException)
            {
                // Invalid XHTML
            }

            List<Inline> inlines = new List<Inline>();

            if (!success && UseDomAsParser)
            {
                // Fall back on the browser's parsing engine and some custom code
                // Create some DOM nodes to use
                var document = HtmlPage.Document;
                // An invisible DIV to contain all the custom content
                var wrapper = document.CreateElement("div");
                wrapper.SetStyleAttribute("display", "none");
                // A DIV to contain the input to the code
                var input = document.CreateElement("div");
                input.SetProperty("innerHTML", text);
                // A DIV to contain the output to the code
                var output = document.CreateElement("div");
                // There should be only one BODY element, but this is an easy way to handle 0 or more
                foreach (var bodyObject in document.GetElementsByTagName("body"))
                {
                    var body = bodyObject as HtmlElement;
                    if (null != body)
                    {
                        // Add wrapper element to the DOM
                        body.AppendChild(wrapper);
                        try
                        {
                            // Add input/output elements to the DOM
                            wrapper.AppendChild(input);
                            wrapper.AppendChild(output);
                            // Simple code for browsers where .innerHTML returns ~XHTML (ex: Firefox)
                            var transformationSimple =
                                "(function(){" +
                                    "var input = document.body.lastChild.firstChild;" +
                                    "var output = input.nextSibling;" +
                                    "output.innerHTML = input.innerHTML;" +
                                "})();";
                            // Complex code for browsers where .innerHTML returns content as-is (ex: Internet Explorer)
                            var transformationComplex =
                                "(function(){" +
                                    "function computeInnerXhtml(node, inner) {" +
                                        "if (node.nodeValue) {" +
                                            "return node.nodeValue;" +
                                        "} else if (node.nodeName && (0 < node.nodeName.length)) {" +
                                            "var xhtml = '';" +
                                            "if (node.firstChild) {" +
                                                "if (inner) {" +
                                                    "xhtml += '<' + node.nodeName + '>';" +
                                                "}" +
                                                "var child = node.firstChild;" +
                                                "while (child) {" +
                                                    "xhtml += computeInnerXhtml(child, true);" +
                                                    "child = child.nextSibling;" +
                                                "}" +
                                                "if (inner) {" +
                                                    "xhtml += '</' + node.nodeName + '>';" +
                                                "}" +
                                            "} else {" +
                                                "return ('/' == node.nodeName.charAt(0)) ? ('') : ('<' + node.nodeName + '/>');" +
                                            "}" +
                                            "return xhtml;" +
                                        "}" +
                                    "}" +
                                    "var input = document.body.lastChild.firstChild;" +
                                    "var output = input.nextSibling;" +
                                    "output.innerHTML = computeInnerXhtml(input);" +
                                "})();";
                            // Create a list of code options, ordered simple->complex
                            var transformations = new string[] { transformationSimple, transformationComplex };
                            // Try each code option until one works
                            foreach (var transformation in transformations)
                            {
                                // Create a SCRIPT element to contain the code
                                var script = document.CreateElement("script");
                                script.SetAttribute("type", "text/javascript");
                                script.SetProperty("text", transformation);
                                // Add it to the wrapper element (which runs the code)
                                wrapper.AppendChild(script);
                                // Get the results of the transformation
                                var xhtml = (string)output.GetProperty("innerHTML") ?? "";
                                // Perform some final transformations for the BR element which browsers get wrong
                                xhtml = xhtml.Replace("<br>", "<br/>");  // Firefox
                                xhtml = xhtml.Replace("<BR>", "<BR/>");  // Internet Explorer
                                try
                                {
                                    // Try to parse
                                    inlines.AddRange(ParseAndSetSpecifiedText(xhtml));
                                    success = true;
                                    break;
                                }
                                catch (XmlException)
                                {
                                    // Still invalid XML
                                }
                            }
                        }
                        finally
                        {
                            // Be sure to remove the wrapper we added to the DOM
                            body.RemoveChild(wrapper);
                        }
                        // Processed one BODY; that's enough
                        break;
                    }
                }
            }
            if (!success)
            {
                return null;
            }

            return inlines;
        }

        private static IList<Inline> ParseAndSetSpecifiedText(string text)
        {
            List<Inline> inlines = new List<Inline>();
            // Clear the collection of Inlines

            // Wrap the input in a <div> (so even plain text becomes valid XML)
            using (var stringReader = new StringReader(string.Concat("<div>", text, "</div>")))
            {
                // Read the input
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    // State variables
                    var bold = 0;
                    var italic = 0;
                    var underline = 0;
                    string link = null;
                    var lastElement = elementP;

                    // Read the entire XML DOM...
                    while (xmlReader.Read())
                    {
                        var nameUpper = xmlReader.Name.ToUpper();
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                // Handle the begin element
                                switch (nameUpper)
                                {
                                    case elementA:
                                        link = "";
                                        // Look for the HREF attribute (can't use .MoveToAttribute because it's case-sensitive)
                                        if (xmlReader.MoveToFirstAttribute())
                                        {
                                            do
                                            {
                                                if ("HREF" == xmlReader.Name.ToUpper())
                                                {
                                                    // Store the link target
                                                    link = xmlReader.Value;
                                                    break;
                                                }
                                            } while (xmlReader.MoveToNextAttribute());
                                        }
                                        break;
                                    case elementB:
                                    case elementSTRONG: bold++; break;
                                    case elementI:
                                    case elementEM: italic++; break;
                                    case elementU: underline++; break;
                                    case elementBR: inlines.Add(new LineBreak()); break;
                                    case elementP:
                                        // Avoid double-space for <p/><p/>
                                        if (lastElement != elementP)
                                        {
                                            inlines.Add(new LineBreak());
                                            inlines.Add(new LineBreak());
                                        }
                                        break;
                                }
                                lastElement = nameUpper;
                                break;
                            case XmlNodeType.EndElement:
                                // Handle the end element
                                switch (nameUpper)
                                {
                                    case elementA: link = null; break;
                                    case elementB:
                                    case elementSTRONG: bold--; break;
                                    case elementI:
                                    case elementEM: italic--; break;
                                    case elementU: underline--; break;
                                    case elementBR: inlines.Add(new LineBreak()); break;
                                    case elementP:
                                        inlines.Add(new LineBreak());
                                        inlines.Add(new LineBreak());
                                        break;
                                }
                                lastElement = nameUpper;
                                break;
                            case XmlNodeType.Text:
                            case XmlNodeType.Whitespace:
                                // Create a Run for the visible text
                                // Collapse contiguous whitespace per HTML behavior
                                StringBuilder builder = new StringBuilder(xmlReader.Value.Length);
                                var last = '\0';
                                foreach (char c in xmlReader.Value.Replace('\n', ' '))
                                {
                                    if ((' ' != last) || (' ' != c))
                                    {
                                        builder.Append(c);
                                    }
                                    last = c;
                                }
                                // Trim leading whitespace if following a <P> or <BR> element per HTML behavior
                                var builderString = builder.ToString();
                                if ((elementP == lastElement) || (elementBR == lastElement))
                                {
                                    builderString = builderString.TrimStart();
                                }
                                // If any text remains to display...
                                if (0 < builderString.Length)
                                {
                                    // Create a Run to display it
                                    var run = new Run { Text = builderString };
                                    // Style the Run appropriately
                                    if (0 < bold) run.FontWeight = FontWeights.Bold;
                                    if (0 < italic) run.FontStyle = FontStyles.Italic;
                                    if (0 < underline) run.TextDecorations = System.Windows.TextDecorations.Underline;
                                    if (null != link)
                                    {
                                        // Links get styled and display their HREF since Run doesn't support MouseLeftButton* events
                                        run.TextDecorations = System.Windows.TextDecorations.Underline;
                                        run.Foreground = new SolidColorBrush { Color = Colors.Blue };
                                        run.Text += string.Concat(" <", link, ">");
                                    }
                                    // Add the Run to the collection
                                    inlines.Add(run);
                                    lastElement = null;
                                }
                                break;
                        }
                    }
                }
            }

            return inlines;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string html = value as string;
            if (string.IsNullOrEmpty(html))
                return null;

            //InlineCollection inline = new InlineCollection();
            return ParseAndSetText(html, UseDomAsParser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion



        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        // Using a DependencyProperty as the backing store for Html.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(string), typeof(HtmlInlineConverter), new PropertyMetadata(HtmlChangedCallback));

        private static void HtmlChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = d as TextBlock;
            if (tb != null)
            {
                tb.Inlines.Clear();
                foreach (var item in ParseAndSetText(e.NewValue as string, true))
                {
                    tb.Inlines.Add(item);
                }
            }
        }

    }
}
