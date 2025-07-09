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
            { "say", new Func<List<object>,object>(NativeFunctions.Say) },
            { "ask", new Func<List<object>,object>(NativeFunctions.Ask) },
        };

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
    }
}
