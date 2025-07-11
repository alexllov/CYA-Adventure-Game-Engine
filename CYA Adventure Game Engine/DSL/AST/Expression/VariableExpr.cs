using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains variables, which should be assigned & registered in the Environment before called upon.
    /// </summary>
    public class VariableExpr : IExpr
    {
        public string Value;
        public VariableExpr(string name)
        {
            Value = name;
        }
        public override string ToString()
        {
            return $"{Value}";
        }

        public object Interpret(Environment state) { return state.GetVal(Value); }
    }
}
