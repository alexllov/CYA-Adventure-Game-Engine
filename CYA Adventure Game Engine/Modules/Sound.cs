using System.Media;

namespace CYA_Adventure_Game_Engine.Modules
{
    public class Sound : IModule, IStatic
    {
        public static void Play(string body)
        {
            new SoundPlayer(body).Play();
        }
    }
}
