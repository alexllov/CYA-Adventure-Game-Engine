using CYA_Adventure_Game_Engine;
namespace External_Modules.Battle_Library
{
    public class Enemies : IModule
    {
        public Enemies() { }

        public Dictionary<string, Enemy> KnownEnemies = new()
        {
            // Strength.
            { "goblin", new Enemy("Goblin", "strength", 1, new Attack("slashes its dagger at you", new Weapons ().Get("dagger"))) },
            { "orc", new Enemy("Orc", "strength", 2, new Attack("thrusts its sword at you", new Weapons ().Get("sword"))) },
            { "troll", new Enemy("Troll", "strength", 3, new Attack("swings its club down towards you", new Weapons ().Get("club"))) },
            // Arcane.
            { "bull frog", new Enemy("Bull Frog", "arcane", 1, new Attack("flicks its tongue at you", new Weapon("tongue", "1d4", 0, "arcane"))) },
            { "wizard", new Enemy("Wizard", "arcane", 2, new Attack("throws a mote of fire at you", new Weapon("fireball", "1d4", 2, "arcane"))) },
            { "dragon", new Enemy("Dragon", "arcane", 5, new Attack("breathes fire at you", new Weapon("fire breath", "1d6", 2, "arcane"))) }
        };

        public Enemy GetRandom()
        {
            List<Enemy> enemies = [.. KnownEnemies.Values];
            Random rand = new();
            int index = rand.Next(0, enemies.Count);
            return enemies.ElementAt(index);
        }
    }

    /// <summary>
    /// Enemy object. Stat determines if its a strength or arcane enemy.
    /// Attack stores the attack text and a Weapon object.
    /// </summary>
    public class Enemy
    {
        public string Name;
        public string Stat;
        public int StatValue;
        public int Experience;
        public Attack Attack;
        public Enemy(string name, string stat, int statValue, Attack attack)
        {
            Name = name;
            Stat = stat;
            StatValue = statValue;
            Experience = statValue;
            Attack = attack;
        }

        public (string, int) Run()
        {
            (string, int) attack = Attack.Run();
            (string, int) addMod = (attack.Item1, attack.Item2 + StatValue);
            return Attack.Run();
        }
    }

    /// <summary>
    /// Holds a string text to describe the attack and a Weapon object.
    /// </summary>
    public class Attack
    {
        public string Text;
        public Weapon Weapon;
        public Attack(string text, Weapon weapon)
        {
            Text = text;
            Weapon = weapon;
        }

        public (string, int) Run()
        {
            return (Text, Weapon.Run());
        }
    }
}
