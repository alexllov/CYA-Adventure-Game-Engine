using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // TODO: This needs reworking from a hard-wired address to one that can be flexible.
            //   Should correspond to a /Modles/ folder within a game's file itself.
            //   Consider having an internal modules for base modules
            //   && if that fails to find, scan for local.
            Type type = Type.GetType($"CYA_Adventure_Game_Engine.Modules.{Module}");

            if (!(typeof(IModule).IsAssignableFrom(type)))
            {
                throw new Exception("Invalid Import statement detected. Can only import IModules.");
            }
            else
            {
                // Static Module Loading First.
                // TODO: Set up If block here for static vs non-static type imports.
                IModule module = (IModule)Activator.CreateInstance(type);
                state.SetVal(Alias, module);
            }
        }
    }
}
