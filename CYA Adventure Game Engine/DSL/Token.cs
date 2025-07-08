using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    public enum TokenType
    {
        None,
        Import, As,
        Identifier, Number, String,
        Dot,
        Comma,
        Pipe,
        Plus, Minus, Multiply, Divide,
        Assign, Equal, NotEqual, GreaterThan, GreaterEqual, LessThan, LessEqual,
        Not,
        And, Or,
        Comment,
        If, Then, Else, While,
        Scene, Table, Code, End,
        GoTo,
        LBracket, RBracket, LParent, RParent,
        EOF,
    }
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }

        public int[] position;

        public Token(TokenType type, string lexeme, int line, int col)
        {
            Type = type;
            Lexeme = lexeme;
            position = [line, col];
        }

        public override string ToString()
        {
            return $"{Type} '{Lexeme}', at line {position[0]}, column {position[1]}";
        }
    }
}
