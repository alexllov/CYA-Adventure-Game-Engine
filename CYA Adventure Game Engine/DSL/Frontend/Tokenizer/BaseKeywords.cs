namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
{
    public static class BaseKeywords
    {
        public static Dictionary<string, TokenType> Keywords = new()
        {
            {"import", TokenType.Import },
            {"as", TokenType.As },
            {"START", TokenType.GoTo },
            {"->", TokenType.GoTo },
            {"scene", TokenType.Scene },
            {"table", TokenType.Table },
            {"overlay", TokenType.Overlay },
            {"access", TokenType.Access },
            {"run", TokenType.Run },
            {"exit", TokenType.Exit },
            {"end", TokenType.End },
            {"if", TokenType.If },
            {"then", TokenType.Then },
            {"else", TokenType.Else },
            {"and", TokenType.And },
            {"or", TokenType.Or },
            {"true", TokenType.Boolean },
            {"false", TokenType.Boolean },
        };
    }
}
