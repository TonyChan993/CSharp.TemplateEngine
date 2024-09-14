using System;
using System.Collections.Generic;

namespace CSharp.TemplateEngine
{
	// A state function that returns a state function.
	// In this way, one state passes directly to the next instead of returning to a switch in which the next one is recalculated based on the previous one.
    delegate LexStateFunc LexStateFunc(LexerBase l);

    /// <summary>
    /// Divide input en tokens.
    /// Inspired by Rob Pike's talk on Go: http://rspace.googlecode.com/hg/slide/lex.html
    /// </summary>
    sealed class LexerBase
    {
        public const char EOF = '\0';
        public string Input; // the string being scanned
        public int Start;
        public int Pos;
		public int Line = 1;
        public List<Token> Tokens { get; set; } // nstead of the channel, accumulate the scanned items here.

        public LexerBase(string input)
        {
            this.Input = input;
            this.Tokens = new List<Token>();
        }

        public int Width
        {
            get { return this.Pos - this.Start; }
		}

		// what it has ahead to process
		public string Ahead
		{
			get { return this.Input.Substring(this.Pos); }
		}

        public void Run(LexStateFunc initialState)
        {
            LexStateFunc state = initialState;

            while (state != null)
            {
                state = state(this);
            }
        }

        /// <summary>
        /// Generate a new token and move forward.
        /// </summary>
		public Token Emit(TokenType t)
        {
            var token = new Token(t, this.Start, this.Input.Substring(this.Start, this.Pos - this.Start));
            this.Tokens.Add(token);
            this.Start = this.Pos;

			if(Template.Debug)
			{
				Console.WriteLine("{0}. {1} -> {2}", this.Tokens.IndexOf(token), token.Type, token.Value);
			}

			return token;
        }

        /// <summary>
        /// Advance one character
        /// </summary>
        public char Next()
        {
            if (this.Pos >= this.Input.Length)
            {
                return EOF;
            }

            var c = this.Input[this.Pos];
            this.Pos++;

			if (c == '\n')
			{
				this.Line++;
			}

            return c;
        }

        public void Ignore()
        {
            this.Start = this.Pos;
        }

        public void Backup()
		{
			if (this.Peek() == '\n')
			{
				this.Line--;
			}

            this.Pos--;
        }

        public char Peek()
        {
            return this.Peek(0);
        }

        public char Peek(int index)
        {
            var position = this.Pos + index;
            if (this.Input.Length <= position)
            {
                return EOF;
            }
            return this.Input[position];
        }

        // Return whether the next characters are equal to prefix.
        public bool HasPrefix(string prefix)
        {
            return StartsWith(ref this.Input, this.Pos, prefix);
        }

		/// <summary>
		/// Check if input starts with value from position start.
		/// </summary>
		static bool StartsWith(ref string input, int start, string value)
		{
			if (input.Length < start + value.Length)
			{
				return false;
			}

			for (int i = 0, l = value.Length; i < l; i++)
			{
				if (input[start + i] != value[i])
				{
					return false;
				}
			}

			return true;
		}

        // If the following exists, consume the entire word.
        public bool AcceptWord(string word)
        {
            if (this.HasPrefix(word))
            {
                this.Pos += word.Length;
                return true;
            }

            return false;
        }

        // Consume the next character if it is within the valid set.
        public bool Accept(string valid)
        {
            var c = this.Next();
            if (c == EOF)
            {
                return false;
            }

            this.Backup();
            return valid.IndexOf(c) >= 0;
        }

        // Consume the next characters if they are within the valid set.
        public void AcceptAnyChar(string valid)
        {
            while (true)
            {
                var c = this.Next();

                if (c == EOF)
                {
                    return;
                }
                else if (valid.IndexOf(c) == -1)
                {
                    this.Backup();
                    return;
                }
            }
        }

		public override string ToString()
		{
			return this.Input.Substring(this.Start, this.Pos - this.Start);
		}
    }
}
