using System.Media;

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
    }
}
