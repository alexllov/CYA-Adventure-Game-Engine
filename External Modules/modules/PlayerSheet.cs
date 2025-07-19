using CYA_Adventure_Game_Engine;

namespace External_Modules.modules
{
    public class PlayerSheet : IModule
    {
        public static int StartingPoints = 10;
        public int Str { get; set; }
        public int Dex;
        public int Con;
        public int Int;
        public int Wis;
        public int Cha;

        public void Set(string field, float floatVal)
        {
            int value = (int)floatVal;

            switch (field.ToLower())
            {
                case "str":
                    Str = value;
                    break;
                case "dex":
                    Dex = value;
                    break;
                case "con":
                    Con = value;
                    break;
                case "int":
                    Int = value;
                    break;
                case "wis":
                    Wis = value;
                    break;
                case "cha":
                    Cha = value;
                    break;
                default:
                    throw new Exception($"Error, PlayerSheet does not contain a field called: {field}");
            }
        }
    }
}
