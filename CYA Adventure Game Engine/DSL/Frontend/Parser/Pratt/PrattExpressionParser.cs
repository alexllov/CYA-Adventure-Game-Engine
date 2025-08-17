using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt
{
    public static class PrattExpressionParser
    {
        // Part Dicts for Pratt.
        static readonly Dictionary<TokenType, IPrefixParselet> PrefixParts = new()
        {
            // Datas.
            {TokenType.Identifier, new NameParselet()},
            {TokenType.Number, new NameParselet()},
            {TokenType.String, new NameParselet()},
            {TokenType.Boolean, new NameParselet()},
            // Prefixes.
            {TokenType.Plus, new PrefixOperatorParselet()},
            {TokenType.Minus, new PrefixOperatorParselet()},
            {TokenType.Not, new PrefixOperatorParselet()},
            {TokenType.LParent, new ParentParselet()},
        };

        static readonly Dictionary<TokenType, IInfixParselet> InfixParts = new()
        {
            // Arithmetic Operators.
            {TokenType.Plus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Minus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Multiply, new BinaryOperatorParselet(Precedence.PRODUCT)},
            {TokenType.Divide, new BinaryOperatorParselet(Precedence.PRODUCT)},
            // Comparative.
            {TokenType.Equal, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.NotEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            // Boolean.
            {TokenType.And, new BinaryOperatorParselet(Precedence.AND)},
            {TokenType.Or, new BinaryOperatorParselet(Precedence.OR)},
            // Dot.
            {TokenType.Dot, new DotParselet(Precedence.DOT)},
        };

        /*
         * The following 'ParseExpression' function is adapted from a function of the same name from:
         * Author: jfcardinal
         * Source: https://github.com/jfcardinal/BantamCs
         * Used under MIT Lisence
         */
        /// <summary>
        /// Expression Parsing - Implements a Pratt Parser.
        /// </summary>
        /// <returns>Expr</returns>
        /// <exception cref="Exception"></exception>
        public static IExpr ParseExpression(this Parser parser, int precedence)
        {
            Token token = parser.Tokens.Advance();

            if (!PrefixParts.TryGetValue(token.Type, out IPrefixParselet? prefix))
            {
                throw new Exception($"Unexpected token type: {token.Type} on line {token.position[0]} in file: {token.SourceFile}. " +
                    $"Occured while parsing an expression within a {parser.CurrentStmtParsing}. " +
                    $"Statement started at {parser.StartOfCurrentStmt}.");
            }
            IExpr left = prefix.Parse(parser, token);

            /*
             * Identify Infix (if there is one)
             * Take the precedence of the next token & compare to this expr's precedence.
             * If next token isn't an infix, will return 0, which should signal the end of his expression.
             */
            while (precedence < GetPrecedence(parser))
            {
                token = parser.Tokens.Advance();
                if (!InfixParts.TryGetValue(token.Type, out IInfixParselet? infix))
                {
                    throw new Exception($"Unexpected token type: {token.Type} on line {token.position[0]} in file: {token.SourceFile}. " +
                        $"Occured while parsing an expression within a {parser.CurrentStmtParsing}. " +
                        $"Statement started at {parser.StartOfCurrentStmt}.");
                }
                left = infix.Parse(parser, left, token);
            }
            return left;
        }

        /*
         * The following 'GetPrecedence' function is taken from a function of the same name from:
         * Author: jfcardinal
         * Source: https://github.com/jfcardinal/BantamCs
         * Used under MIT Lisence
         */
        /// <summary>
        /// Helper func to get precedence of the next token.
        /// </summary>
        /// <returns>int</returns>
        private static int GetPrecedence(Parser parser)
        {
            if (!InfixParts.TryGetValue(parser.Tokens.Peek(0).Type, out IInfixParselet? infix))
            {
                return 0;
            }
            return infix.GetPrecedence();
        }
    }
}
