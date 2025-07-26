using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    /// <summary>
    /// Interface for Prefix Parcelets.
    /// Requirement: Parse method that takes a Parser and a Token, producing an expression (Expr).
    /// </summary>
    internal interface IPrefixParselet
    {
        IExpr Parse(Parser parser, Token token);
    }

    /// <summary>
    /// Handles parsing of names, numbers, and variables into the appropriate Expression types.
    /// These are grouped as the non-operator prefix expressions in the Pratt Parser.
    /// </summary>
    public class NameParselet : IPrefixParselet
    {
        public IExpr Parse(Parser parser, Token token)
        {
            return token.Type switch
            {
                TokenType.Identifier => new VariableExpr(token.Lexeme),
                TokenType.Number => new NumberLitExpr(float.Parse(token.Lexeme)),
                TokenType.String => new StringLitExpr(token.Lexeme),
                TokenType.Boolean => new BooleanExpr(token.Lexeme),
                _ => throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}"),
            };
        }
    }

    /// <summary>
    /// Handles prefix operators like '+' Plus, '-' Minus, & '!' Not.
    /// Currently these are the only three implemented.
    /// </summary>
    public class PrefixOperatorParselet : IPrefixParselet
    {
        readonly int _Precedence;

        public PrefixOperatorParselet(int precedence = Precedence.PREFIX)
        {
            _Precedence = precedence;
        }
        public IExpr Parse(Parser parser, Token token)
        {
            // Parse the next part of the expression.
            IExpr operand = parser.ParseExpression(_Precedence);
            // Return a new prefix expression with the operator and right side.
            return new PrefixExpr(token.Type, operand);
        }
    }

    // TODO: Add support for module.func style stuff.
    public class ParentParselet : IPrefixParselet
    {

        readonly int _Precedence;

        public ParentParselet(int precedence = Precedence.PREFIX)
        {
            _Precedence = precedence;
        }

        public IExpr Parse(Parser parser, Token token)
        {
            List<IExpr> parts = [];
            while (parser.Peek(0).Type is not TokenType.RParent)
            {
                IExpr part = parser.ParseExpression(0);
                parts.Add(part);
            }
            parser.Consume(TokenType.RParent);

            switch (parts[0])
            {
                case BinaryExpr:
                    return parts[0];
                default:
                    IExpr function = parts[0];
                    if (parts.Count > 1)
                    {
                        List<IExpr> args = parts[1..];
                        return new FuncExpr(function, args);
                    }
                    else
                    {
                        return new FuncExpr(function);
                    }
            }
        }
    }
}