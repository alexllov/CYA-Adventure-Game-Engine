using CYA_Adventure_Game_Engine.DSL.Frontend;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    internal static class BinaryOperatorType
    {
        // Binary Operator SubTypes.
        public static readonly Dictionary<string, List<TokenType>> BinaryOperators = new()
        {
            { "arithmetic", [TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide] },
            { "relational", [TokenType.Equal,TokenType.NotEqual, TokenType.GreaterEqual, TokenType.GreaterThan, TokenType.LessEqual, TokenType.LessThan] },
            { "logical", [TokenType.And, TokenType.Or] },
        };
    }
}
