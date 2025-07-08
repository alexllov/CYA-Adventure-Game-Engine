using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal class Interpreter
    {
        // TODO: Fix type declr
        public string Code;
        // TODO: Properly set up AST && Appending stuff for parser s.t. this can take an AST proper.
        public List<Node> baseAST;

        // Base default functions.
        public List<string> DefaultFuncs = new List<string>
        {
            "say",
            "ask",
            "save",
            "back",
        };

        public Interpreter(List<Node> Tree)
        {
            baseAST = Tree;
            Interpret();
        }

        public void Interpret()
        {
            foreach (var stmt in baseAST)
            {
                Evaluate(stmt);
            }
        }

        /// <summary>
        /// Evaluates each Stmt & Expr within the AST.
        /// Uses generalized type 'Node' s.t. recursive calls can extrace Expr values from within stmts.
        /// </summary>
        /// <param name="node">Node: the next node within the AST to be parsed</param>
        /// <exception cref="Exception"></exception>
        private void Evaluate(Node node)
        { }
        //    switch (node)
        //    {
        //        case FuncExprStmt funcStmt:
        //            if (funcStmt._Expr.Method is VariableExpr &&
        //                DefaultFuncs.Contains(funcStmt._Expr.Method.ToString()))
        //            {
        //                switch (funcStmt._Expr.Method.ToString())
        //                {
        //                    case "say":
        //                        if (funcStmt._Expr.Arguments == null)
        //                        {
        //                            throw new Exception("Error, received a 'say' func missing required argument.");
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine($"{string.Join("", funcStmt._Expr.Arguments)}");
        //                        }
        //                        break;
        //                    default:
        //                        throw new Exception("Error, received an unknown default func?? (This should not be possible.)");
        //                }
        //            }
        //            break;
        //
        //        //case BinaryStmt binaryStmt:
        //        //    Console.WriteLine($"Binary Expression: {binaryExpr.Left} {binaryExpr.Operator} {binaryExpr.Right}");
        //        //    break;
        //
        //
        //        // Should Consist of BinaryExpr, PrefixExpr, AssignExpr.
        //        case ExprStmt exprStmt:
        //            switch (exprStmt._Expr)
        //            {
        //                case BinaryExpr bExpr:
        //                    ProcessBinaryExpr(bExpr);
        //                    break;
        //                case PrefixExpr:
        //                    break;
        //                case AssignExpr:
        //                    break;
        //            }
        //            break;
        //    }
        //}

        //private void ProcessBinaryExpr(BinaryExpr expr)
        //{
        //    switch (expr.Operator)
        //    {
        //        case TokenType.Equal:
        //            Console.WriteLine($"Binary Expression: {expr.Left} == {expr.Right}");
        //            break;
        //        case TokenType.GreaterEqual:
        //            Console.WriteLine($"Binary Expression: {expr.Left} >= {expr.Right}");
        //            break;
        //        case TokenType.GreaterThan:
        //            Console.WriteLine($"Binary Expression: {expr.Left} > {expr.Right}");
        //            break;
        //        case TokenType.LessEqual:
        //            Console.WriteLine($"Binary Expression: {expr.Left} <= {expr.Right}");
        //            break;
        //        case TokenType.LessThan:
        //            Console.WriteLine($"Binary Expression: {expr.Left} < {expr.Right}");
        //            break;
        //        case TokenType.Plus:
        //            Console.WriteLine($"Binary Expression: {expr.Left} + {expr.Right}");
        //            break;
        //        case TokenType.Minus:
        //            Console.WriteLine($"Binary Expression: {expr.Left} - {expr.Right}");
        //            break;
        //        case TokenType.Multiply:
        //            Console.WriteLine($"Binary Expression: {expr.Left} * {expr.Right}");
        //            break;
        //        case TokenType.Divide:
        //            Console.WriteLine($"Binary Expression: {expr.Left} / {expr.Right}");
        //            break;
        //        default:
        //            Console.WriteLine($"Unknown Operator: {expr.Operator}");
        //            break;
        //    }
        //}

        //private void ProcessBinaryExpr(BinaryExpr expr)
        //{
        //    Expr left = Evaluate((Expr)expr.Left);
        //}
    }
}
