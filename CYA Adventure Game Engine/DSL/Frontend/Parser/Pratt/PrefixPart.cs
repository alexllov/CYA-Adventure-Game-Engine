using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt
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
                _ => throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}, in file: {token.SourceFile}"),
            };
        }
    }

    /*
     * The following 'PrefixOperatorParselet' class is adapted from a class of the same name from:
     * Author: jfcardinal
     * Source: https://github.com/jfcardinal/BantamCs
     * Used under MIT Lisence
     */
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

    /// <summary>
    /// Used to process expressions within parenthesis.
    /// Switch case used to distinguish between Polish function calls 
    /// & () around expressions to change binding.
    /// </summary>
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
            while (parser.Tokens.Peek(0).Type is not TokenType.RParent)
            {
                IExpr part = parser.ParseExpression(0);
                parts.Add(part);
            }
            parser.Tokens.Consume(TokenType.RParent);

            switch (parts[0])
            {
                // Assumes only one expr in the same parents, as (1+2 3*4) should be invalid anyway.
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