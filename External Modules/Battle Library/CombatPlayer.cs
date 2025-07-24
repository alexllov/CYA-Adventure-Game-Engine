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
    }
}
