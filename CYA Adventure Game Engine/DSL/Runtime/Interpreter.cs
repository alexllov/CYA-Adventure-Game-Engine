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
            Console.WriteLine("\nOptions:");
            int num = 1;
            foreach (InteractableStmt interactable in Env.Local)
            {
                Console.WriteLine($"{num}. {interactable.Name.Interpret(Env)}");
                num++;
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
                Console.Write("Enter your Selection: ");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int i) && Env.HasLocal(i))
                {
                    InteractableStmt istmt = Env.GetLocal(i - 1);
                    RunInteractable(istmt);
                    if (Env.CheckGoToFlag()) { break; }
                }
                else if (choice is not null && Env.CheckAccessibleOverlay(choice, out OverlayStmt? overlay))
                {
                    // Copy interactables to re-load after overlay closes.
                    InteractableStmt[] interactables = [.. Env.Local];
                    RunOverlay(overlay);
                    // Clear locals from overlay & re-fill with this scene's.
                    Env.ClearLocal();
                    Env.AddLocal(interactables);
                    ShowOptions();
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
            return Env.GetScene(Env.GetGoTo());
        }

        private void RunInteractable(InteractableStmt stmt)
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
                if (int.TryParse(choice, out int i) && Env.HasLocal(i))
                {
                    InteractableStmt istmt = Env.GetLocal(i - 1);
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