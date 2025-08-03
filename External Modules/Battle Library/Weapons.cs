using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.Modules;
namespace External_Modules.Battle_Library
{
    public class Weapons : IModule, IStatic
    {
        public Weapons() { }

        public static Dictionary<string, Weapon> WeaponList = new()
        {
            // Strength
            { "dagger", new Weapon("dagger", "1d4", 0, "strength") },
            { "club", new Weapon("club", "1d4", 1, "strength") },
            { "sword", new Weapon("sword", "1d6", 0, "strength") },
            { "longsword", new Weapon("longsword", "1d6", 1, "strength") },
            // Arcane
            { "wand", new Weapon("wand", "1d4", 0, "arcane") },
            { "spellbook", new Weapon("spellbook", "1d4", 1, "arcane") },
            { "staff", new Weapon("staff", "1d6", 0, "arcane") },
            { "fine-staff", new Weapon("fine-staff", "1d6", 1, "arcane") },
        };

        public Weapon Get(string name)
        {
            return WeaponList.TryGetValue(name, out Weapon? weapon) ? weapon
                : throw new Exception($"Weapon '{name}' not found.");
        }

        public Weapon GetRandom()
        {
            List<Weapon> weapons = [.. WeaponList.Values];
            Random rand = new();
            int index = rand.Next(0, weapons.Count);
            return weapons.ElementAt(index);
        }

        public Weapon GetRandom(string type)
        {
            List<Weapon> weapons = [.. WeaponList.Values];
            Random rand = new();
            if (type == "strength")
            {
                List<Weapon> strWeapons = [.. weapons.Where(w => w.Stat == "strength")];
                int index = rand.Next(0, strWeapons.Count);
                return strWeapons.ElementAt(index);
            }
            else if (type == "arcane")
            {
                List<Weapon> arcWeapons = [.. weapons.Where(w => w.Stat == "arcane")];
                int index = rand.Next(0, arcWeapons.Count);
                return arcWeapons.ElementAt(index);
            }
            else
            {
                throw new Exception($"Invalid weapon type '{type}'.");
            }
        }
    }

    public class Weapon
    {
        public string Name { get; set; }
        public string DamageRoll { get; set; }
        public int DamageMod { get; set; }
        public string Stat { get; set; }
        public Weapon(string name, string damageRoll, int damageMod, string stat)
        {
            Name = name;
            DamageRoll = damageRoll;
            DamageMod = damageMod;
            Stat = stat;
        }

        public override string ToString()
        {
            return $"{Name} ({DamageRoll} + {DamageMod}) - {Stat}";
        }

        public int Run()
        {
            // Simulate rolling the damage roll.
            var roll = Dice.Roll(DamageRoll);
            return roll + DamageMod;
        }
    }
}
