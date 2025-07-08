using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal class Interpreter
    {
        // TODO: Fix type declr
        public string Code;
        // TODO: Properly set up AST && Appending stuff for parser s.t. this can take an AST proper.
        public List<Node> AST;

        // Base default functions.
        public List<string> DefaultFuncs = new List<string>
        {
            "say",
            "ask",
            "save",
            "back",
        };

        public Dictionary<string, List<TokenType>> BinaryOperators = new()
        {
            { "arithmetic", [TokenType.Plus, TokenType.Minus, TokenType.Multiply, TokenType.Divide] },
            { "relational", [TokenType.Equal,TokenType.NotEqual, TokenType.GreaterEqual, TokenType.GreaterThan, TokenType.LessEqual, TokenType.LessThan] },
            { "logical", [TokenType.And, TokenType.Or] },
        };

        public Interpreter(List<Node> Tree)
        {
            AST = Tree;
            Interpret();
        }

        public void Interpret()
        {
            foreach (Node node in AST)
            {
                object ans = Evaluate(node);
                Console.WriteLine(ans);
            }
        }

        /// <summary>
        /// Evaluates each Stmt & Expr within the AST.
        /// Uses generalized type 'Node' s.t. recursive calls can extrace Expr values from within stmts.
        /// </summary>
        /// <param name="node">Node: the next node within the AST to be parsed</param>
        /// <exception cref="Exception"></exception>
        private object Evaluate(Node node)
        {
            switch (node)
            {
                //case FuncExpr func:
                //    if (func._Expr.Method is VariableExpr &&
                //        DefaultFuncs.Contains(func._Expr.Method.ToString()))
                //    {
                //        switch (func._Expr.Method.ToString())
                //        {
                //            case "say":
                //                if (func._Expr.Arguments == null)
                //                {
                //                    throw new Exception("Error, received a 'say' func missing required argument.");
                //                }
                //                else
                //                {
                //                    Console.WriteLine($"{string.Join("", func._Expr.Arguments)}");
                //                }
                //                break;
                //            default:
                //                throw new Exception("Error, received an unknown default func?? (This should not be possible.)");
                //        }
                //    }
                //    break;

                //case BinaryStmt binaryStmt:
                //    Console.WriteLine($"Binary Expression: {binaryExpr.Left} {binaryExpr.Operator} {binaryExpr.Right}");
                //    break;


                // Should Consist of BinaryExpr, PrefixExpr, AssignExpr.
                case Expr expr:
                    switch (expr)
                    {
                        case BinaryExpr bExpr:
                            return ProcessBinaryExpr(bExpr);
                        case PrefixExpr pExpr:
                            return ProcessPrefixExpr(pExpr);
                        case AssignExpr:
                            break;
                        case NumberLitExpr num:
                            return num.Value;
                    }
                    break;
            }
            throw new Exception($"Unknown Node type encountered: {node}, type: {node.GetType()}");
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
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

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
                        return left == right;
                    case TokenType.NotEqual:
                        return left != right;
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
            var operand = Evaluate(expr.Operand);
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
    }
}