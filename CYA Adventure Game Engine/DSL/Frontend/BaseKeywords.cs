using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    internal static class BaseKeywords
    {
        internal static Dictionary<string, TokenType> Keywords = new()
        {
            {"import", TokenType.Import },
            {"as", TokenType.As },
            {"START", TokenType.Start },
            {"->", TokenType.GoTo },
            {"scene", TokenType.Scene },
            {"table", TokenType.Table },
            {"overlay", TokenType.Overlay },
            {"end", TokenType.End },
            {"if", TokenType.If },
            {"then", TokenType.Then },
            {"else", TokenType.Else },
            {"while", TokenType.While },
            {"and", TokenType.And },
            {"or", TokenType.Or },
        };
    }
}
