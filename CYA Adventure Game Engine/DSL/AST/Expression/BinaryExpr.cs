using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using CYA_Adventure_Game_Engine.DSL.Runtime;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Binary operations: e.g. 1 + 2,
    /// Made up of a left side (expr), an operator (token), & right side (expr).
    /// </summary>
    public class BinaryExpr : IExpr
    {
        public IExpr Left;
        public TokenType Operator;
        public IExpr Right;

        public BinaryExpr(IExpr left, TokenType oper, IExpr right)
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

        private List<float> CoerceToFloat(object[] items)
        {
            List<float> results = new();
            foreach (object item in items)
            {
                if (float.TryParse(item.ToString(), out var flt)) { results.Add(flt); }
                else { throw new Exception($"Cannot coerce {item} to float."); }
            }
            return results;
        }

        public object Interpret(Environment state)
        {
            var left = Left.Interpret(state);
            var right = Right.Interpret(state);

            if (BinaryOperatorType.BinaryOperators["arithmetic"].Contains(Operator))
            {
                // Check type validity for numeric operators
                //CheckType(typeof(float), [left, right]);

                List<float> result = CoerceToFloat([left, right]);
                left = result[0];
                right = result[1];

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
                        return !left.Equals(right);
                    case TokenType.GreaterEqual:
                        List<float> result = CoerceToFloat([left, right]);
                        return (float)result[0] >= (float)result[1];
                    case TokenType.GreaterThan:
                        result = CoerceToFloat([left, right]);
                        return (float)result[0] > (float)result[1];
                    case TokenType.LessEqual:
                        result = CoerceToFloat([left, right]);
                        return (float)result[0] <= (float)result[1];
                    case TokenType.LessThan:
                        result = CoerceToFloat([left, right]);
                        return (float)result[0] < (float)result[1];
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
}
