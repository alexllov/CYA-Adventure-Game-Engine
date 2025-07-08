using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
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
            {TokenType.LParent, new ParentParselet()},
            {TokenType.GoTo, new GoToParselet()}
        };

        Dictionary<TokenType, IInfixParselet> InfixParts = new()
        {
            // Assignment.
            {TokenType.Assign, new AssignParselet(Precedence.ASSIGNMENT)},
            // Arithmetic Operators.
            {TokenType.Plus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Minus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Multiply, new BinaryOperatorParselet(Precedence.PRODUCT)},
            {TokenType.Divide, new BinaryOperatorParselet(Precedence.PRODUCT)},
            // Comparative.
            {TokenType.Equal, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.NotEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            // Boolean.
            {TokenType.And, new BinaryOperatorParselet(Precedence.AND)},
            {TokenType.Or, new BinaryOperatorParselet(Precedence.OR)},
            // Dot.
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

        public List<Stmt> AST = new List<Stmt>();

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
                AST.Add(ParseStmt());
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
        private Stmt ParseStmt()
        {
            Token token = Tokens[Pos];

            switch (token.Type)
            {
                case TokenType.Import:
                    return ParseImportStmt();

                // Holds If stmts & Interactables.
                case TokenType.LBracket:
                    return ParseBracket();

                // Scene & Components.
                case TokenType.Scene:
                    return ParseSceneStmt();
                    
                default:
                    Expr expr = ParseExpression(0);
                    if (expr is AssignExpr){ return new AssignStmt((AssignExpr)expr); }
                    else { return new ExprStmt(expr); }
            }
        }

        /// <summary>
        /// Constructs Stmt for imports. Allows for optional aliasing.
        /// </summary>
        /// <returns>ImportStmt</returns>
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
                stmts.Add(ParseStmt());
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

        /// <summary>
        /// Creates an If statement, allowing for conditional branching down a 'then' and optional 'else' branch.
        /// </summary>
        /// <returns>IfStmt</returns>
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

        private Stmt ParseInteractable()
        { 
            Expr name = ParseExpression(0);
            Stmt body = ParseBlock(TokenType.RBracket);
            Consume(TokenType.RBracket);
            return new InteractableStmt(name, body);
        }

        private Stmt ParseBracket()
        {
            Token token = Consume(TokenType.LBracket);

            switch (Peek(0).Type)
            {
                case TokenType.If:
                    return ParseIfStmt();

                //TODO: THIS NEEDS EXPADING TO COPE WITH $Strings && OTHER IN FUTURE.
                case TokenType.String:
                    return ParseInteractable();

                default:
                    throw new Exception($"Unexpected token type following '[': {token.Type}, on line{token.position[0]}");
            }
        }

        private Stmt ParseSceneStmt()
        {
            // Consume the 'scene' token.
            Advance();
            // Very next Token should be string with ID for scene.
            Token ID = Consume(TokenType.String);
            List<Stmt> parts = new();
            while (!HeaderEnds.Contains(Peek(0).Type))
            {
                // Scenes have special sugar for strings,
                // & can contain special components: interactables.
                // So we will filter for those.
                switch (Peek(0).Type)
                {
                    /*
                     * TODO: test this, may need reworking.
                     * May need to change s.t. say func can be reassigned?
                     * OR include some sort of safety blocks to make keywords unable to be reassigned??
                     * ^^ THIS is probably the correct solution.
                     */
                    case TokenType.String:
                        FuncExpr say = new(new FuncExpr(new VariableExpr("say"), [new StringLitExpr(Peek(0).Lexeme)]));
                        ExprStmt sayStmt = new(say);
                        parts.Add(sayStmt);
                        // Advance needed as Stmt hand made, so string part isn't being consumed.
                        Advance();
                        break;

                    // Default -> ParseStmt using recursive calls to process.
                    default:
                        Stmt stmt = ParseStmt();
                        parts.Add(stmt);
                        break;
                }
            }
            // Consume the End Token if found.
            if (Peek(0).Type is TokenType.End)
            {
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

        public Token Consume(TokenType type)
        {
            if (Peek(0).Type == type)
            {
                return Advance();
            }
            throw new Exception($"Expected token type {type}, but found {Peek(0).Type}.");
        }

        public Token Peek(int dist)
        {
            if (Pos + dist < Tokens.Count)
            {
                return Tokens[Pos+dist];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        public bool Match(params TokenType[] types)
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
