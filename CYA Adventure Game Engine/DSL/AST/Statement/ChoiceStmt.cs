using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains Choice object.
    /// </summary>
    public class ChoiceStmt : IStmt
    {
        public IExpr Name;
        public IStmt Body;
        public ChoiceStmt(IExpr name, IStmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString()
        {
            return $"ChoiceStmt(Name: {Name}, Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.AddLocalChoice(this);
        }
    }
}
