using CYA_Adventure_Game_Engine.DSL.Frontend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains a prefix and a value:
    /// e.g. '-1', '!true', '+a'...
    /// </summary>
    public class PrefixExpr : IExpr
    {
        public TokenType Operator;
        public IExpr Operand;
        public PrefixExpr(TokenType type, IExpr operand)
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
}
