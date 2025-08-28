using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Expr Stmt: basic wrapper to promote Exprs for Stmt positions.
    /// </summary>
    public class ExprStmt : IStmt
    {
        public IExpr Expr;
        public ExprStmt(IExpr expr)
        {
            Expr = expr;
        }

        public override string ToString()
        {
            return $"ExprStmt({Expr})";
        }

        public void Interpret(Environment state)
        {
            Expr.Interpret(state);
        }

        /// <summary>
        /// Calls on the Pratt parser to parse an expression and wraps it in an ExprStmt.
        /// </summary>
        /// <returns>IStmt</returns>
        public static ExprStmt Parse(Parser parser)
        {
            IExpr expr = parser.ParseExpression(0);
            return new ExprStmt(expr);
        }
    }
}
