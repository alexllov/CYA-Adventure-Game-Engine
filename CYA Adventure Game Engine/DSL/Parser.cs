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
        Dictionary<TokenType, PrefixParcelet> PrefixParts = new()
        {
            {TokenType.Identifier, new NameParcelet()},
            {TokenType.Number, new NameParcelet()},
            {TokenType.String, new NameParcelet()},
            {TokenType.Plus, new PrefixOperatorParcelet()},
            {TokenType.Minus, new PrefixOperatorParcelet()},
            {TokenType.Not, new PrefixOperatorParcelet()}
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
            PrefixParcelet prefix = PrefixParts[token.Type];
            if (prefix == null)
            {
                throw new Exception($"Unexpected token type: {token.Type}.");
            }
            return prefix.Parse(this, token);
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
            if (Peek().Type == type)
            {
                return Advance();
            }
            throw new Exception($"Expected token type {type}, but found {Peek().Type}.");
        }

        private Token Peek()
        {
            if (!IsAtEnd())
            {
                return Tokens[Pos];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        private Token PeekNext()
        {
            if (Pos + 1 < Tokens.Count)
            {
                return Tokens[Pos + 1];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Peek().Type == type)
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }
    }
}
