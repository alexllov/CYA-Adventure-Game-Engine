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
    /// Handles the assignment of a value to a variable name.
    /// </summary>
    public class AssignStmt : IStmt
    {
        public IExpr Name;
        public IExpr Value;
        public AssignStmt(IExpr name, IExpr value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Debug Method.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"AssignStmt(Name: {Name}, Value: {Value})";
        }

        public void Interpret(Environment state)
        {
            if (Name is VariableExpr vExpr)
            {
                var name = vExpr.Value;
                object value = Value.Interpret(state);
                state.SetVal(name, value);
            }
            else { throw new Exception("Error, invalid argument passed as variable name."); }
        }
    }
}
