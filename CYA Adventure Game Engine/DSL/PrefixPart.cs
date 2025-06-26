using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    /// <summary>
    /// Interface for Prefix Parcelets.
    /// Requirement: Parse method that takes a Parser and a Token, producing an expression (Expr).
    /// </summary>
    internal interface IPrefixParselet
    {
        Expr Parse(Parser parser, Token token);
    }

    /// <summary>
    /// Handles parsing of names, numbers, and variables into the appropriate Expression types.
    /// These are grouped as the non-operator prefix expressions in the Pratt Parser.
    /// </summary>
    public class NameParselet : IPrefixParselet
    {
        public Expr Parse(Parser parser, Token token)
        {
            switch (token.Type)
            {
                case TokenType.Identifier:
                    return new StringLitExpr(token.Lexeme);
                case TokenType.Number:
                    return new NumberLitExpr(double.Parse(token.Lexeme));
                case TokenType.String:
                    return new VariableExpr(token.Lexeme);
                default:
                    throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}");
            }
        }
    }

    /// <summary>
    /// Handles prefix operators like '+' Plus, '-' Minus, & '!' Not.
    /// Currently these are the only three implemented.
    /// </summary>
    public class PrefixOperatorParselet : IPrefixParselet
    {
        int _Precedence;

        public PrefixOperatorParselet(int precedence = Precedence.PREFIX)
        {
            _Precedence = precedence;
        }
        public Expr Parse(Parser parser, Token token)
        {
            // TODO: Clean Comments here
            // Consume the operator token.
            // parser.Advance();

            // Parse the next part of the expression.
            Expr operand = parser.ParseExpression(_Precedence);
            // Return a new prefix expression with the operator and right side.
            return new PrefixExpr(token.Type, operand);
        }
        public int GetPrecedence()
        {
            return _Precedence;
        }
    }





}