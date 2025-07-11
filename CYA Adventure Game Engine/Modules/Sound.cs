using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace CYA_Adventure_Game_Engine.Modules
{
    // TODO: Set up as static class to handle sound effects.
    // Does not need instances to be made, hence can be static.
    public class Sound : IModule
    {
        public static string Data = "I am the data :)";

        (bool, string) IModule.Query(string method, List<string> body)
        {
            return (true, "");
        }
        (bool, string) IModule.Process(string method, List<string> body)
        {
            new SoundPlayer (body.First()).Play();
            return (true, "");
        }

        public static void PlayFunc()
        {
            Console.WriteLine("Wowee, i played a sound (no sound will actually play)");
        }

        public static Action Play = PlayFunc;
    }
}
