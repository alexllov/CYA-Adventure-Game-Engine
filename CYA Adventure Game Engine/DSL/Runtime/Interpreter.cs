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
                EvaluateStmt(stmt);
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

        /// <summary>
        /// Evaluates each <seealso cref="Stmt"/> within the AST.
        /// </summary>
        /// <param name="stmt">Stmt: the next statement within the AST to be parsed</param>
        /// <exception cref="Exception"></exception>
        private void EvaluateStmt(Stmt stmt)
        {
            switch (stmt)
            {
                case SceneStmt sstmt:
                    AssignSceneStmt(sstmt);
                    break;

                case InteractableStmt istmt:
                    AssignInteractableStmt(istmt);
                    break;

                case StartStmt start:
                    RunGame(start);
                    break;

                case GoToStmt goTo:
                    GoToAddress = (string)EvaluateExpr(goTo.Location);
                    break;

                // Should Consist of BinaryExpr, PrefixExpr, AssignExpr.
                case AssignStmt assStmt:
                    EvalAssignStmt(assStmt);
                    break;

                case BlockStmt bStmt:
                    EvalBlockStmt(bStmt);
                    break;

                case IfStmt istmt:
                    EvalIfStmt(istmt);
                    break;

                case ExprStmt exprStmt:
                    Expr expr = exprStmt.Expr;
                    EvaluateExpr(expr);
                    break;

                default:
                    throw new Exception($"Unknown Node type encountered: {stmt}, type: {stmt.GetType()}");
            }
        }

        private object EvaluateExpr(Expr expr)
        {
            return expr.Interpret(Env);
        }

        private void AssignSceneStmt(SceneStmt stmt)
        {
            Env.SetScene(stmt.Name, stmt);
        }

        public void AssignInteractableStmt(InteractableStmt stmt)
        {
            Env.AddLocal(stmt);
        }

        private void EvalAssignStmt(AssignStmt stmt)
        {
            if (stmt.Name is VariableExpr nameExpr)
            {
                string name = nameExpr.ToString();
                object value = EvaluateExpr(stmt.Value);
                Env.SetVal(name, value);
            }
        }

        private void EvalIfStmt(IfStmt stmt)
        {
            var condition = EvaluateExpr(stmt.Condition);
            if (condition is not bool) { throw new Exception("Error, If statement condition does not evaluate to true or false."); }
            if ((bool)condition)
            {
                EvaluateStmt(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch is not null)
            {
                EvaluateStmt(stmt.ElseBranch);
            }
        }

        public void EvalBlockStmt(BlockStmt block)
        {
            foreach (Stmt stmt in block)
            {
                EvaluateStmt(stmt);
            }
        }

        private SceneStmt RunGoTo()
        {
            SceneStmt nextScene = Env.GetScene(GoToAddress);
            return nextScene;
        }

        private void RunGame(StartStmt start)
        {
            GoToAddress = (string)EvaluateExpr(start.Location);
            SceneStmt scene = RunGoTo();
            while (true)
            {
                scene = RunScene(scene);
            }
        }

        private SceneStmt RunScene(SceneStmt scene)
        {
            string localScene = scene.Name;
            // Reset local scope.
            Env.ClearLocal();

            foreach (Stmt stmt in scene.Body)
            {
                EvaluateStmt(stmt);
            }
            Console.WriteLine("\nOptions:");
            int num = 1;
            foreach (InteractableStmt interactable in Env.Local)
            {
                Console.WriteLine($"{num}. {EvaluateExpr(interactable.Name)}");
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
                    if (GoToAddress != localScene) { break; }
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
            return Env.GetScene(GoToAddress);
        }

        private void RunInteractable(InteractableStmt stmt)
        {
            EvaluateStmt(stmt.Body);
        }
    }
}