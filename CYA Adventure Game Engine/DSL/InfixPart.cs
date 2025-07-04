﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal interface IInfixParselet
    {
        Expr Parse(Parser parser, Expr left, Token token);
        int GetPrecedence();
    }

    public class AssignParselet : IInfixParselet
    {
        int _Precedence;

        public AssignParselet(int precedence)
        {
            _Precedence = precedence;
        }

        public Expr Parse(Parser parser, Expr left, Token token)
        {
            Expr right = parser.ParseExpression(Precedence.ASSIGNMENT - 1);

            if (!(left is VariableExpr or DotExpr))
            {
                throw new Exception($"Left hand side must be a var. Got {left.GetType()} instead.");
            }
            else
            {
                return new AssignExpr(left, right);
            }
        }

        public int GetPrecedence()
        {
            return _Precedence;
        }
    
    }

    public class BinaryOperatorParselet : IInfixParselet
    {
        int _Precedence;

        public BinaryOperatorParselet(int precedence)
        {
            _Precedence = precedence;
        }
        public Expr Parse(Parser parser, Expr left, Token token)
        {
            // Parse the right side of the expression.
            Expr right = parser.ParseExpression(_Precedence);
            // Return a new binary expression with the left side, operator, and right side.
            return new BinaryExpr(left, token.Type, right);
        }

        public int GetPrecedence()
        {
            return _Precedence;
        }
    }

    public class DotParselet : IInfixParselet
    {
        int _Precedence;
        public DotParselet(int precedence)
        {
            _Precedence = precedence;
        }
        public Expr Parse(Parser parser, Expr left, Token token)
        {
            // Parse the right side of the expression.
            Expr right = parser.ParseExpression(_Precedence);
            return new DotExpr(left, right);
        }
        public int GetPrecedence()
        {
            return _Precedence;
        }
    }
}

