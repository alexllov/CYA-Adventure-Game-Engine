using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    internal class Engine
    {

        private Dictionary<string, Scene> Scenes;
        private Dictionary<string, IModule>? State;
        public Engine(Dictionary<string, Scene> scenes, Dictionary<string, IModule>? state)
        {
            Scenes = scenes;
            State = state;
        }

        private static Choice SelectChoice(Scene scene)
        {
            Base_UI.ShowLine(scene.ToString());

            bool needInput = true;

            while (needInput)
            {
                Base_UI.Show("Enter Choice: ");
                string? input = Console.ReadLine();
                
                // TODO: Setup Menu handling.
                if (input == "m")
                {
                    //HandleMenu()
                }
                else if (int.TryParse(input, out int i))
                {
                    // TODO: Deal with choice number out of range.
                    Choice choice = scene.Choices[i];
                    needInput = false;
                    return choice;
                }
                else
                {
                    Base_UI.ShowLine("Invalid Entry.");
                }
            }
            Choice choiceF = scene.Choices[1];
            needInput = false;
            return choiceF;

        }

        public void Run()
        {
            string start = "1";
            Scene currentScene = Scenes[start];

            /*
             * TODO: Change this to take State["UI"] in future.
             * Will have to list as req name & module if do.
             * Look @ if better way to handle, ie method override to replace basic UI.
             */

            while (true)
            {
                // TODO: add a check here for invalid scene.

                // select choice & processing need to be in a block here to ensure failure to complete -> scene reload.
                // ^^ this may acc already be handled by the loop itself. Just have the err msg -> NOT change scene.

                Choice choice = SelectChoice(currentScene);
                Scene lastScene = currentScene;
                //Console.WriteLine($"You selected the following choice:\n{choice.ToString()}\nTarget:{choice.Target}");

                /*
                 *  process choice ->
                 *      1. query all actions to check all are possible given current states.
                 *      2. process all actions s.t. they return the appropriate functions.
                 *      3. return the process info
                 *          i. Failure at query -> str err msg explaining failure.
                 *          ii.Functions required to exec all actions in order.
                 *      NB. Actions CAN change choice.target.
                 */

                // VERY  END OF THE LOOP
                currentScene = Scenes[choice.Target];



            } 
        }
    }
}
