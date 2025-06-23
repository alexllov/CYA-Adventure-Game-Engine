using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal class Tokenizer
    {
        private string Source;
        private int Start = 0, Pos = 0;
        private int Line = 1, Col = 1;
        private static Dictionary<string, TokenType> Keywords = new()
        {
            {"import", TokenType.Import },
            {"as", TokenType.As },
            {"->", TokenType.GoTo },
            {"if", TokenType.If },
            {"then", TokenType.Then },
            {"else", TokenType.Else },
            {"while", TokenType.While },
        };
        private List<Token> Tokens = new();

        public void Show()
        {
            Console.WriteLine("Tokens:");
            Console.WriteLine($"Len Tokens: {Tokens.Count()}");
            foreach (var token in Tokens)
            {
                Console.WriteLine(token);
            }
        }
        public Tokenizer(string filepath)
        {
            Source = File.ReadAllText(filepath);
            Console.WriteLine($"Source code loaded from {filepath} with {Source.Length} characters.");
            Console.WriteLine($"{Source}");
            Tokenize();
        }

        /// <summary>
        /// Tokenizes the source code, returning a list of tokens.
        /// </summary>
        /// <returns>List<Token></returns>
        private List<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                Start = Pos;
                ScanToken();
            }
            Tokens.Add(new Token(TokenType.EOF, "", Line, Col));
            return Tokens;
        }

        private char Advance()
        {
            Pos++;
            Col++;
            return Source[Pos - 1];
        }
        private bool IsAtEnd()
        {
            return Pos >= Source.Length;
        }
        private void ScanToken()
        {
            char c = Advance();
            Console.WriteLine(c);
            switch (c)
            {
                case '#':
                    while (!IsAtEnd() && Source[Pos] != '\n')
                    {
                        Advance();
                    }
                    AddToken(TokenType.Comment);
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
                    if (Char.IsDigit(c))
                    {
                        ReadNumber();
                    }
                    else if (Char.IsLetterOrDigit(c))
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
        /// Returns current char w/o advancing position, or consuming char.
        /// If EOF, returns '\0' identifier.
        /// </summary>
        /// <returns></returns>
        private char Peek()
        {
            if (IsAtEnd())
            {
                return '\0';
            }
            return Source[Pos];
        }

        private char PeekNext()
        {
            if (Pos + 1 >= Source.Length)
            {
                return '\0';
            }
            return Source[Pos + 1];
        }
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

        private void ReadString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
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

        private void ReadNumber()
        {
            while (Char.IsDigit(Peek()))
            {
                Advance();
            }
            while (Peek() == '.' && Char.IsDigit(PeekNext()))
            {
                Advance();
                while (Char.IsDigit(Peek()))
                {
                    Advance();
                }
            }
            AddToken(TokenType.Number);
        }

        private void ReadIdentifier()
        {
            while (Char.IsLetterOrDigit(Peek()) || Peek() == '_')
            {
                Advance();
            }
            string text = Source[Start..Pos];
            TokenType type = Keywords.ContainsKey(text) ? Keywords[text] : TokenType.Identifier;
            AddToken(type);
        }

        private void AddToken(TokenType type)
        {
            string text = Source[Start..Pos];
            Tokens.Add(new Token(type, text, Line, Col));
        }

        private Exception Error(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        {
            return new Exception($"Error at line {Line}, Col {Col}: {message}");
        }
    }
}