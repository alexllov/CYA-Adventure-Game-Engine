using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CYA_Adventure_Game_Engine.Modules
{
    internal class Dice : IModule
    {
        (bool, string) IModule.Query(string method, List<string> body)
        {
            return (true, "");
        }

        (bool, string) IModule.Process(string method, List<string> body)
        {
            var rand = new Random();

            //Work out the dice to roll
            var numbers = method.Split('d').Select(int.Parse).ToList();
            int diceCount = numbers[0];
            int diceSides = numbers[1];
            int tot = 0;
            for (int i = 0; i < diceCount; i++)
            {
                tot += rand.Next(1, diceSides + 1);
            }
            return (true, $"{tot}");
        }
    }
}
