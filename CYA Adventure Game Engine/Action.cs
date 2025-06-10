using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public class Action
    {
        private readonly string Address;
        private readonly string Method;
        private readonly List<string> Body;

        public Action(string address, string method, List<string> body)
        {
            Address = address;
            Method = method;
            Body = body;
        }

        public (bool, string) Query(Dictionary<string, IModule> state)
        {
            // TODO: setup handling for defaults (eg, save)
            List<(bool, string)> results = new();
            if (Address is not "")
            {
                return state[Address].Query(Method, Body);
            }
            return (true, "");
        }
    }

    //public class BaseAction : Action
    //{
    //
    //}
}
