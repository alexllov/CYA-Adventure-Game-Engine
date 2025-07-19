using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Handles the importing of optional modules. Optional aliasing support.
    /// </summary>
    public class ImportStmt : IStmt
    {
        public string Module;
        public string Alias;
        public ImportStmt(string module, string alias = null)
        {
            Module = module;
            Alias = alias ?? module;
        }
        public override string ToString()
        {
            return Alias == null ? $"ImportStmt(Module: {Module})" : $"ImportStmt(Module: {Module}, Alias: {Alias})";
        }

        public void Interpret(Environment state)
        {
            IModule module = state.GetModule(Module);
            state.SetVal(Alias, module);
        }
    }
}
