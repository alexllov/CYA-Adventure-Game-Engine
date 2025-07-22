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

        private string GoToAddress;

        public Interpreter(AbstSyntTree Tree, Environment env, string mode = "default")
        {
            AST = Tree;
            Env = env;
            DebugMode = mode == "debug" ? true : false;
        }

        public void Interpret()
        {
            /*
             * Prior Set-up to hoist Scenes.
             * This allows scenes to be written in any order & ensure the GoTos function.
             */
            HoistScenes();
            int startCount = AST.Tree.Count(s => s is StartStmt);
            if (startCount != 1) { throw new Exception($"Warning, a Game file needs exactly 1 'START' command. {startCount} were found."); }
            foreach (IStmt stmt in AST)
            {
                stmt.Interpret(Env);
            }
        }

        private void HoistScenes()
        {
            List<SceneStmt> sceneStmts = new();
            List<IStmt> generalStmts = new();
            List<IStmt> startStmts = new();
            foreach (IStmt stmt in AST)
            {
                if (stmt is SceneStmt scene) { sceneStmts.Add(scene); }
                else if (stmt is StartStmt start) { startStmts.Add(start); }
                else { generalStmts.Add(stmt); }
            }
            if (startStmts.Count > 1) { throw new Exception("Error, multiple Start points declared. There can only be one"); }
            AST = new AbstSyntTree(sceneStmts.Concat(generalStmts).Concat(startStmts).ToList());
        }

        // TODO:
        //Fully Separate Run Logic.
        // After the foreach loop for stmts from the AST is completed,
        // -> RunGame & use the Global Env created from intrepreting in there.
        public void RunGame()
        {
            SceneStmt scene = RunGoTo();
            while (true)
            {
                scene = RunScene(scene);
            }
        }

        private SceneStmt RunGoTo()
        {
            SceneStmt nextScene = Env.GetScene(Env.GetGoTo());
            return nextScene;
        }

        private void ShowOptions()
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

        private SceneStmt RunScene(SceneStmt scene)
        {
            string localScene = scene.Name;
            // Reset local scope.
            Env.ClearLocal();
            Console.WriteLine("\n========================================\n");
            scene.Body.Interpret(Env);

            ShowOptions();
            while (true)
            {
                string text = "Enter your ";
                if (Env.LocalChoices.Count > 0 && Env.LocalCommands.Count == 0) { text += "choice: "; }
                else if (Env.LocalChoices.Count == 0 && Env.LocalCommands.Count > 0) { text += "command: "; }
                else { text += "choice or command: "; }
                Console.Write(text);
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int i) && Env.HasLocalChoice(i))
                {
                    ChoiceStmt istmt = Env.GetLocalChoice(i - 1);
                    RunInteractable(istmt);
                    if (Env.CheckGoToFlag()) { break; }
                }
                else if (choice is not null && Env.CheckAccessibleOverlay(choice, out OverlayStmt? overlay))
                {
                    // Copy interactables to re-load after overlay closes.
                    ChoiceStmt[] interactables = [.. Env.LocalChoices];
                    RunOverlay(overlay);
                    // Clear locals from overlay & re-fill with this scene's.
                    Env.ClearLocal();
                    Env.AddLocalChoice(interactables);
                    ShowOptions();
                }
                // Command Logic.
                else if (choice is not null && choice.Split(' ').Length == 2)
                {
                    List<string> command = [.. choice.Split(' ')];
                    // Check for the noun in dict.
                    if (Env.HasLocalCommand(command[1], out CommandStmt? cStmt))
                    {
                        // Chech for verb on noun.
                        if (cStmt is not null && cStmt.Verbs.TryGetValue(command[0], out IStmt? vStmt))
                        {
                            vStmt.Interpret(Env);
                            if (Env.CheckGoToFlag()) { break; }
                        }
                        else { Console.WriteLine($"Unrecognised Verb: {command[0]} for Noun: {command[1]}."); }
                    }
                    else
                    {
                        Console.WriteLine($"Unrecognised Noun: {command[1]}.");
                    }
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
            return Env.GetScene(Env.GetGoTo());
        }

        private void RunInteractable(ChoiceStmt stmt)
        {
            stmt.Body.Interpret(Env);
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
            ShowOptions();
            while (true)
            {
                Console.Write("Enter your Selection: ");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int i) && Env.HasLocalChoice(i))
                {
                    ChoiceStmt istmt = Env.GetLocalChoice(i - 1);
                    RunInteractable(istmt);
                    if (Env.CheckGoToFlag()) { break; }
                }
                // Scenes that are accessible by the user are by default exitable too, using the same keybind.
                else if (overlay.KeyBind is not null && choice == overlay.KeyBind)
                {
                    Console.WriteLine("\n");
                    break;
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
        }
    }
}