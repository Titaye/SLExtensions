namespace SLExtensions.Text.Parsers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Provides basic parse functionality.
    /// </summary>
    public abstract class Parser
    {
        #region Fields

        private IToken current;
        private ILexer lexer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Parser"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public Parser(TextReader reader)
        {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }
            this.lexer = CreateLexer(reader);
            this.current = this.lexer.NextToken();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Creates the lexer.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract ILexer CreateLexer(TextReader reader);

        /// <summary>
        /// Reads the next token without consuming it.
        /// </summary>
        /// <returns></returns>
        protected IToken Peek()
        {
            return this.current;
        }

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        protected IToken Read()
        {
            IToken token = this.current;
            this.current = this.lexer.NextToken();
            return token;
        }

        #endregion Methods
    }
}