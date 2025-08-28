using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace VOXI.Frontend
{
    public class NounExpr : IExpr
    {
        public string Noun;
        public Dictionary<string, IVerb> Verbs;

        public NounExpr(string noun, Dictionary<string, IVerb> verbs)
        {
            Noun = noun;
            Verbs = verbs;
        }

        public override string ToString()
        {
            List<string> actions = [];
            foreach (var kvp in Verbs)
            {
                actions.Add($"Verb({kvp.Key}): {kvp.Value.ToString()}");
            }
            string actString = string.Join("\n  ", actions);
            return $"NounExpr({Noun}:\n  {actString})";
        }

        public object Interpret(Environment state)
        {
            return new NounObject(Noun, Verbs);
        }

        public static NounExpr Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "noun statement";
            // Consume the '{' & 'noun' token.
            parser.Tokens.Advance();
            parser.Tokens.Advance();
            // Get string noun name.
            string noun = parser.Tokens.Consume(TokenType.String).Lexeme;

            // Get verbs until some other token.
            Dictionary<string, IVerb> verbs = [];
            while (parser.Tokens.IdentLexemeMatch("verb"))
            {
                List<IVerb> newVerbs = ParseVerbs(parser);
                foreach (IVerb newSingleVerb in newVerbs)
                {
                    verbs[newSingleVerb.Verb] = newSingleVerb;
                }
                // Eat the commas at teh end of verbs segments with them.
                if (parser.Tokens.Peek(0).Type is TokenType.Comma) { parser.Tokens.Consume(TokenType.Comma); }
            }
            parser.Tokens.Consume(TokenType.RCurly);
            return new NounExpr(noun, verbs);
        }

        private static List<IVerb> ParseVerbs(Parser parser)
        {
            parser.CurrentStmtParsing = "verb statement";
            List<IVerb> verbs = [];
            List<string> aliases = [];
            while (parser.Tokens.Peek(0).Type is TokenType.String)
            {
                // Get string verb name.
                aliases.Add(parser.Tokens.Consume(TokenType.String).Lexeme);
            }
            // Parse the commands for this verb.
            switch (parser.Tokens.Peek(0))
            {
                case { Type: TokenType.RCurly }:
                    throw new Exception($"Error, verb {aliases[0]} has no associated commands. " +
                        $"Error location: {parser.Tokens.Peek(0)}. " +
                        $"Occured during {parser.CurrentStmtParsing}, within {parser.StartOfCurrentStmt}.");
                case { Type: TokenType.Identifier, Lexeme: "prep" }:
                    List<DitransitiveVerbStmt> ditransVerbs = DitransitiveVerbStmt.ParseDitransitiveVerbs(parser, aliases);
                    foreach (DitransitiveVerbStmt ditransVerb in ditransVerbs)
                    {
                        verbs.Add(ditransVerb);
                    }
                    break;

                default:
                    IStmt action = BlockStmt.Parse(parser,
                        (Token token) => token switch
                        {
                            { Type: TokenType.RCurly } => true,
                            { Type: TokenType.Identifier, Lexeme: "verb" } => true,
                            _ => false,
                        }
                        );
                    foreach (string alias in aliases)
                    {
                        verbs.Add(new TransitiveVerbStmt(alias, action));
                    }
                    break;
            }
            return verbs;
        }


    }
}
