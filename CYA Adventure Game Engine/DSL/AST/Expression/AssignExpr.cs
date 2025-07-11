using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Used to register Identifiers to values, eg Variable assignment.
    /// </summary>
    public class AssignExpr : IExpr
    {
        public IExpr Name;
        public IExpr Value;

        public AssignExpr(IExpr name, IExpr value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"AssignExpr({Name}, {Value})";
        }

        public object Interpret(Environment state)
        {
            throw new Exception("Untreated Assign Expression found. Parser Problem.");
        }
    }
}
