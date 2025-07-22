using CYA_Adventure_Game_Engine;
namespace External_Modules.modules
{
    public class MoonrunnerPlayer : IModule
    {
        public int Skill;
        public int Stamina;
        public int Luck;
        public void Set(string field, float floatVal)
        {
            int value = (int)floatVal;

            switch (field.ToLower())
            {
                case "skill":
                    Skill = value;
                    break;
                case "stamina":
                    Stamina = value;
                    break;
                case "luck":
                    Luck = value;
                    break;
                default:
                    throw new Exception($"Error, PlayerSheet does not contain a field called: {field}");
            }
        }
    }
}
