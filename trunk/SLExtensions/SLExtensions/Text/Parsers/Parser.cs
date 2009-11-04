using System;
using System.IO;
using System.Text;
using System.Xml;

namespace SLExtensions.Text.Parsers
{
    /// <summary>
    /// Provides basic parse functionality.
    /// </summary>
    public abstract class Parser
    {
        private ILexer lexer;
        private IToken current;

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

        /// <summary>
        /// Creates the lexer.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract ILexer CreateLexer(TextReader reader);
    }
}
