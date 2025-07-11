using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
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

        public List<IStmt> AST = new List<IStmt>();

        /// <summary>
        /// Debug method.
        /// </summary>
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
        }

        /// <summary>
        /// Takes the Token list initially passed into the parser and constructs an AST from it.
        /// </summary>
        public void Parse()
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
        public IExpr ParseExpression(int precedence)
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
            IExpr left = prefix.Parse(this, token);

            // Identify Infix.
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

        /// <summary>
        /// Parses the next statement. Returns an IStmt.
        /// </summary>
        /// <returns></returns>
        private IStmt ParseStmt()
        {
            Token token = Tokens[Pos];

            switch (token.Type)
            {
                case TokenType.Import:
                    return ParseImportStmt();

                // Holds If stmts & Interactables.
                case TokenType.LBracket:
                    return ParseBracket();

                case TokenType.Start:
                    return ParseStart();

                case TokenType.GoTo:
                    return ParseGoTo();

                // Scene & Components.
                case TokenType.Scene:
                    return ParseSceneStmt();
                    
                default:
                    IExpr expr = ParseExpression(0);
                    if (expr is AssignExpr aExpr){ return ParseAssignStmt(aExpr); }
                    else { return new ExprStmt(expr); }
            }
        }

        /// <summary>
        /// Unwraps AssignExpr into AssignStmt.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private AssignStmt ParseAssignStmt(AssignExpr expr)
        {
            return new AssignStmt(expr.Name, expr.Value);
        }

        /// <summary>
        /// Constructs Stmt for imports. Allows for optional aliasing.
        /// </summary>
        /// <returns>ImportStmt</returns>
        private ImportStmt ParseImportStmt()
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
        private IStmt ParseBlock(params TokenType[] stoppingPoint)
        {
            List<IStmt> stmts = new();
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
        private IStmt ParseIfStmt()
        {
            // Consume the 'if' token.
            Advance();
            IExpr condition = ParseExpression(0);
            Consume(TokenType.Then);
            IStmt thenBranch = ParseBlock(TokenType.Else, TokenType.RBracket);
            IStmt elseBranch = null;
            if (Match(TokenType.Else))
            {
                elseBranch = ParseBlock(TokenType.RBracket);
            }
            // TODO: Consider moving this into the Bracket Parsing space to avoid duplication.
            // Consume closing bracket.
            Consume(TokenType.RBracket);
            return new IfStmt(condition, thenBranch, elseBranch);
        }

        /// <summary>
        /// Creates Interactable statement.
        /// </summary>
        /// <returns>InteractableStmt</returns>
        private InteractableStmt ParseInteractable()
        { 
            IExpr name = ParseExpression(0);
            IStmt body = ParseBlock(TokenType.RBracket);
            Consume(TokenType.RBracket);
            return new InteractableStmt(name, body);
        }

        /// <summary>
        /// Parses the statement types identified by brackets.
        /// Currently: If statement, Interactable statement.
        /// </summary>
        /// <returns>IStmt</returns>
        /// <exception cref="Exception"></exception>
        private IStmt ParseBracket()
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

        /// <summary>
        /// Creates Start statement.
        /// </summary>
        /// <returns>StartStmt</returns>
        /// <exception cref="Exception"></exception>
        private StartStmt ParseStart()
        {
            // Consume "Start" token.
            Consume(TokenType.Start);
            IExpr ID = ParseExpression(0);
            if (!(ID is StringLitExpr slID)) { throw new Exception($"Unexpected Expr type following START. Expected String Literal, received {ID} of type {ID.GetType()}."); }
            return new StartStmt(slID);
        }

        /// <summary>
        /// Creates GoTo statement.
        /// </summary>
        /// <returns>GoToStmt</returns>
        /// <exception cref="Exception"></exception>
        private GoToStmt ParseGoTo()
        {
            // Consume "GoTo" token.
            Consume(TokenType.GoTo);
            IExpr loc = ParseExpression(0);
            if (!(loc is StringLitExpr sLoc)) { throw new Exception($"Unexpected Expr type following START. Expected String Literal, received {loc} of type {loc.GetType()}."); }
            return new GoToStmt(sLoc);
        }

        /// <summary>
        /// Creates a Scene statement.
        /// </summary>
        /// <returns>SceneStmt</returns>
        private SceneStmt ParseSceneStmt()
        {
            // Consume the 'scene' token.
            Advance();
            // Very next Token should be string with ID for scene.
            Token ID = Consume(TokenType.String);
            List<IStmt> parts = new();
            while (!HeaderEnds.Contains(Peek(0).Type))
            {
                // Scenes have special sugar for strings,
                // & can contain special components: interactables.
                // So we will filter for those.
                switch (Peek(0).Type)
                {
                    case TokenType.String:
                        FuncExpr say = new(new VariableExpr("say"), [new StringLitExpr(Peek(0).Lexeme)]);
                        ExprStmt sayStmt = new(say);
                        parts.Add(sayStmt);
                        // Advance needed as Stmt hand made, so string part isn't being consumed.
                        Advance();
                        break;

                    // Default -> ParseStmt using recursive calls to process.
                    default:
                        IStmt stmt = ParseStmt();
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

        /// <summary>
        /// Checks for EOF based on position & Tokens length.
        /// </summary>
        /// <returns>bool</returns>
        private bool IsAtEnd()
        {
            return Pos >= Tokens.Count;
        }

        /// <summary>
        /// Moves position forward, returning the next token.
        /// </summary>
        /// <returns>Token</returns>
        public Token Advance()
        {
            if (!IsAtEnd())
            {
                return Tokens[Pos++];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        /// <summary>
        /// Takes an expected token type, compares it to the current token.
        /// If the type is correct, it is consumed. Else throws error.
        /// </summary>
        /// <param name="type">TokenType</param>
        /// <returns>Token</returns>
        /// <exception cref="Exception"></exception>
        public Token Consume(TokenType type)
        {
            if (Peek(0).Type == type)
            {
                return Advance();
            }
            throw new Exception($"Expected token type {type}, but found {Peek(0).Type}.");
        }

        /// <summary>
        /// Scans the token at a given position, relative to the current position of Pos.
        /// returns the scanned token.
        /// </summary>
        /// <param name="dist">int: the relative position of the token to scan.</param>
        /// <returns>Token</returns>
        public Token Peek(int dist)
        {
            if (Pos + dist < Tokens.Count)
            {
                return Tokens[Pos+dist];
            }
            return new Token(TokenType.EOF, "", -1, -1);
        }

        /// <summary>
        /// Checks the Type of the given token to match on a list of types.
        /// Allows for conditional branching if specific optional tokens are found - e.g. 'else' branches in If statements.
        /// </summary>
        /// <param name="types">list of TokenTypes</param>
        /// <returns>bool</returns>
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
