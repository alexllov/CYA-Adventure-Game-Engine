using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CYA_Adventure_Game_Engine
{
    internal class Engine
    {

        private Dictionary<string, Scene> Scenes;
        private Dictionary<string, IModule> State;
        public Engine(Dictionary<string, Scene> scenes, Dictionary<string, IModule> state)
        {
            Scenes = scenes;
            State = state;
        }

        private static Choice SelectChoice(Scene scene)
        {
            bool needInput = true;

            // Set to 1 as dummy for return parsing...?
            Choice choice = scene.Choices[1];

            //foreach (Choice c in scene.Choices)
            //{
            //    if (c.Actions is not null)
            //    {
            //        foreach (Action a in c.Actions)
            //        {
            //            Console.WriteLine(a.Raw);
            //            Console.WriteLine($"Length ^^ = {c.Actions.Count}");
            //        } 
            //    }
            //}

            while (needInput)
            {
                Base_UI.Show("Enter Choice: ");
                string? input = Console.ReadLine();
                
                // TODO: Setup Menu handling.
                if (input == "m")
                {
                    //HandleMenu()
                }
                else if (int.TryParse(input, out int i) 
                        && 0 < i 
                        && i < scene.Choices.Count)
                {
                    choice = scene.Choices[(i-1)];
                    needInput = false;
                    //return choice;
                }
                else
                {
                    Base_UI.ShowLine("Invalid Entry.");
                }
            }
            return choice;

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
                Base_UI.ShowLine(currentScene.ToString());

                // Check there are choices to make. Avoids end loop when hit end state.
                if (currentScene.Choices.Count > 0)
                {
                    // Placeholder choice to keep choice in scope.
                    Choice choice = new("placeholder", currentScene.ID, []);
                    // Keep presenting choices until a valid choice is made given state.
                    while (true)
                    {
                        choice = SelectChoice(currentScene);
                        Scene lastScene = currentScene;

                        (bool, string) response = choice.QueryActions(State);
                        if (!response.Item1)
                        {
                            Base_UI.ShowLine(response.Item2);
                        }
                        else { break; }
                    }


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
                else { break; }
            } 
        }
    }
}
