using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    internal class Interpreter
    {
        public AbstSyntTree AST;

        // Debug = print Expr results.
        private bool DebugMode;

        private Environment Env;

        private string? GoToAddress;

        public Interpreter(AbstSyntTree Tree, Environment env, string mode = "default")
        {
            AST = Tree;
            Env = env;
            DebugMode = mode == "debug";
        }

        /// <summary>
        /// Interprets all top-level statements from the AST, 
        /// completing a "first-pass" to set up the game Environment in order to be ran properly in 'RunGame'.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Interpret()
        {
            int startCount = AST.Tree.Count(s => s is StartStmt);
            if (startCount != 1) { throw new Exception($"Warning, a Game file needs exactly 1 'START' command. {startCount} were found."); }
            foreach (IStmt stmt in AST)
            {
                stmt.Interpret(Env);
            }
        }

        /// <summary>
        /// Core Game loop. Move player to the next scene & then run it.
        /// </summary>
        public void RunGame()
        {
            SceneStmt scene = RunGoTo();
            while (true)
            {
                scene = RunScene(scene);
            }
        }

        /// <summary>
        /// Requests the next game scene registered in the environment.
        /// </summary>
        /// <returns>SceneStmt containing the next scene.</returns>
        private SceneStmt RunGoTo()
        {
            SceneStmt nextScene = Env.GetScene(Env.GetGoTo());
            return nextScene;
        }

        /// <summary>
        /// Helper func, presents any Choice-Driven 'choices' to the user.
        /// </summary>
        private void ShowChoices()
        {
            if (Env.LocalChoices.Count > 0)
            {
                Console.WriteLine("\nOptions:");
                int num = 1;
                foreach (ChoiceStmt interactable in Env.LocalChoices)
                {
                    Console.WriteLine($"{num}. {interactable.Name.Interpret(Env)}");
                    num++;
                }
            }
        }

        /// <summary>
        /// Runs the code stored within a Scene statement.
        /// Takes user input & checks for corresponding 'Choices', 'Keybinds' (for overlays), or 'Commanands' to execute appropriately.
        /// </summary>
        /// <param name="scene"></param>
        /// <returns>SceneStmt: the next scene to be ran as result of player's input.</returns>
        private SceneStmt RunScene(SceneStmt scene)
        {
            // Reset local scope.
            Env.ClearLocal();
            Console.WriteLine("\n========================================\n");
            scene.Body.Interpret(Env);

            ShowChoices();
            while (true)
            {
                string text = "Enter your " + Env switch
                {
                    { LocalChoices.Count: > 0, LocalCommands.Count: 0 } => "choice: ",
                    { LocalChoices.Count: 0, LocalCommands.Count: > 0 } => "command: ",
                    _ => "choice or command: "
                };
                Console.Write(text);

                // Get & process user input.
                var choice = Console.ReadLine();
                // Choice.
                if (int.TryParse(choice, out int i) && Env.HasLocalChoice(i))
                {
                    HandleChoice(i);
                }
                // Overlay.
                else if (choice is not null && Env.CheckAccessibleOverlay(choice, out OverlayStmt? overlay))
                {
                    HandleOverlay(overlay!);
                }
                // Command.
                else if (choice is not null && choice.Split(' ').Length == 2)
                {
                    HandleCommand(choice);
                }
                else { Console.WriteLine("Error, invalid selection."); }

                // Break from loop if GoTo updated.
                if (Env.CheckGoToFlag()) { break; }
            }
            return Env.GetScene(Env.GetGoTo());
        }

        /// <summary>
        /// Gets the block statement associated with player's chosen 'Choice' & interpret's it.
        /// </summary>
        /// <param name="i">int i</param>
        private void HandleChoice(int i)
        {
            ChoiceStmt iStmt = Env.GetLocalChoice(i - 1);
            iStmt.Body.Interpret(Env);
        }

        private void HandleCommand(string choice)
        {
            List<string> command = [.. choice.Split(' ')];
            // Check for the noun in dict.
            if (!Env.HasLocalCommand(command[1], out CommandStmt? cStmt))
            {
                Console.WriteLine($"Unrecognised Noun: {command[1]}.");
                return;
            }
            // Chech for verb on noun.
            if (cStmt is null || !cStmt.Verbs.TryGetValue(command[0], out IStmt? vStmt))
            {
                Console.WriteLine($"Unrecognised Verb: {command[0]} for Noun: {command[1]}.");
                return;
            }
            vStmt.Interpret(Env);
        }

        /// <summary>
        /// Helper func before running overlay: copies the local env & wipes it
        /// s.t. the overlay is clear & the scene can be repopulated once the overlay is left.
        /// </summary>
        /// <param name="overlay"></param>
        private void HandleOverlay(OverlayStmt overlay)
        {
            // Copy interactables to re-load after overlay closes.
            ChoiceStmt[] interactables = [.. Env.LocalChoices];
            Dictionary<string, CommandStmt> localCommands = Env.LocalCommands;
            RunOverlay(overlay);
            // Clear locals from overlay & re-fill with this scene's.
            Env.ClearLocal();
            Env.AddLocalChoice(interactables);
            Env.AddLocalCommand(localCommands);
            ShowChoices();
        }

        /// <summary>
        /// Overlays can be opened on top of scenes. They function similarly, but can be exited to return to the base scene.
        /// </summary>
        /// <param name="overlay"></param>
        private void RunOverlay(OverlayStmt overlay)
        {
            Env.ClearLocal();
            Console.WriteLine("\n");
            overlay.Body.Interpret(Env);
            ShowChoices();
            while (true)
            {
                Console.Write("Enter your Selection: ");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int i) && Env.HasLocalChoice(i))
                {
                    HandleChoice(i);
                }
                // Scenes that are accessible by the user are by default exitable too, using the same keybind.
                else if (overlay.KeyBind is not null && choice == overlay.KeyBind)
                {
                    Console.WriteLine("\n");
                    break;
                }
                else { Console.WriteLine("Error, invalid selection."); }

                if (Env.CheckGoToFlag()) { break; }
            }
        }
    }
}