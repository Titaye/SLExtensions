using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Xml;

using SLExtensions.Text.Parsers;

namespace SLExtensions.Xaml.Rtf
{
    /// <summary>
    /// Implements the RTF parser
    /// </summary>
    public class RtfParser
        : Parser
    {
        private static Dictionary<string, object> destinations = CreateDestinations();
        private List<Color> colorTable = new List<Color>();
        private Dictionary<int, string> fontTable = new Dictionary<int, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RtfParser"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public RtfParser(TextReader reader)
            : base(reader)
        {
            this.colorTable.Add(Colors.Black);
            this.Foreground = Colors.Black;
            this.FontFamily = "Portable User Interface";
            this.FontSize = 12 / 1.5;
        }

        /// <summary>
        /// Creates the lexer.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override ILexer CreateLexer(TextReader reader)
        {
            return new RtfLexer(reader);
        }

        /// <summary>
        /// Gets the current font family.
        /// </summary>
        /// <value>The font family.</value>
        public string FontFamily { get; private set; }
        /// <summary>
        /// Gets the current size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize { get; private set; }
        /// <summary>
        /// Gets the current font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight { get; private set; }
        /// <summary>
        /// Gets the current font style.
        /// </summary>
        /// <value>The font style.</value>
        public FontStyle FontStyle { get; private set; }
        /// <summary>
        /// Gets the current foreground.
        /// </summary>
        /// <value>The foreground.</value>
        public Color Foreground { get; private set; }
        /// <summary>
        /// Gets the current text decorations.
        /// </summary>
        /// <value>The text decorations.</value>
        public TextDecorationCollection TextDecorations { get; private set; }
        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns></returns>
        public RtfToken NextToken()
        {
            RtfToken token;
            do {
                token = (RtfToken)Read();

                if (token.Type == RtfTokenType.ControlSymbol) {
                    if (token.Value == "*") {
                        SkipGroup();
                    }
                }
                else if (token.Type == RtfTokenType.ControlWord) {
                    if (token.Value == "colortbl") {
                        ParseColorTable();
                    }
                    else if (token.Value == "fonttbl") {
                        ParseFontTable();
                    }
                    else if (destinations.ContainsKey(token.Value)) {
                        SkipGroup();
                    }
                    else if (token.Value == "cf") {
                        int index = token.Parameter ?? 0;
                        if (index < this.colorTable.Count) {
                            this.Foreground = this.colorTable[token.Parameter.Value];
                        }
                    }
                    else if (token.Value == "f") {
                        int index = token.Parameter ?? 0;
                        string fontFamily;
                        if (this.fontTable.TryGetValue(index, out fontFamily)){
                            this.FontFamily = fontFamily;
                        }
                    }
                    else if (token.Value == "fs") {
                        if (token.Parameter.HasValue) {
                            this.FontSize = token.Parameter.Value / 1.5;
                        }
                    }
                    else if (token.Value == "b") {
                        this.FontWeight = token.Parameter == 0 ? FontWeights.Normal : FontWeights.Bold;
                    }
                    else if (token.Value == "i") {
                        this.FontStyle = token.Parameter == 0 ? FontStyles.Normal : FontStyles.Italic;
                    }
                    else if (token.Value == "ul") {
                        this.TextDecorations = token.Parameter == 0 ? null : System.Windows.TextDecorations.Underline;
                    }
                    else if (token.Value == "plain") {
                        ResetFont();
                    }
                    else if (token.Value == "par") {
                        break;
                    }
                }
                else if (token.Type == RtfTokenType.Text) {
                    break;
                }
            }
            while (!token.IsEof);
            
            return token;
        }

        private void ResetFont()
        {
            this.FontSize = 12;
            this.FontStyle = FontStyles.Normal;
            this.FontWeight = FontWeights.Normal;
            this.TextDecorations = null;
        }

        private void ParseColorTable()
        {
            RtfToken token;
            do {
                token = (RtfToken)Read();

                if (token.Type == RtfTokenType.ControlWord && token.Value == "red") {
                    RtfToken tokenG = Match(RtfTokenType.ControlWord, "green");
                    RtfToken tokenB = Match(RtfTokenType.ControlWord, "blue");
                    Match(RtfTokenType.Text, ";");

                    this.colorTable.Add(Color.FromArgb(0xff, (byte)token.Parameter, (byte)tokenG.Parameter, (byte)tokenB.Parameter));
                }
            }
            while (!token.IsEof && token.Type != RtfTokenType.GroupEnd);
        }

        private void ParseFontTable()
        {
            int groups = 1;
            RtfToken token;
            do {
                token = (RtfToken)Read();
                if (token.Type == RtfTokenType.GroupStart) {
                    groups++;
                }
                else if (token.Type == RtfTokenType.GroupEnd) {
                    groups--;
                }
                else if (token.Type == RtfTokenType.ControlWord && token.Value == "f") {
                    int index = token.Parameter.Value;
                    bool isDefault = false;

                    while (token.Type != RtfTokenType.Text && !token.IsEof) {
                        token = (RtfToken)Read();

                        if (token.Type == RtfTokenType.ControlWord && token.Value == "fnil") {
                            isDefault = true;
                        }
                    }
                    string fontFamily = token.Value;
                    if (fontFamily.EndsWith(";")) {
                        fontFamily = fontFamily.Substring(0, fontFamily.Length - 1);
                    }
                    this.fontTable[index] = fontFamily;

                    if (isDefault) {
                        this.FontFamily = fontFamily;
                    }
                }
            }
            while (!token.IsEof && groups > 0);
        }

        private void SkipGroup()
        {
            int groups = 1;
            RtfToken token;
            do {
                token = (RtfToken)Read();
                if (token.Type == RtfTokenType.GroupStart) {
                    groups++;
                }
                else if (token.Type == RtfTokenType.GroupEnd) {
                    groups--;
                }
            }
            while (!token.IsEof && groups > 0);
        }

        private RtfToken Match(RtfTokenType type)
        {
            RtfToken token = (RtfToken)Read();
            if (token.Type != type) {
                throw new ParseException("Unexpected token");
            }

            return token;
        }

        private RtfToken Match(RtfTokenType type, string value)
        {
            RtfToken token = (RtfToken)Read();
            if (token.Type != type || token.Value != value) {
                throw new ParseException("Unexpected token");
            }

            return token;
        }

        private static Dictionary<string, object> CreateDestinations()
        {
            Dictionary<string, object> destinations = new Dictionary<string, object>();
            destinations["info"] = null;
            destinations["stylesheet"] = null;

            return destinations;
        }
    }
}
