using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.Modules;
namespace External_Modules.Battle_Library
{
    public class CombatPlayer : IModule
    {
        public int Strength;
        public int Arcane;
        public int Health;
        public int Experience;

        public Inventory Inventory { get; set; }

        public CombatPlayer()
        {
            Strength = 0;
            Arcane = 0;
            Health = 5;
            Experience = 0;
            Inventory = new();
        }

        public override string ToString()
        {
            return $"Strength: {Strength}, Arcane: {Arcane}, Health: {Health}, Experience: {Experience}.";
        }
        public void AssignPoints(float points)
        {
            AssignCombatStats((int)points);
        }

        private void AssignCombatStats(int points)
        {
            Console.WriteLine($"You have {points} points to assign to your Strength & Arcane stats.");
            while (true)
            {
                Console.Write("Assign Strength: ");
                string choice = Console.ReadLine();
                if (int.TryParse(choice, out int strength) && strength >= 0 && strength <= points)
                {
                    Strength = strength;
                    Arcane = points - strength;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number between 0 and " + points + ".");
                }
            }
        }

        public void LevelUp()
        {
            if (Experience < 3)
            {
                Console.WriteLine("You need 3 experience to level up. You don't have enough");
            }
            else
            {
                Console.WriteLine("You can spend 3 experience to level up - gain one more point in Strengh, Arcane, or Health.\n"
                    + $"Current Stats: Strength: {Strength}, Arcane: {Arcane}, Health: {Health}, Experience: {Experience}.\n"
                    + $"Options:\n"
                    + "1. Strength\n"
                    + "2. Arcane\n"
                    + "3. Health\n"
                    + "4. Back\n");
                while (true)
                {
                    Console.Write("Choose an option: ");
                    string choice = Console.ReadLine();
                    if (choice is not null
                        && int.TryParse(choice, out int i)
                        && i > 0 && i <= 4)
                    {
                        switch (i)
                        {
                            case 1:
                                Strength++;
                                Experience -= 3;
                                Console.WriteLine($"You gained a point in Strength. New Strength: {Strength}.");
                                return;
                            case 2:
                                Arcane++;
                                Experience -= 3;
                                Console.WriteLine($"You gained a point in Arcane. New Arcane: {Arcane}.");
                                return;
                            case 3:
                                Health++;
                                Experience -= 3;
                                Console.WriteLine($"You gained a point in Health. New Health: {Health}.");
                                return;
                            case 4:
                                return;
                        }
                    }
                }
            }
        }
    }
}
