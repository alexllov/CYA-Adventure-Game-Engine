using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST.Expression
{
    /// <summary>
    /// Contains an outer object (left) and inner property (right).
    /// </summary>
    public class DotExpr : IExpr
    {
        public IExpr Left;
        public IExpr Right;
        public DotExpr(IExpr left, IExpr right)
        {
            Left = left;
            Right = right;
        }
        public override string ToString()
        {
            return $"DotExpr({Left}, {Right})";
        }

        public object Interpret(Environment state)
        {
            throw new Exception("Oops, this is not yet implemented.");
        }
    }
}
