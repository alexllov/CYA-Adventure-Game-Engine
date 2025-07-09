using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    public class Environment
    {
        private Dictionary<string, object> Env = new();

        public Environment() { }

        public void SetVal(string name, object value)
        {
            Env[name] = value;
        }
        public object GetVal(string name)
        {
            if (!(Env.ContainsKey(name))) { throw new Exception($"Error, tried to use non-assigned variable, {name}"); }
            return Env[name];
        }
    }
}
