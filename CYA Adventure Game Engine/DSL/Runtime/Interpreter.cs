using CYA_Adventure_Game_Engine.DSL.Frontend;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Tracing;
using System.Linq;
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

        // Binary Operator SubTypes.
        public Dictionary<string, List<TokenType>> BinaryOperators = new()
        {
            { "arithmetic", [TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide] },
            { "relational", [TokenType.Equal,TokenType.NotEqual, TokenType.GreaterEqual, TokenType.GreaterThan, TokenType.LessEqual, TokenType.LessThan] },
            { "logical", [TokenType.And, TokenType.Or] },
        };

        private Environment Env;

        public Interpreter(List<Stmt> Tree, Environment env, string mode="default")
        {
            AST = Tree;
            Env = env;
            DebugMode = mode == "debug" ? true : false;
        }

        public void Interpret()
        {
            /*
             * Prior Set-up to simulate hoisting of Scenes.
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
            foreach (Stmt stmt in AST)
            {
                if (stmt is SceneStmt sstmt) { sceneStmts.Add(sstmt); }
                else { generalStmts.Add(stmt); }
            }
            AST = sceneStmts.Concat(generalStmts).ToList();
        }

        /// <summary>
        /// Evaluates each Stmt & Expr within the AST.
        /// Uses generalized type 'Node' s.t. recursive calls can extrace Expr values from within stmts.
        /// </summary>
        /// <param name="node">Node: the next node within the AST to be parsed</param>
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

                case ExprStmt:
                    Expr expr = ((ExprStmt)stmt).Expr;
                    EvaluateExpr(expr);
                    break;

                default:
                    throw new Exception($"Unknown Node type encountered: {stmt}, type: {stmt.GetType()}");
            }
        }

        private object EvaluateExpr(Expr expr)
        {
            switch (expr)
            {
                case AssignExpr:
                    throw new Exception("Untreated Assign Expression found. Parser Problem.");
                case BinaryExpr bExpr:
                    object bResult = ProcessBinaryExpr(bExpr);
                    if (DebugMode) { Console.WriteLine(bResult); }
                    return bResult;
                case FuncExpr fExpr:
                    return ProcessFuncExpr(fExpr);
                // TODO: Figure out what to do with this.
                /*
                 * Return true; is in as a placeholder.
                 * This might just endlessly nest into itself.
                 */
                case GoToExpr gtExpr:
                    BlockStmt nextScene = ProcessGoToExpr(gtExpr);
                    RunScene(nextScene);
                    return true;
                case NumberLitExpr num:
                    return num.Value;
                case PrefixExpr pExpr:
                    object pResult = ProcessPrefixExpr(pExpr);
                    if (DebugMode) { Console.WriteLine(pResult); }
                    return pResult;
                case StringLitExpr str:
                    return str.Value;
                case VariableExpr variable:
                    return Env.GetVal(variable.Value);
                default:
                    throw new Exception($"Unknown Expr type detected. Expr: {expr.GetType()}");
            }
        }

        private void AssignSceneStmt(SceneStmt stmt)
        {
            Env.SetScene(stmt.Name, stmt.Body);
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
            Console.WriteLine($"Condition = {condition}");
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

        /// <summary>
        /// Helper func. Pass a type & list of args. If any are not of the expected type, Error thrown.
        /// </summary>
        /// <param name="type">object type</param>
        /// <param name="items">list of args</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool CheckType(Type type, params object[] items)
        {
            foreach (object item in items) 
            { 
                if (!type.IsInstanceOfType(item))
                {
                    throw new Exception($"Invalid argument type detected. Expeected {type}, but got {item.GetType()} instead.");
                }
            }
            return true;
        }


        private object ProcessBinaryExpr(BinaryExpr expr)
        {
            var left = EvaluateExpr(expr.Left);
            var right = EvaluateExpr(expr.Right);

            if (BinaryOperators["arithmetic"].Contains(expr.Operator))
            {
                // Check type validity for numeric operators
                CheckType(typeof(double), [left, right]);

                switch (expr.Operator)
                {
                    case TokenType.Plus:
                        return (double)left + (double)right;
                    case TokenType.Minus:
                        return (double)left - (double)right;
                    case TokenType.Multiply:
                        return (double)left * (double)right;
                    case TokenType.Divide:
                        if ((double)right == 0) { throw new Exception("Division by 0 error."); }
                        return (double)left / (double)right;
                    default:
                        throw new Exception("How did we end up here? Valid arithmetic operator detected but not present");
                }
            }
            else if (BinaryOperators["relational"].Contains(expr.Operator))
            {
                switch (expr.Operator)
                { 
                    case TokenType.Equal:
                        Console.WriteLine($"Left: {left.GetType()}");
                        Console.WriteLine($"Right: {right.GetType()}");
                        if (left == right) { Console.WriteLine("They are equal"); }
                        return left.Equals(right);
                    case TokenType.NotEqual:
                        return !(left.Equals(right));
                    case TokenType.GreaterEqual:
                        CheckType(typeof(double), [left, right]);
                        return (double)left >= (double)right;
                    case TokenType.GreaterThan:
                        CheckType(typeof(double), [left, right]);
                        return (double)left > (double)right;
                    case TokenType.LessEqual:
                        CheckType(typeof(double), [left, right]);
                        return (double)left <= (double)right;
                    case TokenType.LessThan:
                        CheckType(typeof(double), [left, right]);
                        return (double)left < (double)right;
                    default:
                        throw new Exception("How did we end up here? Valid relational operator detected but not present");
                }
            }
            else if (BinaryOperators["logical"].Contains(expr.Operator))
            {
                switch (expr.Operator) 
                {
                    case TokenType.And:
                        CheckType(typeof(bool), [left, right]);
                        return (bool)left && (bool)right;
                    case TokenType.Or:
                        CheckType(typeof(bool), [left, right]);
                        return (bool)left || (bool)right;
                    default:
                        throw new Exception("How did we end up here? Valid logical detected but not present");
                }
            }
            else
            {
                throw new Exception("Invalid binary expression operator found.");
            }
        }


        private object ProcessPrefixExpr(PrefixExpr expr)
        {
            var operand = EvaluateExpr(expr.Operand);
            switch (expr.Operator)
            {
                case TokenType.Plus:
                    return operand;
                case TokenType.Minus:
                    if (operand is not double)
                    {
                        throw new Exception($"Invalid value taking '-' prefix of type: {operand.GetType()}");
                    }
                    else
                    {
                        return -(double)operand;
                    }
                case TokenType.Not:
                    if (operand is not bool)
                    {
                        throw new Exception($"Invalid value taking '!' prefix of type: {operand.GetType()}");
                    }
                    else
                    {
                        return !(bool)operand;
                    }
                default:
                    throw new Exception($"Error, prefix of type {expr.Operator.GetType()} not yet supported.");
            }
        }

        private object ProcessFuncExpr(FuncExpr expr)
        {
            // TODO: this needs reworign s.t. proper funcs & dot funcs will work & other things can throw appropriate errors.
            var function = EvaluateExpr(expr.Method);
            List<object> args = new List<object>();
            foreach (Expr arg in expr.Arguments) 
            {
                args.Add(EvaluateExpr(arg));
            }
            if (function is Func<List<object>,object> multArgFunc) 
            {
                return multArgFunc(args); 
            }
            else if (function is Func<object> arglessFunc)
            {
                return arglessFunc();
            }
            throw new Exception("Function call of unsupported argument type found.");
        }

        private BlockStmt ProcessGoToExpr(GoToExpr expr)
        {
            string SceneAddr = (string)EvaluateExpr(expr.Location);
            Console.WriteLine($"SceneAddr: {SceneAddr}");
            BlockStmt nextScene = Env.GetScene(SceneAddr);
            return nextScene;
        }

        private void RunScene(BlockStmt scene)
        {
            // Reset local scope.
            Env.ClearLocal();

            foreach (Stmt stmt in scene.Statements)
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
                }
                else { Console.WriteLine("Error, invalid selection."); }
            }
        }

        private void RunInteractable(InteractableStmt stmt)
        {
            EvaluateStmt(stmt.Body);
        }
    }
}