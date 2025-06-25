using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal interface PrefixPart
    {
        Expr Parse(Parser parser, Token token);
    }

    public class NamePart : PrefixPart
    {
        public Expr Parse(Parser parser, Token token)
        {
            switch (token.Type)
            {
                case TokenType.Identifier:
                    return new StringLitExpr(token.Lexeme);
                case TokenType.Number:
                    return new NumberLitExpr(double.Parse(token.Lexeme));
                // May need to modify this depending on how Vars are to be stored.
                case TokenType.String:
                    return new VariableExpr(token.Lexeme);
                default:
                    throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}");
            }
        }
    }

    public class PrefixOperatorPart : PrefixPart
    {
        public Expr parse(Parser parser, Token token)
        {
            // Consume the operator token.
            parser.Advance();
            // Parse the next part of the expression.
            Expr operand = parser.ParsePrefixPart();
            // Return a new prefix expression with the operator and right side.
            return new PrefixExpr(token.Type, operand);
        }
    }





}