namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
{
    public class TokenList
    {
        private List<Token> Tokens;
        int Pos = 0;

        public TokenList(List<Token> tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Checks for EOF based on position & Tokens length.
        /// </summary>
        /// <returns>bool</returns>
        private bool IsAtEnd()
        {
            return Pos >= Tokens.Count;
        }

        /// <summary>
        /// Moves position forward, returning the next token.
        /// </summary>
        /// <returns>Token</returns>
        public Token Advance()
        {
            if (!IsAtEnd())
            {
                return Tokens[Pos++];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        /// <summary>
        /// Takes an expected token type, compares it to the current token.
        /// If the type is correct, it is consumed. Else throws error.
        /// </summary>
        /// <param name="type">TokenType</param>
        /// <returns>Token</returns>
        /// <exception cref="Exception"></exception>
        public Token Consume(TokenType type)
        {
            if (Peek(0).Type == type)
            {
                return Advance();
            }
            throw new Exception($"Expected token type {type}, but found {Peek(0)}.");
        }

        /// <summary>
        /// Scans the token at a given position, relative to the current position of Pos.
        /// returns the scanned token.
        /// </summary>
        /// <param name="dist">int: the relative position of the token to scan.</param>
        /// <returns>Token</returns>
        public Token Peek(int dist)
        {
            if (Pos + dist < Tokens.Count)
            {
                return Tokens[Pos + dist];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        /// <summary>
        /// Checks the Type of the given token to match on a list of types & Consumes on a match.
        /// Allows for conditional branching if specific optional tokens are found - e.g. 'else' branches in If statements.
        /// </summary>
        /// <param name="types">list of TokenTypes</param>
        /// <returns>bool</returns>
        public bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Peek(0).Type == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
    }
}

