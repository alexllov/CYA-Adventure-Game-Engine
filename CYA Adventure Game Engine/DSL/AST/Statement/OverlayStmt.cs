using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    public class OverlayStmt : IStmt
    {
        public string Name;
        public string? KeyBind;
        public BlockStmt Body;
        public OverlayStmt(string name, BlockStmt body, string? key = null)
        {
            Name = name;
            Body = body;
            KeyBind = key;
        }
        public override string ToString()
        {
            return $"OverlayStmt(\n  Name: {Name},\n  KeyBind: {KeyBind},\n  Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.SetOverlay(Name, this);
        }
    }
}
