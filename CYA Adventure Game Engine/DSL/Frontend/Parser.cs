using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{

    public class Parser
    {
        // Part Dicts for Pratt.
        readonly Dictionary<TokenType, IPrefixParselet> PrefixParts = new()
        {
            {TokenType.Identifier, new NameParselet()},
            {TokenType.Number, new NameParselet()},
            {TokenType.String, new NameParselet()},
            {TokenType.Boolean, new NameParselet()},
            {TokenType.Plus, new PrefixOperatorParselet()},
            {TokenType.Minus, new PrefixOperatorParselet()},
            {TokenType.Not, new PrefixOperatorParselet()},
            {TokenType.LParent, new ParentParselet()},
        };

        readonly Dictionary<TokenType, IInfixParselet> InfixParts = new()
        {
            // Arithmetic Operators.
            {TokenType.Plus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Minus, new BinaryOperatorParselet(Precedence.SUM)},
            {TokenType.Multiply, new BinaryOperatorParselet(Precedence.PRODUCT)},
            {TokenType.Divide, new BinaryOperatorParselet(Precedence.PRODUCT)},
            // Comparative.
            {TokenType.Equal, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.NotEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.LessEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterThan, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
            {TokenType.GreaterEqual, new BinaryOperatorParselet(Precedence.CONDITIONAL)},
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
        readonly List<TokenType> HeaderEnds = [TokenType.Scene, TokenType.Table, TokenType.Overlay, TokenType.End, TokenType.EOF];

        // Parser Components.
        private readonly List<Token> Tokens;
        private int Pos = 0;

        public List<IStmt> AST = [];

        public Parser(List<Token> tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Takes the Token list initially passed into the parser and constructs an AST from it.
        /// </summary>
        public AbstSyntTree Parse()
        {
            while (Peek(0).Type != TokenType.EOF)
            {
                AST.Add(ParseStmt());
                //Console.WriteLine(AST.Last());
            }
            return new AbstSyntTree(AST);
        }

        /// <summary>
        /// Parses the next statement. Returns an IStmt.
        /// </summary>
        /// <returns></returns>
        private IStmt ParseStmt()
        {
            Token token = Tokens[Pos];

            return (token.Type, Peek(1).Type) switch
            {
                (TokenType.Import, _) => ParseImportStmt(),
                // []'s Hold If stmts & Choices.
                (TokenType.LBracket, TokenType.If) => ParseIfStmt(),
                (TokenType.LBracket, TokenType.String) => ParseChoice(),
                (TokenType.LBracket, _) => throw new Exception($"Unexpected token type following '[': {token.Type}, on line{token.position[0]}"),

                // TODO: Remove the lower & move fully to noun parsing.
                (TokenType.LCurly, TokenType.Noun) => ParseNoun(),
                (TokenType.LCurly, _) => ParseAddNounStmt(),
                // GoTo.
                (TokenType.GoTo, _) => ParseGoTo(),
                // Scene & Components.
                (TokenType.Scene, _) => ParseSceneStmt(),
                // Table.
                (TokenType.Table, _) => ParseTable(),
                // Overlay: run & exit allow for overlay control.
                (TokenType.Overlay, _) => ParseOverlay(),
                (TokenType.Run, _) => ParseRun(),
                (TokenType.Exit, _) => ParseExit(),
                // Assignment.
                (TokenType.Identifier, TokenType.Assign) => ParseAssign(),
                // Default: loose expression, to be wrapped as Expression Statement.
                _ => HandleExpression()
            };
        }

        /// <summary>
        /// Helper func, allows for AssignExprs to be wrapped separately from other Exprs.
        /// </summary>
        /// <returns>IStmt</returns>
        public IStmt HandleExpression()
        {
            IExpr expr = ParseExpression(0);
            //if (expr is AssignExpr aExpr) { return ParseAssignStmt(aExpr); }
            return new ExprStmt(expr);
        }

        /// <summary>
        /// Expression Parsing - Implements a Pratt Parser.
        /// </summary>
        /// <returns>Expr</returns>
        /// <exception cref="Exception"></exception>
        public IExpr ParseExpression(int precedence)
        {
            Token token = Advance();

            if (!PrefixParts.TryGetValue(token.Type, out IPrefixParselet? prefix))
            {
                throw new Exception($"Unexpected token type: {token.Type} on line {token.position[0]}.");
            }
            IExpr left = prefix.Parse(this, token);

            /*
             * Identify Infix (if there is one)
             * Take the precedence of the next token & compare to this expr's precedence.
             * If next token isn't an infix, will return 0, which should signal the end of his expression.
             */
            while (precedence < GetPrecedence())
            {
                token = Advance();
                if (!InfixParts.TryGetValue(token.Type, out IInfixParselet? infix))
                {
                    throw new Exception($"Unexpected token type: {token.Type} on line {token.position[0]}.");
                }
                left = infix.Parse(this, left, token);
            }
            return left;
        }

        /// <summary>
        /// Helper func to get precedence of the next token.
        /// </summary>
        /// <returns>int</returns>
        private int GetPrecedence()
        {
            if (!InfixParts.TryGetValue(Peek(0).Type, out IInfixParselet? infix))
            {
                return 0;
            }
            return infix.GetPrecedence();
        }

        /// <summary>
        /// Unwraps AssignExpr into AssignStmt.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private AssignStmt ParseAssign()
        {
            string name = Peek(0).Lexeme;
            Consume(TokenType.Identifier);
            Consume(TokenType.Assign);
            IExpr value = ParseExpression(0);
            return new AssignStmt(name, value);
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
            List<IStmt> stmts = [];
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
        private IfStmt ParseIfStmt()
        {
            // Consume the '[' & then 'if' token.
            Advance();
            Advance();
            IExpr condition = ParseExpression(0);
            Consume(TokenType.Then);
            IStmt thenBranch = ParseBlock(TokenType.Else, TokenType.RBracket);
            IStmt? elseBranch = null;
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
        /// Creates Choice statement.
        /// </summary>
        /// <returns>InteractableStmt</returns>
        private ChoiceStmt ParseChoice()
        {
            // Consume the '['
            Advance();
            IExpr name = ParseExpression(0);
            IStmt body = ParseBlock(TokenType.RBracket);
            Consume(TokenType.RBracket);
            return new ChoiceStmt(name, body);
        }

        private AddNounStmt ParseAddNounStmt()
        {
            Consume(TokenType.LCurly);
            List<IExpr> nouns = [];
            while (Peek(0).Type is not TokenType.RCurly)
            {
                nouns.Add(ParseExpression(0));
            }
            Consume(TokenType.RCurly);
            return new AddNounStmt(nouns);
        }

        private NounStmt ParseNoun()
        {
            // Consume the '{' & 'noun' token.
            Advance();
            Advance();
            // Get string noun name.
            string noun = Consume(TokenType.String).Lexeme;

            // Get verbs until some other token.
            Dictionary<string, IVerb> verbs = [];
            while (Peek(0).Type is TokenType.Verb)
            {
                List<IVerb> newVerbs = ParseVerbs();
                foreach (IVerb newSingleVerb in newVerbs)
                {
                    verbs[newSingleVerb.Verb] = newSingleVerb;
                }
                // Eat the commas at teh end of verbs segments with them.
                if (Peek(0).Type is TokenType.Comma) { Consume(TokenType.Comma); }
            }
            Consume(TokenType.RCurly);
            return new NounStmt(noun, verbs);
        }

        private List<IVerb> ParseVerbs()
        {
            // Consume the 'verb' token.
            Advance();

            List<IVerb> verbs = [];
            List<string> aliases = [];
            while (Peek(0).Type is TokenType.String)
            {
                // Get string verb name.
                aliases.Add(Consume(TokenType.String).Lexeme);
            }
            // Parse the commands for this verb.
            switch (Peek(0).Type)
            {
                case TokenType.RCurly:
                    throw new Exception($"Error, verb {aliases[0]} has no associated commands.");
                case TokenType.Prep:
                    List<DitransitiveVerbStmt> ditransVerbs = ParseDitransitiveVerbs(aliases);
                    foreach (DitransitiveVerbStmt ditransVerb in ditransVerbs)
                    {
                        verbs.Add(ditransVerb);
                    }
                    break;

                default:
                    IStmt action = ParseBlock(TokenType.Verb, TokenType.RCurly);
                    foreach (string alias in aliases)
                    {
                        verbs.Add(new TransitiveVerbStmt(alias, action));
                    }
                    break;
            }
            return verbs;
        }

        /// <summary>
        /// Parses DitransitiveVerbStmt & constructs a list of them for each alias provided.
        /// These consist of a verb alias, and a Preposition statement.
        /// Example "'give' note 'to sally'"
        /// </summary>
        /// <param name="verbAliases"></param>
        /// <returns>List<DitransitiveVerbStmt></returns>
        private List<DitransitiveVerbStmt> ParseDitransitiveVerbs(List<string> verbAliases)
        {
            // Get all the prepositions & attached indr objects.
            Dictionary<string, PrepositionStmt> prepositions = [];
            while (Peek(0).Type is TokenType.Prep)
            {
                List<PrepositionStmt> preps = ParsePrepositions();
                foreach (PrepositionStmt prep in preps)
                {
                    prepositions[prep.Name] = prep;
                }
            }

            // Construct a ditrans verb for each alias
            List<DitransitiveVerbStmt> ditransVerbs = [];
            foreach (string verb in verbAliases)
            {
                ditransVerbs.Add(new DitransitiveVerbStmt(verb, prepositions));
            }
            return ditransVerbs;
        }

        /// <summary>
        /// Parses all Prepositions inside a DitransitiveVerbStmt,
        /// These consist of a "preposition" & indirect object, e.g. the recipient of an action
        /// Example: "give note 'to sally'"
        /// </summary>
        /// <returns>List<PrepositionStmt></returns>
        private List<PrepositionStmt> ParsePrepositions()
        {
            TokenType[] prepositionEnds =
            [
                TokenType.Prep,
                TokenType.Default,
                TokenType.Verb,
                TokenType.Noun,
                TokenType.RCurly
            ];
            // Consume the 'prep' token.
            Consume(TokenType.Prep);
            List<string> aliases = [];

            // id all aliases for this preposition.
            while (Peek(0).Type is TokenType.String)
            {
                aliases.Add(Consume(TokenType.String).Lexeme);
            }

            // Parse the indirect objects for this prep.
            Dictionary<string, IStmt> indirectObjects = [];
            while (Peek(0).Type is TokenType.Noun)
            {
                Consume(TokenType.Noun);
                string name = Consume(TokenType.String).Lexeme;
                indirectObjects[name] = ParseBlock(prepositionEnds);
            }

            // Prep block must end in Default.
            // Consume the "default" token & get the appropriate action block.
            Consume(TokenType.Default);
            indirectObjects["default"] = ParseBlock(prepositionEnds);

            // Create an identical PrepStmt for each alias.
            List<PrepositionStmt> preps = [];
            foreach (string alias in aliases)
            {
                preps.Add(new PrepositionStmt(alias, indirectObjects));
            }
            return preps;
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
            // Allowing for VariableExprs allows for -> aliasing for reusable overlays & scenes that -> different locations.
            if (loc is not StringLitExpr && loc is not VariableExpr)
            {
                throw new Exception($"Unexpected Expr type following GoTo ('START' or '->'). Expected String Literal or variable, received {loc} of type {loc.GetType()}.");
            }
            return new GoToStmt(loc);
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
            List<IStmt> parts = [];
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
        /// Creates a Table statement.
        /// Used to store entries with common attributes, e.g. a table of interchangable weapons.
        /// </summary>
        /// <returns>TableStmt</returns>
        /// <exception cref="Exception"></exception>
        private TableStmt ParseTable()
        {
            // Consule table token.
            Advance();
            // Next token should be variable table name.
            IExpr ID = ParseExpression(0);
            if (ID is not VariableExpr) { throw new Exception("table declaration should begin with a variable name."); }

            List<List<IExpr>> records = [];
            List<IExpr> row = [];
            while (!HeaderEnds.Contains(Peek(0).Type))
            {
                // || = end of current row & start of next => store completed row.
                if (Peek(0).Type is TokenType.Pipe && Peek(1).Type is TokenType.Pipe)
                {
                    records.Add(row);
                    Advance();
                    row = [];
                }
                // Step over '|'s between columns.
                else if (Peek(0).Type is TokenType.Pipe)
                {
                    Advance();
                }
                else
                {
                    IExpr attribute = ParseExpression(0);
                    row.Add(attribute);
                }

                /*
                 * Catch last record before EOF to add.
                 * This is separate s.t. it doesn't consume that following token which could be the start of a scene.
                 */
                if (HeaderEnds.Contains(Peek(1).Type))
                {
                    records.Add(row);
                    Consume(TokenType.Pipe);
                    break;
                }
            }
            // If table ends with an End token, consume it.
            if (Peek(0).Type is TokenType.End) { Advance(); }
            return new TableStmt((VariableExpr)ID, records);
        }

        /// <summary>
        /// Creates an OverlayStmt.
        /// Overlay's act like scenes but are entered on-top, without leaving the current scene.
        /// Example: a game menu that you want to be accessible from any scene.
        /// </summary>
        /// <returns></returns>
        private OverlayStmt ParseOverlay()
        {
            //Consume overlay
            Advance();
            // Very next Token should be string with ID for scene.
            Token ID = Consume(TokenType.String);
            bool Accessible = false;
            string AccessString = "";
            if (Peek(0).Type is TokenType.Access)
            {
                Accessible = true;
                Advance();
                if (Peek(0).Type is TokenType.String) { AccessString = Peek(0).Lexeme; }
                Advance();
            }
            List<IStmt> parts = [];
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
            if (Accessible) { return new OverlayStmt(ID.Lexeme, body, AccessString); }
            else { return new OverlayStmt(ID.Lexeme, body); }
        }

        private RunStmt ParseRun()
        {
            // Consume the 'run' token.
            Advance();
            // Next token should be string with ID for code to run.
            Token ID = Consume(TokenType.String);
            return new RunStmt(ID.Lexeme);
        }

        private ExitStmt ParseExit()
        {
            // Consume the 'exit' token.
            Advance();
            return new ExitStmt();
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
                return Tokens[Pos + dist];
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
