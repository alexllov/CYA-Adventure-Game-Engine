using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains a Number Literal, held as a float.
    /// </summary>
    public class NumberLitExpr : IExpr
    {
        public float Value;
        public NumberLitExpr(float val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return $"NumberLitExpr({Value})";
        }

        public object Interpret(Environment state) { return Value; }
    }
}
