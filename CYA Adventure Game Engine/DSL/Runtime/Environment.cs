using CYA_Adventure_Game_Engine.DSL.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    public class Environment
    {
        /// <summary>
        /// Dict stores all assignments crated in interpretation.
        /// Pre-assigned values represent language defaults, e.g. Native Functions.
        /// </summary>
        private Dictionary<string, object> Env = new()
        {
            { "say", NativeFunctions.Say },
            { "ask", NativeFunctions.Ask },
        };

        private Dictionary<string, BlockStmt> Scenes = new();

        public List<InteractableStmt> Local = new();

        public Environment() { }

        /// <summary>
        /// Set a value to a given alias.
        /// </summary>
        /// <param name="name">string variable alias.</param>
        /// <param name="value">object value.</param>
        public void SetVal(string name, object value)
        {
            Env[name] = value;
        }

        /// <summary>
        /// Retreive assigned value from alias.
        /// </summary>
        /// <param name="name">variable alias.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public object GetVal(string name)
        {
            if (!(Env.ContainsKey(name))) { throw new Exception($"Error, tried to use non-assigned variable, {name}"); }
            return Env[name];
        }

        public void SetScene(string name, BlockStmt value)
        {
            if (Scenes.ContainsKey(name)) { throw new Exception($"Error, a Scene with the name {name} has already been declared."); }
            Scenes[name] = value;
        }

        public BlockStmt GetScene(string name) 
        {
            if (Scenes.TryGetValue(name, out BlockStmt? block)) { return block; }
            else { throw new Exception($"Error, requested a scene that does not exist: {name}"); }
        }

        public void ClearLocal()
        {
            Local = [];
        }

        public void AddLocal(InteractableStmt interactable)
        {
            Local.Add(interactable);
        }

        public InteractableStmt GetLocal(int i)
        {
            if (Local.Count() < i) { throw new Exception("List index out of range."); }
            else { return Local[i]; }
        }
    }
}
