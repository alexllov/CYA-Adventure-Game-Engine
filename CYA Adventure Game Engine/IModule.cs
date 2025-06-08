using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public interface IModule
    {
        //string Name { get; }
        // TODO: Query();
        (bool, string) Process (string method, List<string> body);

    }
}
