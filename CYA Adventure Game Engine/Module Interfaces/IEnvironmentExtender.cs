using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace CYA_Adventure_Game_Engine
{
    public interface IEnvironmentExtender
    {
        public void CreateEnvironmentWrapper(Environment environment);
    }
}


