using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
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
