﻿namespace CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer
{
    internal static class BaseKeywords
    {
        internal static Dictionary<string, TokenType> Keywords = new()
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
            {"while", TokenType.While },
            {"and", TokenType.And },
            {"or", TokenType.Or },
            {"true", TokenType.Boolean },
            {"false", TokenType.Boolean },
            {"noun", TokenType.Noun},
            {"verb", TokenType.Verb},
            {"prep", TokenType.Prep},
            {"ind", TokenType.Noun},
            {"default", TokenType.Default },
            {"other", TokenType.Default },
        };
    }
}
