namespace CYA_Adventure_Game_Engine.Modules
{
    public class Dice : IModule
    {
        public static float Roll(string rollType)
        {
            var rand = new Random();
            if (!(rollType.Contains('d')))
            {
                throw new Exception(
                "Error, wrongly formatted call to Dice.Roll function. " +
                "String argument must be formatted as xdy where x is the number of dice to roll and y is the size of the dice. " +
                "Please ensure these are searated by a 'd'"
                );
            }
            var numbers = rollType.Split('d').Select(int.Parse).ToList();
            int diceCount = numbers[0];
            int diceSides = numbers[1];
            int total = 0;
            for (int i = 0; i < diceCount; i++)
            {
                total += rand.Next(1, diceSides + 1);
            }
            return (float)total;
        }
    }
}
