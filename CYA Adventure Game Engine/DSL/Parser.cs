using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{

    public class Parser
    {
        // Part Dicts for Pratt.
        Dictionary<TokenType, PrefixParselet> PrefixParts = new()
        {
            {TokenType.Identifier, new NameParselet()},
            {TokenType.Number, new NameParselet()},
            {TokenType.String, new NameParselet()},
            {TokenType.Plus, new PrefixOperatorParselet()},
            {TokenType.Minus, new PrefixOperatorParselet()},
            {TokenType.Not, new PrefixOperatorParselet()}
        };

        Dictionary<TokenType, InfixParselet> InfixParts = new()
        {
            {TokenType.Plus, new BinaryOperatorParselet()},
            {TokenType.Minus, new BinaryOperatorParselet()},
            {TokenType.Multiply, new BinaryOperatorParselet()},
            {TokenType.Divide, new BinaryOperatorParselet()},
            {TokenType.Equal, new BinaryOperatorParselet()},
            {TokenType.NotEqual, new BinaryOperatorParselet()},
            {TokenType.LessThan, new BinaryOperatorParselet()},
            {TokenType.GreaterThan, new BinaryOperatorParselet()},
            {TokenType.And, new BinaryOperatorParselet()},
            {TokenType.Or, new BinaryOperatorParselet()}
        };

        // Parser Components.
        private readonly List<Token> Tokens;
        private int Pos = 0;
        public List<Stmt> AST;
        public Parser(List<Token> tokens) 
        {
            Tokens = tokens;
            Parse();
        }

        private void Parse()
        {
            while (!IsAtEnd())
            {
                AST.Add(ParseStatement());
            }
        }

        /// <summary>
        /// Expression Parsing - Implements a Pratt Parser.
        /// </summary>
        /// <returns>Expr</returns>
        /// <exception cref="Exception"></exception>
        public Expr ParseExpression()
        {
            Token token = Advance();

            PrefixParselet prefix;
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
            InfixParselet infix;
            if (InfixParts.ContainsKey(token.Type))
            {
                prefix = PrefixParts[token.Type];
            }
            else
            {
                throw new Exception($"Unexpected token type: {token.Type} at {token.position[0]}:{token.position[1]}");
            }

            Advance();
            return infix.Parse(this, left, token);
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
            Expr condition = ParseExpression();
            Consume(TokenType.Then);
            Stmt thenBranch = ParseStatement();
            Stmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = ParseStatement();
            }
            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private Expr ParseExpression()
        {
            Token token = Advance();
            PrefixPart prefix = 
        }

        private bool IsAtEnd()
        {
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
            if (Pos + dist >= Tokens.Count)
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
