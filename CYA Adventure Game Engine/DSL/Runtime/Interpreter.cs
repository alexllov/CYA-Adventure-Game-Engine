using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Frontend.AST;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    internal class Interpreter
    {
        public List<Stmt> AST;

        // Debug = print Expr results.
        private bool DebugMode;

        private Environment Env;

        private string GoToAddress;

        public Interpreter(List<Stmt> Tree, Environment env, string mode="default")
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
            foreach (Stmt stmt in AST)
            {
                stmt.Interpret(Env);
            }
        }

        private void HoistScenes()
        {
            List<SceneStmt> sceneStmts = new();
            List<Stmt> generalStmts = new();
            List<Stmt> startStmts = new();
            foreach (Stmt stmt in AST)
            {
                if (stmt is SceneStmt scene) { sceneStmts.Add(scene); }
                else if (stmt is StartStmt start) { startStmts.Add(start); }
                else { generalStmts.Add(stmt); }
            }
            if (startStmts.Count > 1) { throw new Exception("Error, multiple Start points declared. There can only be one"); }
            AST = sceneStmts.Concat(generalStmts).Concat(startStmts).ToList();
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

        private SceneStmt RunScene(SceneStmt scene)
        {
            string localScene = scene.Name;
            // Reset local scope.
            Env.ClearLocal();

            scene.Body.Interpret(Env);
            
            Console.WriteLine("\nOptions:");
            int num = 1;
            foreach (InteractableStmt interactable in Env.Local)
            {
                Console.WriteLine($"{num}. {interactable.Name.Interpret(Env)}");
                num++;
            }

            while (true)
            {
                Console.Write("Enter your Selection: ");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int i) && Env.HasLocal(i)) 
                {
                    InteractableStmt istmt = Env.GetLocal(i-1);
                    RunInteractable(istmt);
                    if (Env.GetGoTo() != localScene) { break; }
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
            return Env.GetScene(Env.GetGoTo());
        }

        private void RunInteractable(InteractableStmt stmt)
        {
            stmt.Body.Interpret(Env);
        }
    }
}