﻿namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
{
    /// <summary>
    /// Token Types.
    /// </summary>
    public enum TokenType
    {
        None,
        Import, As,
        Identifier, Number, String, Boolean,
        Plus, Minus, Multiply, Divide,
        Assign, Equal, NotEqual, GreaterThan, GreaterEqual, LessThan, LessEqual,
        Not,
        And, Or,
        If, Then, Else, While,
        Scene, Table, Overlay, End,
        Run, Exit,
        Access,
        GoTo,
        LBracket, RBracket, LParent, RParent, LCurly, RCurly,
        Noun, Verb, Prep, Default,
        Dot,
        Comma,
        Colon,
        Pipe,
        Comment,
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
