
using CYA_Adventure_Game_Engine.DSL.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST
{
    // =============== Abstract ===============

    // Expression: evaluates to a value.
    public interface Expr 
    {
        public abstract object Interpret(Environment state);
    }


    // =============== Expressions ===============

    // Literal values: number, string lit...
    public class NumberLitExpr : Expr
    {
        public float Value;
        public NumberLitExpr(float val)
        {
            Value = val;
        }

        public override string ToString()
        {
            return $"NumberLitExpr({Value})";
        }

        public object Interpret(Environment state) { return Value; }
    }

    public class StringLitExpr : Expr
    {
        public string Value;
        public StringLitExpr(string val)
        {
            Value = val;
        }
        public override string ToString()
        {
            // TODO: Set up a DebugToString for parser purposes that contians this return.
            return $"StringLitExpr({Value})";
        }

        public object Interpret(Environment state) { return Value; }
    }

    // Variable: named values.
    public class VariableExpr : Expr
    {
        public string Value;
        public VariableExpr(string name)
        {
            Value = name;
        }
        public override string ToString()
        {
            return $"{Value}";
        }

        public object Interpret(Environment state) { return state.GetVal(Value); }
    }

    // Prefix Expr (operands for Pratt Parser): e.g. '-'1, '!'true, '+'a...
    public class PrefixExpr : Expr
    {
        public TokenType Operator;
        public Expr Operand;
        public PrefixExpr(TokenType type, Expr operand)
        {
            Operator = type;
            Operand = operand;
        }

        public override string ToString()
        {
            return $"PrefixExpr({Operator}, {Operand})";
        }

        public object Interpret(Environment state)
        {
            var operand = Operand.Interpret(state);
            switch (Operator)
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
                    throw new Exception($"Error, prefix of type {Operator.GetType()} not yet supported.");
            }
        }
    }

    // Binary operations: e.g. 1 + 2,
    // made up of a left side, an operator, & right side.
    public class BinaryExpr : Expr
    {
        public Expr Left;
        public TokenType Operator;
        public Expr Right;

        public BinaryExpr(Expr left, TokenType oper, Expr right)
        {
            Left = left;
            Operator = oper;
            Right = right;
        }
        public override string ToString()
        {
            return $"BinaryExpr({Left}, {Operator}, {Right})";
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

        public object Interpret(Environment state)
        {
            var left = Left.Interpret(state);
            var right = Right.Interpret(state);

            if (BinaryOperatorType.BinaryOperators["arithmetic"].Contains(Operator))
            {
                // Check type validity for numeric operators
                CheckType(typeof(float), [left, right]);

                switch (Operator)
                {
                    case TokenType.Plus:
                        return (float)left + (float)right;
                    case TokenType.Minus:
                        return (float)left - (float)right;
                    case TokenType.Multiply:
                        return (float)left * (float)right;
                    case TokenType.Divide:
                        if ((float)right == 0) { throw new Exception("Division by 0 error."); }
                        return (float)left / (float)right;
                    default:
                        throw new Exception("How did we end up here? Valid arithmetic operator detected but not present");
                }
            }
            else if (BinaryOperatorType.BinaryOperators["relational"].Contains(Operator))
            {
                switch (Operator)
                {
                    case TokenType.Equal:
                        return left.Equals(right);
                    case TokenType.NotEqual:
                        return !(left.Equals(right));
                    case TokenType.GreaterEqual:
                        CheckType(typeof(float), [left, right]);
                        return (float)left >= (float)right;
                    case TokenType.GreaterThan:
                        CheckType(typeof(float), [left, right]);
                        return (float)left > (float)right;
                    case TokenType.LessEqual:
                        CheckType(typeof(float), [left, right]);
                        return (float)left <= (float)right;
                    case TokenType.LessThan:
                        CheckType(typeof(float), [left, right]);
                        return (float)left < (float)right;
                    default:
                        throw new Exception("How did we end up here? Valid relational operator detected but not present");
                }
            }
            else if (BinaryOperatorType.BinaryOperators["logical"].Contains(Operator))
            {
                switch (Operator)
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
    }

    /// <summary>
    /// Used to register Identifiers to values, eg Variable assignment.
    /// </summary>
    public class AssignExpr : Expr
    {
        public Expr Name;
        public Expr Value;

        public AssignExpr(Expr name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"AssignExpr({Name}, {Value})";
        }

        public object Interpret(Environment state)
        {
            throw new Exception("Untreated Assign Expression found. Parser Problem.");
        }
    }

    public class DotExpr : Expr
    {
        public Expr Left;
        public Expr Right;
        public DotExpr(Expr left, Expr right)
        {
            Left = left;
            Right = right;
        }
        public override string ToString()
        {
            return $"DotExpr({Left}, {Right})";
        }

        public object Interpret(Environment state)
        {
            throw new Exception("Oops, this is not yet implemented.");
        }
    }

    public class FuncExpr : Expr
    {
        public Expr Method;
        public List<Expr> Arguments;
        public FuncExpr(Expr method, List<Expr> arguments = null)
        {
            Method = method;
            Arguments = arguments ?? [];
        }
        public override string ToString()
        {
            if (Arguments != null)
            {
                //foreach (Expr arg in Arguments)
                //{
                //    Console.WriteLine($"arg: {arg}, type: {arg.GetType()}");
                //}
                return $"FuncExpr(Method: {Method}, Arguments: [{string.Join(", ", Arguments)}])";
            }
            else
            {
                return $"FuncExpr(Method: {Method}";
            }
        }

        public object Interpret(Environment state)
        {
            // TODO: this needs reworign s.t. proper funcs & dot funcs will work & other things can throw appropriate errors.
            var function = Method.Interpret(state);
            List<object> args = [];
            foreach (Expr arg in Arguments)
            {
                args.Add(arg.Interpret(state));
            }
            if (function is Func<List<object>, object> multArgFunc)
            {
                return multArgFunc(args);
            }
            else if (function is Func<object> arglessFunc)
            {
                return arglessFunc();
            }
            throw new Exception("Function call of unsupported argument type found.");
        }
    }
}
