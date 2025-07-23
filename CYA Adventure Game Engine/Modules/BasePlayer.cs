namespace CYA_Adventure_Game_Engine.Modules
{
    public class BasePlayer : IModule
    {
        public int Strength;
        public int Arcane;
        public int Health;
        public int Experience;

        public void Set(string attr, float floatVal)
        {
            int val = (int)floatVal;

            switch (attr.ToLower())
            {
                case "str":
                case "strength":
                    Strength = val;
                    break;
                case "arc":
                case "arcane":
                    Arcane = val;
                    break;
                case "hea":
                case "health":
                    Health = val;
                    break;
                case "exp":
                case "experience":
                    Experience = val;
                    break;
                default:
                    throw new Exception($"Error, attempted to set {attr} in BasePlayer. This is not a known attribute.");
            }
        }

        public int Get(string attr)
        {
            switch (attr.ToLower())
            {
                case "strength":
                    return Strength;
                case "arcane":
                    return Arcane;
                case "health":
                    return Health;
                case "experience":
                    return Experience;
                default:
                    throw new Exception($"Error, attempted to get {attr} in BasePlayer. This is not a known attribute.");


            }

        }
    }
}
