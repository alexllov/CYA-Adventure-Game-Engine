using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    internal class RunStmt : IStmt
    {
        public string Overlay;
        public RunStmt(string overlay)
        {
            Overlay = overlay;
        }
        public void Interpret(Environment state)
        {
            state.SetRunOverlayFlag(Overlay);
        }
        public override string ToString()
        {
            return $"runStmt({Overlay})";
        }
    }
}
