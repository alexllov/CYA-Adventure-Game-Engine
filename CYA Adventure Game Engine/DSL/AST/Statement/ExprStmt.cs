using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
