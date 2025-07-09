using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    internal class Tokenizer
    {
        // Unprocessed source code as str.
        private string Source;
        // Start is the start of the current token,
        // Pos is the current position in the source code.
        private int Start = 0, Pos = 0;
        // Pointer pos, used for debugging.
        private int Line = 1, Col = 1;
        private static Dictionary<string, TokenType> Keywords = new()
        {
            {"import", TokenType.Import },
            {"as", TokenType.As },
            {"->", TokenType.GoTo },
            {"scene", TokenType.Scene },
            {"table", TokenType.Table },
            {"code", TokenType.Code },
            {"end", TokenType.End },
            {"if", TokenType.If },
            {"then", TokenType.Then },
            {"else", TokenType.Else },
            {"while", TokenType.While },
            {"and", TokenType.And },
            {"or", TokenType.Or },
        };
        
        // The Actual Tokens List.
        public List<Token> Tokens = new();

        /// <summary>
        /// Tokenier constructor. Takes filepath to use as source code.
        /// Tokenizes the source code and stores it in the Tokens list.
        /// </summary>
        /// <param name="filepath">file path of game code</param>
        public Tokenizer(string filepath)
        {
            Source = File.ReadAllText(filepath);
        }
        public void Show()
        {
            Console.WriteLine("Tokens:");
            Console.WriteLine($"Len Tokens: {Tokens.Count()}");
            foreach (var token in Tokens)
            {
                Console.WriteLine(token);
            }
        }

        /// <summary>
        /// Tokenizes the source code, returning a list of tokens.
        /// </summary>
        /// <returns>List<Token> obj containing processed game code.</returns>
        public List<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                Start = Pos;
                ScanToken();
            }
            Tokens.Add(new Token(TokenType.EOF, "", Line, Col));
            return Tokens;
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
                    while (!IsAtEnd() && Source[Pos] != '\n')
                    {
                        Advance();
                    }
                    // Removed Adding comments s.t. they are discarded immediately & never seen at parsing.
                    //AddToken(TokenType.Comment);
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
                        throw Error($"Unexpected character: `{c}`");
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
            return Source[Pos - 1];
        }

        /// <summary>
        /// Bool, is current pos the EOF.
        /// </summary>
        /// <returns>Bool</returns>
        private bool IsAtEnd()
        {
            return Pos >= Source.Length;
        }

        /// <summary>
        /// Returns char at the position "distance" from the current position w/o advancing position.
        /// If EOF, returns '\0' identifier.
        /// </summary>
        /// <param name="dist">integer, gives distance relative to current token to observe.</param>
        /// <returns></returns>
        private char Peek(int dist)
        {
            if (Pos + dist >= Source.Length)
            {
                return '\0';
            }
            return Source[Pos + dist];
        }

        /// <summary>
        /// IDs the next char, and advances position, "consuming" it, if it matches.
        /// Used to identify 2-character tokens like "->".
        /// </summary>
        /// <param name="expected">Takes a char to match on</param>
        /// <returns>Bool</returns>
        private bool Match(char expected)
        {
            if (IsAtEnd() || Source[Pos] != expected)
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
            while (Peek(0) != '"' && !IsAtEnd())
            {
                if (Peek(0) == '\n')
                {
                    Line++;
                    Col = 1;
                }
                else
                {
                    Advance();
                }
            }

            if (IsAtEnd())
            {
                throw new Exception($"Unterminated string at line {Line}, column {Col}");
            }

            // Consume closing quote.
            Advance();

            // Get string val & create token custom way as start & end need moving due to ""s.
            string val = Source[(Start + 1)..(Pos - 1)];
            Tokens.Add(new Token(TokenType.String, val, Line, Col));

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
            string text = Source[Start..Pos];
            TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.Identifier;
            AddToken(type);
        }

        /// <summary>
        /// Creates a token of the specified type with the current source substring.
        /// </summary>
        /// <param>TokenType</param>
        private void AddToken(TokenType type)
        {
            string text = Source[Start..Pos];
            Tokens.Add(new Token(type, text, Line, Col));
        }

        /// <summary>
        /// Creates err msg with location info.
        /// </summary>
        private Exception Error(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        {
            return new Exception($"Error at line {Line}, Col {Col}: {message}");
        }
    }
}