namespace SLExtensions.Xaml.Rtf
{
    using System;
    using System.IO;
    using System.Text;

    using SLExtensions.Text.Parsers;

    /// <summary>
    /// A lexer for RTF
    /// </summary>
    public class RtfLexer : Lexer<RtfLexerState>
    {
        #region Fields

        private static char[] IgnoredChars = new char[] { '\r', '\n', '\t', '\0' };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtfLexer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public RtfLexer(TextReader reader)
            : base(reader)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected override RtfLexerState DefaultState
        {
            get { return RtfLexerState.Default; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns></returns>
        public override IToken NextToken()
        {
            RtfToken token = new RtfToken();

            while (IsInRange(IgnoredChars)) {
                Read();
            }

            int c = Peek();

            if (c == '{') {
                token.Type = RtfTokenType.GroupStart;
                Read();
            }
            else if (c == '}') {
                token.Type = RtfTokenType.GroupEnd;
                Read();
            }
            else if (c == '\\'){
                token = ParseWord();
            }
            else if (c == EOF) {
                token.Type = RtfTokenType.Eof;
            }
            else {
                token = ParseText();
            }

            return token;
        }

        private bool IsAsciiLetter(int c)
        {
            return IsInRange('a', 'z') || IsInRange('A', 'Z');
        }

        private bool IsDigit(int c)
        {
            return IsInRange('0', '9');
        }

        private RtfToken ParseControlWord()
        {
            StringBuilder word = new StringBuilder();
            int? parameter = null;
            bool negative = false;

            int c;

            while ((c = Peek()) != EOF) {
                if (c == ' ') {
                    Read();
                    break;
                }

                if (this.State == RtfLexerState.ControlWordParameter) {
                    if (IsDigit(c)) {
                        parameter = parameter * 10 + (c - '0');
                    }
                    else {
                        break;
                    }
                }
                else {
                    if (IsAsciiLetter(c)) {
                        word.Append((char)c);
                    }
                    else if (IsDigit(c)) {
                        PushState(RtfLexerState.ControlWordParameter);
                        parameter = c - '0';
                    }
                    else if (c == '-') {
                        PushState(RtfLexerState.ControlWordParameter);
                        negative = true;
                    }
                    else {
                        break;
                    }
                }
                Read();
            }

            if (this.State == RtfLexerState.ControlWordParameter) {
                PopState();
            }

            RtfToken token = new RtfToken();
            token.Type = RtfTokenType.ControlWord;
            token.Value = word.ToString();
            if (parameter.HasValue) {
                token.Parameter = negative ? -parameter : parameter;
            }

            return token;
        }

        private RtfToken ParseText()
        {
            StringBuilder text = new StringBuilder();
            text.Append((char)Read());

            while(true) {
                while (IsInRange(IgnoredChars)) {
                    Read();
                }

                int c = Peek();
                if (c == '{' || c == '}' || c == '\\' || c == EOF) {
                    break;
                }

                text.Append((char)Read());
            }

            RtfToken token = new RtfToken();
            token.Type = RtfTokenType.Text;
            token.Value = text.ToString();

            return token;
        }

        private RtfToken ParseWord()
        {
            RtfToken token = new RtfToken();

            Read();     // consume /
            int c = Peek();
            if (IsAsciiLetter(c)) {
                token = ParseControlWord();
            }
            else if (c == '{' || c == '}' || c == '\\') {
                // escaped character
                token.Type = RtfTokenType.Text;
                token.Value = ((char)c).ToString();

                Read();     // consume character
            }
            else {
                token.Type = RtfTokenType.ControlSymbol;
                token.Value = ((char)c).ToString();

                Read();     // consume symbol
            }

            return token;
        }

        #endregion Methods
    }
}