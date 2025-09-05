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

        /*
         * The following function, Peek() is based conceptually upon the Peek() function implemented by
         * Nystrom, R. in Chapter 6 of "Crafting Interpreters", but modified in function as it takes an integer argument
         * that allows for the token being 'peeked' to be at a position relative to the current position of Pos.
         * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
         * 
         * Nystrom, R. (2019) Parsing Expressions. Available at: https://craftinginterpreters.com/parsing-expressions.html (Accessed 5 September 2025)
         */
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
            return new Token(TokenType.EOF, "", -1, -1, "");
        }

        /*
         * The following function, Advance() is a modified version of the advance() function implemented by
         * Nystrom, R. in Chapter 6 of "Crafting Interpreters".
         * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
         * 
         * Nystrom, R. (2019) Parsing Expressions. Available at: https://craftinginterpreters.com/parsing-expressions.html (Accessed 5 September 2025)
         */
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
            return new Token(TokenType.EOF, "", -1, -1, "");
        }

        /*
         * The following function, IsAtEnd() is a C# implementation of the isAtEnd() function implemented by
         * Nystrom, R. in Chapter 4 of "Crafting Interpreters".
         * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
         * 
         * Nystrom, R. (2019) Scanning. Available at: https://craftinginterpreters.com/scanning.html (Accessed 5 September 2025)
         */
        /// <summary>
        /// Checks for EOF based on position & Tokens length.
        /// </summary>
        /// <returns>bool</returns>
        private bool IsAtEnd()
        {
            return Pos >= Tokens.Count;
        }

        /*
         * The following function, Consume() is a C# implementation of the consume() function implemented by
         * Nystrom, R. in Chapter 6 of "Crafting Interpreters".
         * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
         * 
         * Nystrom, R. (2019) Parsing Expressions. Available at: https://craftinginterpreters.com/parsing-expressions.html (Accessed 5 September 2025)
         */
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

        /*
         * The following function, Match() is a C# implementation of the match() function implemented by
         * Nystrom, R. in Chapter 6 of "Crafting Interpreters".
         * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
         * 
         * Nystrom, R. (2019) Parsing Expressions. Available at: https://craftinginterpreters.com/parsing-expressions.html (Accessed 5 September 2025)
         */
        /// <summary>
        /// Checks the Type of the given token to match on a list of types & Consumes on a match.
        /// Allows for conditional branching if specific optional tokens are found - e.g. 'else' branches in If statements.
        /// </summary>
        /// <param name="types">list of TokenTypes</param>
        /// <returns>bool</returns>
        // Using params here allowd for calls to just pass in a list of TokenTypes, rather than an array.
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

        /// <summary>
        /// Checks if the current token is of tokentype Identifier
        /// && that its lexeme matches the value passed.
        /// </summary>
        /// <param name="targetLexeme"></param>
        /// <returns></returns>
        public bool IdentLexemeMatch(string targetLexeme)
        {
            Token token = Peek(0);
            if (token.Type is TokenType.Identifier && token.Lexeme == targetLexeme)
            {
                Advance();
                return true;
            }
            return false;
        }
    }
}

