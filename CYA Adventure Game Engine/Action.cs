using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public class Action
    {
        private readonly string? Raw;

        public Action(string? action)
        {
            this.Raw = action;
        }

        public string DebugToString()
        {
            return $"{Raw}";
        }
    }
}
