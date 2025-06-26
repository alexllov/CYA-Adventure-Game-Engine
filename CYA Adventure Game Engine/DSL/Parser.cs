using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{

    public class Parser
    {
        // Part Dicts for Pratt.
        Dictionary<TokenType, IPrefixParselet> PrefixParts = new()
        {
            {TokenType.Identifier, new NameParselet()},
            {TokenType.Number, new NameParselet()},
            {TokenType.String, new NameParselet()},
            {TokenType.Plus, new PrefixOperatorParselet()},
            {TokenType.Minus, new PrefixOperatorParselet()},
            {TokenType.Not, new PrefixOperatorParselet()}
        };

        Dictionary<TokenType, IInfixParselet> InfixParts = new()
        {
            {TokenType.Assign, new AssignParselet(Precedence.ASSIGNMENT)},
            {TokenType.Plus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Minus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Multiply, new BinaryOperatorParselet(Precedence.PRODUCT)},
            {TokenType.Divide, new BinaryOperatorParselet(Precedence.PRODUCT)},
            {TokenType.Equal, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.NotEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.And, new BinaryOperatorParselet(Precedence.AND)},
            {TokenType.Or, new BinaryOperatorParselet(Precedence.OR)}
        };

        // Parser Components.
        private readonly List<Token> Tokens;
        private int Pos = 0;

        // TODO: Re-Add this once Stmt implemented.
        //public List<Stmt> AST;

        // TODO: Remove this once Stmt implemented.
        public List<Expr> Expressions = new List<Expr>();

        public void Show()
        {
            Console.WriteLine("Parser Expressions:");
            foreach (var expr in Expressions)
            {
                Console.WriteLine(expr);
            }
        }

        public Parser(List<Token> tokens) 
        {
            Tokens = tokens;
            Parse();
        }

        private void Parse()
        {
            // TODO: Reimplement this once Stmt handling is added.
            // while (!IsAtEnd())
            // {
            //     AST.Add(ParseStatement());
            // }
            while (Peek(0).Type != TokenType.EOF)
            {
                Expressions.Add(ParseExpression(0));
            }
        }

        /// <summary>
        /// Expression Parsing - Implements a Pratt Parser.
        /// </summary>
        /// <returns>Expr</returns>
        /// <exception cref="Exception"></exception>
        public Expr ParseExpression(int precedence)
        {
            Token token = Advance();
            Console.WriteLine($"ParseExpression: consumed token {token.Type} '{token.Lexeme}'");
            Token peek = Peek(0);
            Console.WriteLine($"Peeked next: {peek.Type} '{peek.Lexeme}'");

            IPrefixParselet prefix;
            if (PrefixParts.ContainsKey(token.Type))
            {
                prefix = PrefixParts[token.Type];
            }
            else
            {
                throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}");
            }
            Expr left = prefix.Parse(this, token);

            // Identify Infix.
            // token = Peek(0); <- might be needed, check later TODO
            while (precedence < GetPrecedence())
            {
                token = Advance();
                IInfixParselet infix;
                if (InfixParts.ContainsKey(token.Type))
                {
                    infix = InfixParts[token.Type];
                    left = infix.Parse(this, left, token);
                }
                else
                {
                    throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}");
                }
            }
            return left;
        }

        /// <summary>
        /// Helper func to get precedence of the next token.
        /// </summary>
        /// <returns>int</returns>
        private int GetPrecedence()
        {
            IInfixParselet infix;
            if (InfixParts.ContainsKey(Peek(0).Type))
            {
                infix = InfixParts[Peek(0).Type];
                return infix.GetPrecedence();
            }
            else { return 0; }
        }


        // TODO: Construct giant SwitchCase here i guess.
        private Stmt ParseStatement()
        {
            Token token = Tokens[Pos];

            switch (token.Type)
            {
                case TokenType.Import:
                    return ParseImportStmt();

                // TODO: Keep working on this Identifier case.
                //case TokenType.Identifier:
                //    // Check if it's a variable declaration or an expression.
                //    if (PeekNext().Type == TokenType.Pipe)
                //    {
                //        return ParseImportStmt();
                //    }

                case TokenType.RBracket:
                    return ParseBracketStmt();
                    
                default:
                    throw new Exception($"Unexpected token: {token}");
            }
        }

        private Stmt ParseImportStmt()
        {
            // Consume the 'import' token that IDd the stmt.
            Advance();
            Token module = Consume(TokenType.Identifier);
            if (Match(TokenType.As))
            {
                Token alias = Consume(TokenType.Identifier);
                return new ImportStmt(module.Lexeme, alias.Lexeme);
            }
            return new ImportStmt(module.Lexeme);
        }

        private Stmt ParseBracketStmt()
        {
            Token token = Tokens[Pos];
            // Consume the opening bracket.
            Consume(TokenType.RBracket);
            switch (token.Type)
            {
                case TokenType.If:
                    return ParseIfStmt();
                default:
                    throw new Exception($"Unexpected token: {token}");
            }
        }

        private Stmt ParseIfStmt()
        {
            // Consume the 'if' token.
            Advance();
            Expr condition = ParseExpression(Precedence.CONDITIONAL);
            Consume(TokenType.Then);
            Stmt thenBranch = ParseStatement();
            Stmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = ParseStatement();
            }
            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private bool IsAtEnd()
        {
            Console.WriteLine($"Pos: {Pos}, Len: {Tokens.Count}");
            return Pos >= Tokens.Count;
        }

        // TODO: review alt return branch.
        // TODO: Review protection level when integrating Pratt Parser for exprs.
        public Token Advance()
        {
            if (!IsAtEnd())
            {
                return Tokens[Pos++];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        private Token Consume(TokenType type)
        {
            if (Peek(0).Type == type)
            {
                return Advance();
            }
            throw new Exception($"Expected token type {type}, but found {Peek(0).Type}.");
        }

        private Token Peek(int dist)
        {
            if (Pos + dist < Tokens.Count)
            {
                return Tokens[Pos+dist];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Peek(0).Type == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
    }
}
