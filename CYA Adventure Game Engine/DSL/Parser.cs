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
            {TokenType.Ask, new AskParselet()},
            //{TokenType.LParent, new CallParselet()}
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
            {TokenType.Or, new BinaryOperatorParselet(Precedence.OR)},
            {TokenType.Dot, new DotParselet(Precedence.DOT)},
        };

        /// <summary>
        /// Header ends: the list of Token Types that will end the collection of stmts into any Header.
        /// These Include: 'Scene', 'Table', 'Code', 'End', 'EOF'.
        /// </summary>
        List<TokenType> HeaderEnds = [TokenType.Scene, TokenType.Table, TokenType.Code, TokenType.End, TokenType.EOF];

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

                // TODO: THIS NEEDS EDIT AS FUNCTIONALITY CHANGED
                case TokenType.LBracket:
                    return ParseBracket();

                case TokenType.Say:
                    return ParseSayStmt();

                // Scene & Components.
                case TokenType.Scene:
                    return ParseSceneStmt();
                    
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
            if (Peek(0).Type == TokenType.Say)
            {
                Advance();
            }
            /*
             * TODO: Consider adding a safety check here for the Scene sugar.
             * Will need to detect Strings or $Strings.
             * elif (Peek(0).Type == TokenType.String)
             * else THROW ERROR...
            */
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

        private Stmt ParseSceneStmt()
        {
            // Consume the 'scene' token.
            Advance();
            // Very next Token should be string with ID for scene.
            Token ID = Consume(TokenType.String);
            List<Stmt> parts = new();
            while (!(HeaderEnds.Contains(Peek(0).Type)))
            {
                Console.WriteLine($"Peeked: {Peek(0).Type}");
                // Scenes have special sugar for strings,
                // & can contain special components: interactables.
                // So we will filter for those.
                switch (Peek(0).Type)
                {
                    // Strings treated as automatic say stmts.
                    case TokenType.String:
                        Stmt say = ParseSayStmt();
                        parts.Add(say);
                        break;

                    // Ident should be an assignment expression.
                    // We process & convert to AssignStmt.
                    // Err thrown if unexpected Expr type found, as other should not be top level in this context.
                    case TokenType.Identifier:
                        Expr assignment = ParseExpression(0);
                        if (assignment.GetType() == typeof(AssignExpr))
                        {
                            AssignStmt assign = new AssignStmt((AssignExpr)assignment);
                            parts.Add(assign);
                        }
                        else
                        {
                          throw new Exception($"Unexpected expression type: {assignment.GetType()} on line {Peek(0).position[0]}.");
                        }
                        break;
                    case TokenType.LBracket:
                        break;
                    case TokenType.LParent:
                        break;
                }
            }
            // Consume the End Token if found.
            if (Peek(0).Type is TokenType.End)
            {
                Console.WriteLine("Found End Token.");
                Consume(TokenType.End);
            }

            // Convert List parts to BlockStmt.
            BlockStmt body = new(parts);
            return new SceneStmt(ID.Lexeme, body);
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
