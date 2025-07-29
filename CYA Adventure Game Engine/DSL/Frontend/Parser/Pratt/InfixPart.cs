using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt
{
    /// <summary>
    /// Infix: an operator between a left & right hand expression.
    /// </summary>
    internal interface IInfixParselet
    {
        IExpr Parse(Parser parser, IExpr left, Token token);
        int GetPrecedence();
    }

    /// <summary>
    /// Processes a binary operator between two expressions.
    /// </summary>
    public class BinaryOperatorParselet : IInfixParselet
    {
        readonly int _Precedence;

        public BinaryOperatorParselet(int precedence)
        {
            _Precedence = precedence;
        }
        public IExpr Parse(Parser parser, IExpr left, Token token)
        {
            // Parse the right side of the expression.
            IExpr right = parser.ParseExpression(_Precedence);
            // Return a new binary expression with the left side, operator, and right side.
            return new BinaryExpr(left, token.Type, right);
        }

        public int GetPrecedence()
        {
            return _Precedence;
        }
    }

    /// <summary>
    /// Creates a DotExpr.
    /// </summary>
    public class DotParselet : IInfixParselet
    {
        readonly int _Precedence;
        public DotParselet(int precedence)
        {
            _Precedence = precedence;
        }
        public IExpr Parse(Parser parser, IExpr left, Token token)
        {
            // Parse the right side of the expression.
            IExpr right = parser.ParseExpression(_Precedence);
            if (right is VariableExpr rVar) { return new DotExpr(left, rVar); }
            else { throw new Exception("Error, tried to construct Dot expression with an invalid right argument. Must be a Variable Expression."); }

        }
        public int GetPrecedence()
        {
            return _Precedence;
        }
    }
}

