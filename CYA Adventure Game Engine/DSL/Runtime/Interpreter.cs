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
            int startCount = AST.Tree.Count(s => s is GoToStmt);
            if (startCount > 1) { throw new Exception($"Warning, a Game file at most 1 'START' command. {startCount} were found."); }
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

            while (true)
            {
                if (Env.LocalChoices.Count == 0
                && !AnyActiveChoicers(out List<IChoiceHandler> _)
                && Env.CheckGoToFlag())
                {
                    break;
                }

                // Show the available native choices for the scene (if any).
                ShowChoices();

                // Construct the input prompt.
                string text = Env switch
                {
                    { LocalChoices.Count: > 0 } => Env.ChoiceHandlers
                        .Aggregate("Enter your choice", (prev, curr) => curr.GetUserFacingText(prev)) + ": ",
                    { LocalChoices.Count: 0 } => Env.ChoiceHandlers
                        .Aggregate("", (prev, curr) => curr.GetUserFacingText(prev)) + ": ",
                };

                // If the choices are empty, then construct an end message.
                if (text == ": ")
                {
                    if (Env.AccessibleOverlays.Count > 0)
                    {
                        text = "You have reached an end. Select an overlay: ";
                    }
                    text = "You have reached an end.";
                }

                Console.Write(text);

                // Get & process user input.
                var choice = Console.ReadLine();

                // Choice.
                if (int.TryParse(choice, out int i) && Env.HasLocalChoice(i))
                {
                    HandleChoice(i);
                }
                // Accessible Overlay.
                else if (choice is not null && Env.CheckAccessibleOverlay(choice, out OverlayStmt? overlay))
                {
                    HandleOverlay(overlay!);
                }

                // Command.
                else if (choice is not null
                         && AnyActiveChoicers(out List<IChoiceHandler> activeChoicers))
                {
                    HandleCommand(activeChoicers, choice);
                    if (Env.CheckSuccessfulCommands())
                    {
                        // Consider doing some sort of "errors from latest" check
                        // to ensure the breaking CH didn't have any errors.
                        // Current: successfuls only added after all errors -> early return,
                        // however this means that needs to be properly implemented by every module writer.
                        foreach (IStmt stmt in Env.GetSuccessfulCommands())
                        {
                            stmt.Interpret(Env);
                        }
                    }
                    else
                    {
                        foreach (string error in Env.GetCommandErrors())
                        {
                            Console.WriteLine(error);
                        }
                    }
                }
                else { Console.WriteLine("Error, invalid selection."); }

                // Check if overlay triggered by a Choice or Command with a 'run' stmt attached.
                if (Env.CheckRunOverlayFlag(out OverlayStmt? oStmt))
                {
                    HandleOverlay(oStmt!);
                }

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

        /// <summary>
        /// Scan through the held IChoiceHandler modules. If any are active, return true.
        /// Any actives are added to activeChoicers list.
        /// </summary>
        /// <param name="activeChoicers"></param>
        /// <returns></returns>
        private bool AnyActiveChoicers(out List<IChoiceHandler> activeChoicers)
        {
            activeChoicers = [];
            foreach (IChoiceHandler choicer in Env.ChoiceHandlers)
            {
                if (choicer.IsActive())
                {
                    activeChoicers.Add(choicer);
                }
            }
            if (activeChoicers.Count > 0) { return true; }
            return false;
        }

        /// <summary>
        /// Calls on each 'active' IChoiceHandler to attempt to handle the user's input.
        /// </summary>
        /// <param name="choicers"></param>
        /// <param name="choice"></param>
        private void HandleCommand(List<IChoiceHandler> choicers, string choice)
        {
            // RESET COMMAND ERRORS & SUCCESSFUL COMMANDS.
            foreach (IChoiceHandler choiceHandler in choicers)
            {
                choiceHandler.HandleCommand(choice);
                if (Env.CheckSuccessfulCommands()) { break; }
            }
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
            foreach (IChoiceHandler choicer in Env.ChoiceHandlers) { choicer.StoreLocal(); }

            RunOverlay(overlay);

            // Clear locals from overlay & re-fill with this scene's.
            Env.ClearLocal();
            Env.AddLocalChoice(interactables);
            foreach (IChoiceHandler choicer in Env.ChoiceHandlers) { choicer.DumpLocal(); }
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

            while (true)
            {
                ShowChoices();
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

                // If a GoTo or Exit statement was executed, break from the loop to leave the overlay.
                if (Env.CheckGoToFlag() || Env.CheckOverlayExitFlag()) { break; }

                // Check if overlay triggered by a Choice or Command with a 'run' stmt attached.
                // Directly run rather than handle s.t. local scope for the root scene isn't interfered with.
                if (Env.CheckRunOverlayFlag(out OverlayStmt? oStmt))
                {
                    ChoiceStmt[] choices = [.. Env.LocalChoices];
                    RunOverlay(oStmt!);
                    Env.ClearLocal();
                    Env.AddLocalChoice(choices);
                }
            }
        }
    }
}