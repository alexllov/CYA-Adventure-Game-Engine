namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
{
    public class Tokenizer
    {
        // Unprocessed source codes.
        private readonly List<Source> Sources;

        // Source currently being processed.
        private Source ActiveSource;

        // Start is the start of the current token,
        // Pos is the current position in the source code.
        private int Start = 0, Pos = 0;
        // Pointer pos, used for debugging.
        private int Line = 1, Col = 1;

        // The Actual Tokens List.
        public List<Token> Tokens = [];

        // Dict of known Keywords.
        Dictionary<string, TokenType> Keywords;

        /// <summary>
        /// Tokenier constructor. Takes filepath to use as source code.
        /// Tokenizes the source code and stores it in the Tokens list.
        /// </summary>
        /// <param name="filepath">file path of game code</param>
        public Tokenizer(List<Source> sources, Dictionary<string, TokenType> keywords)
        {
            Sources = sources;
            Keywords = keywords;

        }
        public void Show()
        {
            Console.WriteLine("Tokens:");
            Console.WriteLine($"Len Tokens: {Tokens.Count}");
            foreach (var token in Tokens)
            {
                Console.WriteLine(token);
            }
        }

        /// <summary>
        /// Tokenizes the source code, returning a list of tokens.
        /// </summary>
        /// <returns>List<Token> obj containing processed game code.</returns>
        public TokenList Tokenize()
        {
            foreach (Source sourcefile in Sources)
            {
                ActiveSource = sourcefile;
                Start = 0;
                Pos = 0;
                Line = 1;
                Col = 1;
                while (!IsAtEnd())
                {
                    Start = Pos;
                    ScanToken();
                }
            }
            Tokens.Add(new Token(TokenType.EOF, "", Line, Col, ActiveSource.RelativePath));
            return new TokenList(Tokens);
        }

        /// <summary>
        /// Large match-case statement that scans the next token to identify its type.
        /// </summary>
        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '#':
                    while (!IsAtEnd() && ActiveSource.Content[Pos] != '\n')
                    {
                        Advance();
                    }
                    break;

                // Skip whitespace.
                case ' ':
                case '\r':
                case '\t':
                    Col++;
                    break;

                // New Line.
                case '\n':
                    Line++;
                    Col = 1;
                    break;

                // Braces.
                case '[':
                    AddToken(TokenType.LBracket);
                    break;
                case ']':
                    AddToken(TokenType.RBracket);
                    break;
                case '(':
                    AddToken(TokenType.LParent);
                    break;
                case ')':
                    AddToken(TokenType.RParent);
                    break;
                case '{':
                    AddToken(TokenType.LCurly);
                    break;
                case '}':
                    AddToken(TokenType.RCurly);
                    break;

                // Dot, Comma.
                case '.':
                    AddToken(TokenType.Dot);
                    break;
                case ',':
                    AddToken(TokenType.Comma);
                    break;

                // Pipes.
                case '|':
                    AddToken(TokenType.Pipe);
                    break;

                // Colon.
                case ':':
                    AddToken(TokenType.Colon);
                    break;

                // Operators & GoTo.
                case '-':
                    if (Match('>'))
                    {
                        AddToken(TokenType.GoTo);
                    }
                    else
                    {
                        AddToken(TokenType.Minus);
                    }
                    break;
                case '+':
                    AddToken(TokenType.Plus);
                    break;
                case '*':
                    AddToken(TokenType.Multiply);
                    break;
                case '/':
                    AddToken(TokenType.Divide);
                    break;
                case '=':
                    if (Match('='))
                    {
                        AddToken(TokenType.Equal);
                    }
                    else
                    {
                        AddToken(TokenType.Assign);
                    }
                    break;
                case '!':
                    if (Match('='))
                    {
                        AddToken(TokenType.NotEqual);
                    }
                    else
                    {
                        AddToken(TokenType.Not);
                    }
                    break;
                case '<':
                    if (Match('='))
                    {
                        AddToken(TokenType.LessEqual);
                    }
                    else
                    {
                        AddToken(TokenType.LessThan);
                    }
                    break;
                case '>':
                    if (Match('='))
                    {
                        AddToken(TokenType.GreaterEqual);
                    }
                    else
                    {
                        AddToken(TokenType.GreaterThan);
                    }
                    break;

                // String Literal.
                case '"':
                    ReadString();
                    break;

                default:
                    if (char.IsDigit(c))
                    {
                        ReadNumber();
                    }
                    else if (char.IsLetterOrDigit(c))
                    {
                        ReadIdentifier();
                    }
                    else
                    {
                        throw new Exception($"Unexpected character: `{c}` on line {Line}, in file {ActiveSource.RelativePath}");
                    }
                    break;
            }
        }

        /// <summary>
        /// Moves the current position forward by one character.
        /// </summary>
        /// <returns>Character at new position.</returns>
        private char Advance()
        {
            Pos++;
            Col++;
            return ActiveSource.Content[Pos - 1];
        }

        /// <summary>
        /// Bool, is current pos the EOF.
        /// </summary>
        /// <returns>Bool</returns>
        private bool IsAtEnd()
        {
            return Pos >= ActiveSource.Content.Length;
        }

        /// <summary>
        /// Returns char at the position "distance" from the current position w/o advancing position.
        /// If EOF, returns '\0' identifier.
        /// </summary>
        /// <param name="dist">integer, gives distance relative to current token to observe.</param>
        /// <returns></returns>
        private char Peek(int dist)
        {
            if (Pos + dist >= ActiveSource.Content.Length)
            {
                return '\0';
            }
            return ActiveSource.Content[Pos + dist];
        }

        /// <summary>
        /// IDs the next char, and advances position, "consuming" it, if it matches.
        /// Used to identify 2-character tokens like "->".
        /// </summary>
        /// <param name="expected">Takes a char to match on</param>
        /// <returns>Bool</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd() || ActiveSource.Content[Pos] != expected)
            {
                return false;
            }
            else
            {
                Pos++;
                return true;
            }
        }

        // TODO: Add {} support to id identifiers to create $strings, eg "hello {name}".
        /// <summary>
        /// Reads String literals, id'd by ""s
        /// </summary>
        private void ReadString()
        {
            int startLine = Line;
            while (Peek(0) != '"' && !IsAtEnd())
            {
                if (Peek(0) == '\n')
                {
                    Line++;
                    Col = 1;
                    Advance();
                }
                else
                {
                    Advance();
                }
            }

            if (IsAtEnd())
            {
                throw new Exception($"Unterminated string starting on line {startLine}.");
            }

            // Consume closing quote.
            Advance();

            // Get string val & create token custom way as start & end need moving due to ""s.
            string val = ActiveSource.Content[(Start + 1)..(Pos - 1)];
            Tokens.Add(new Token(TokenType.String, val, Line, Col, ActiveSource.RelativePath));

        }

        /// <summary>
        /// Finds numbers, inc decimals.
        /// </summary>
        private void ReadNumber()
        {
            while (char.IsDigit(Peek(0)))
            {
                Advance();
            }
            while (Peek(0) == '.' && char.IsDigit(Peek(1)))
            {
                Advance();
                while (char.IsDigit(Peek(0)))
                {
                    Advance();
                }
            }
            AddToken(TokenType.Number);
        }

        /// <summary>
        /// Finds var names and keywords.
        /// </summary>
        private void ReadIdentifier()
        {
            while (char.IsLetterOrDigit(Peek(0)) || Peek(0) == '_')
            {
                Advance();
            }
            string text = ActiveSource.Content[Start..Pos];
            TokenType type = Keywords.TryGetValue(text, out TokenType value) ? value : TokenType.Identifier;
            AddToken(type);
        }

        /// <summary>
        /// Creates a token of the specified type with the current source substring.
        /// </summary>
        /// <param>TokenType</param>
        private void AddToken(TokenType type)
        {
            string text = ActiveSource.Content[Start..Pos];
            Tokens.Add(new Token(type, text, Line, Col, ActiveSource.RelativePath));
        }
    }
}