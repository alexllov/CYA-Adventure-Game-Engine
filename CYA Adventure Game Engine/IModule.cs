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
        // TODO: change Process return stuff;
        public (bool, string) Process (string method, List<string> body);

        public (bool, string) Query(string method, List<string> body);
    }
}
