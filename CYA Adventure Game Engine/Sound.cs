using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    // TODO: Set up as static class to handle sound effects.
    // Does not need instances to be made, hence can be static.
    public class Sound : IModule
    {

        (bool, string) IModule.Query(string method, List<string> body)
        {
            return (true, "");
        }
        (bool, string) IModule.Process(string method, List<string> body)
        {
            return (true, "");
        }
    }
}
