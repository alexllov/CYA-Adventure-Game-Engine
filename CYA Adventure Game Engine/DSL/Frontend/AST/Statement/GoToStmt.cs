using CYA_Adventure_Game_Engine.DSL.Frontend.AST.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST.Statement
{
    /// <summary>
    /// GoTo: Controls Flow between scenes.
    /// </summary>
    public class GoToStmt : IStmt
    {
        public StringLitExpr Location;

        public GoToStmt(StringLitExpr loc)
        {
            Location = loc;
        }
        public override string ToString()
        {
            return $"GoToStmt({Location})";
        }

        public void Interpret(Environment state)
        {
            string loc = (string)Location.Interpret(state);
            state.SetGoTo(loc);
        }
    }
}
