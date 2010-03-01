namespace SLExtensions.Text.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Provides basic lexer functionality.
    /// </summary>
    public abstract class Lexer<T> : ILexer
    {
        #region Fields

        /// <summary>
        /// Defines the end of file character.
        /// </summary>
        public const int EOF = -1;

        private TextReader reader;
        private Stack<T> states;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Lexer"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public Lexer(TextReader reader)
        {
            this.reader = reader;
            this.states = new Stack<T>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected abstract T DefaultState
        {
            get;
        }

        /// <summary>
        /// Gets the current state of the lexer.
        /// </summary>
        /// <value>The state.</value>
        protected T State
        {
            get
            {
                if (this.states.Count > 0) {
                    return this.states.Peek();
                }
                return this.DefaultState;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns></returns>
        public abstract IToken NextToken();

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>
        /// 	<c>true</c> if the current character is in given range; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsInRange(char c1, char c2)
        {
            int c = Peek();
            return c >= c1 && c <= c2;
        }

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>
        /// 	<c>true</c> if the current character is in given range; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsInRange(char[] range)
        {
            int c = Peek();
            for (int i = 0; i < range.Length; i++) {
                if (c == range[i]) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="c">The c.</param>
        protected void Match(char c)
        {
            if (Peek() == c) {
                Read();
            }
            else {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void Match(char c, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (Peek() == c) {
                Read();
                i++;
            }
            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the specified string.
        /// </summary>
        /// <param name="s">The s.</param>
        protected void Match(string s)
        {
            for (int i = 0; i < s.Length; i++) {
                if (Peek() == s[i]) {
                    Read();
                }
                else {
                    throw new ParseException("String mismatch");
                }
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="c">The c.</param>
        protected void MatchRange(char[] c)
        {
            if (IsInRange(c)) {
                Read();
            }
            else {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char[] c, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (IsInRange(c)) {
                Read();
                i++;
            }
            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        protected void MatchRange(char c1, char c2)
        {
            if (IsInRange(c1, c2)) {
                Read();
            }
            else {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char c1, char c2, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (IsInRange(c1, c2)) {
                Read();
                i++;
            }
            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Reads the next character without consuming it.
        /// </summary>
        /// <returns></returns>
        protected int Peek()
        {
            return this.reader.Peek();
        }

        /// <summary>
        /// Pops the state.
        /// </summary>
        /// <returns></returns>
        protected T PopState()
        {
            return this.states.Pop();
        }

        /// <summary>
        /// Pushes a new state on the stac.
        /// </summary>
        /// <param name="state">The state.</param>
        protected void PushState(T state)
        {
            this.states.Push(state);
        }

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        /// <returns></returns>
        protected int Read()
        {
            return this.reader.Read();
        }

        private void ValidateOccurence(int count, int minOccurs, int maxOccurs)
        {
            if (count < minOccurs || count > maxOccurs) {
                throw new ParseException("Invalid number of characters");
            }
        }

        #endregion Methods
    }
}