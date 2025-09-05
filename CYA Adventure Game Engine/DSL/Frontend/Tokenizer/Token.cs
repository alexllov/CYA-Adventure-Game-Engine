namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
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
        Dot,
        Comma,
        Colon,
        Pipe,
        Comment,
        EOF,
    }

    /*
     * The following object, Token is modified from the Token object implemented by
     * Nystrom, R. in Chapter 4 of "Crafting Interpreters".
     * Unlike Nystom's, this does not include a object literal field, and does include the added SourceFile string.
     * This is used in accordance with the MIT lisence granted to Nystrom, R. for "Crafting Interpreters".
     * 
     * Nystrom, R. (2019) Scanning. Available at: https://craftinginterpreters.com/scanning.html (Accessed 5 September 2025)
     */
    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }

        public int[] position;

        public string SourceFile;

        public Token(TokenType type, string lexeme, int line, int col, string sourceFile)
        {
            Type = type;
            Lexeme = lexeme;
            position = [line, col];
            SourceFile = sourceFile;
        }

        public override string ToString()
        {
            return $"{Type} '{Lexeme}', at line {position[0]}, in file: {SourceFile}";
        }
    }
}
