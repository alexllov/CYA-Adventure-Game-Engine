using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal interface IInfixParselet
    {
        Expr Parse(Parser parser, Expr left, Token token);
        int GetPrecedence();
    }

    public class BinaryOperatorParselet : IInfixParselet
    {
        int _Precedence;

        public BinaryOperatorParselet(int precedence)
        {
            _Precedence = precedence;
        }
        public Expr Parse(Parser parser, Expr left, Token token)
        {
            // Consume the operator token? This should be handled in ParseExpression anyway.
            // parser.Advance();
            // Parse the right side of the expression.
            Expr right = parser.ParseExpression(_Precedence);
            // Return a new binary expression with the left side, operator, and right side.
            return new BinaryExpr(left, token.Type, right);
        }

        public int GetPrecedence()
        {
            return _Precedence;
        }
    }
}

