using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal interface InfixParselet
    {
        Expr Parse(Parser parser, Expr left, Token token);
    }

    public class BinaryOperatorParselet : InfixParselet
    {
        public Expr Parse(Parser parser, Expr left, Token token)
        {
            // Consume the operator token? This should be handled in ParseExpression anyway.
            // parser.Advance();
            // Parse the right side of the expression.
            Expr right = parser.ParseExpression();
            // Return a new binary expression with the left side, operator, and right side.
            return new BinaryExpr(left, token.Type, right);
        }
    }
}
