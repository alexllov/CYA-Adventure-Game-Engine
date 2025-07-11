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
    /// The starting GoTo for the game.
    /// </summary>
    public class StartStmt : IStmt
    {
        public StringLitExpr Location;
        public StartStmt(StringLitExpr loc)
        {
            Location = loc;
        }

        public void Interpret(Environment state)
        {
            string loc = (string)Location.Interpret(state);
            state.SetGoTo(loc);
        }
    }
}
