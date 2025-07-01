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
            {TokenType.Not, new PrefixOperatorParselet()},
            {TokenType.Ask, new AskParselet()}
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
        public List<Stmt> AST = new List<Stmt>();

        // TODO: Remove this once Stmt implemented.
        public List<Expr> Expressions = new List<Expr>();

        // TODO: UPDATE TO STMTS RATHER THAN EXPRS.
        public void Show()
        {
            Console.WriteLine("AST Statements:");
            foreach (var stmt in AST)
            {
                Console.WriteLine(stmt);
            }
        }

        public Parser(List<Token> tokens) 
        {
            Tokens = tokens;
            Parse();
        }

        private void Parse()
        {
            while (Peek(0).Type != TokenType.EOF)
            {
                AST.Add(ParseStatement());
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

                case TokenType.LBracket:
                    return ParseBracket();

                case TokenType.Say:
                    return ParseSayStmt();

                //case TokenType.RBracket:
                //    return ParseBracketStmt();
                    
                // NOT adding if stmt here to separate BinaryExpr from others as a binary should not exist in isolation.
                default:
                    Expr expr = (ParseExpression(0));
                    ExprStmt stmt = new(expr);
                    return stmt;
            }
        }

        private Stmt ParseBracket()
        {
            Advance();
            Token token = Tokens[Pos];

            switch (token.Type)
            {
                case TokenType.If:
                    return ParseIfStmt();

                default:
                    throw new Exception($"Unexpected token type following '[': {token.Type}");
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

        private Stmt ParseSayStmt()
        {
            Advance();
            Expr expr = ParseExpression(0);
            return new SayStmt(expr);
        }

        /// <summary>
        /// Func to compile possible blocks of statements either into a BlockStmt or single Stmt depending on how many.
        /// Example use: IF stmt processing to collect all statements in the 'then' & 'else' branches
        /// Collects statements until it finds the dedicated 'stopping' token.
        /// </summary>
        /// <param name="stoppingPoint">Token types list representing end point for Stmt collection.</param>
        /// <returns>Stmt</returns>
        private Stmt ParseBlock(params TokenType[] stoppingPoint)
        {
            List<Stmt> stmts = new();
            while (!stoppingPoint.Contains(Peek(0).Type))
            {
                stmts.Add(ParseStatement());
            }
            if (stmts.Count > 1)
            {
                return new BlockStmt(stmts);
            }
            else
            {
                return stmts[0];
            }
        }

        private Stmt ParseIfStmt()
        {
            // Consume the 'if' token.
            Advance();
            Expr condition = ParseExpression(0);
            Consume(TokenType.Then);
            Stmt thenBranch = ParseBlock(TokenType.Else, TokenType.RBracket);
            Stmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = ParseBlock(TokenType.RBracket);
            }
            // TODO: Consider moving this into the Bracket Parsing space to avoid duplication.
            // Consume closing bracket.
            Consume(TokenType.RBracket);
            return new IfStmt(condition, thenBranch, elseBranch);
        }

        // TODO: This Needs Refining.
        // private Stmt ParseBracketStmt()
        // {
        //     Token token = Tokens[Pos];
        //     // Consume the opening bracket.
        //     Consume(TokenType.RBracket);
        //     switch (token.Type)
        //     {
        //         case TokenType.If:
        //             return ParseIfStmt();
        //         default:
        //             throw new Exception($"Unexpected token: {token}");
        //     }
        // }

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
