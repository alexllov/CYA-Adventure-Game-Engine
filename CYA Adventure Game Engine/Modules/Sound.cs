using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Data;

namespace CYA_Adventure_Game_Engine.Modules
{
    // TODO: Set up as static class to handle sound effects.
    // Does not need instances to be made, hence can be static.
    public class Sound : IModule
    {

        public static void Play(string body)
        {
            new SoundPlayer(body).Play();
        }

        public void Test () { Console.WriteLine("This is the TEST FUNC in SOUND"); }
    }
}
