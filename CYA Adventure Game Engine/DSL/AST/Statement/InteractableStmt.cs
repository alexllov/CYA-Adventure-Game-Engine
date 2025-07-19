using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains Interactable object.
    /// </summary>
    public class InteractableStmt : IStmt
    {
        public IExpr Name;
        public IStmt Body;
        public InteractableStmt(IExpr name, IStmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString()
        {
            return $"InteractableStmt(Name: {Name}, Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.AddLocal(this);
        }
    }
}
