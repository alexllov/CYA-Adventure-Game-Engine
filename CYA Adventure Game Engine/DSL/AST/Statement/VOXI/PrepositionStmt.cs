using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI
{
    public class PrepositionStmt : IStmt
    {
        public string Name;
        public Dictionary<string, IStmt> IndirectObjects;

        public PrepositionStmt(string prep, Dictionary<string, IStmt> indirectObjects)
        {
            Name = prep;
            IndirectObjects = indirectObjects;
        }

        public override string ToString()
        {
            List<string> indirects = [];
            foreach (var kvp in IndirectObjects)
            {
                indirects.Add($"IndirectObject({kvp.Key}): {kvp.Value.ToString()}");
            }
            string indString = string.Join(',', indirects);
            return $"Preposition({Name}: {indString})";
        }

        public void Interpret(Environment state)
        {
            // Get the command from the state.
            string command = state.GetCommand();
            if (command == "")
            {
                throw new Exception("Preposition: Command not found! Engine error.");
            }
            else
            {
                // IN SCENE & DEFINED -> stmt
                // IN SCENE & NotDefined -> default
                // NOT IN SCENE -> NOT IN SCENE.
                Dictionary<string, NounStmt> local = state.GetLocalNouns();
                // Find the requested IndNoun in local nouns.
                if (local.TryGetValue(command, out NounStmt stmt))
                {
                    // If the Ind obj is defined.
                    if (IndirectObjects.TryGetValue(command, out IStmt? indirectObject))
                    {
                        // Set the new command without the indirect object
                        state.SetCommand("");
                        state.AddSuccessfulCommand(indirectObject);
                    }
                    // If not defined but present do default.
                    else
                    {
                        state.AddSuccessfulCommand(IndirectObjects["default"]);
                    }
                }
                else
                {
                    state.AddCommandError($"There is no {command} here.");
                }
            }
        }

        /// <summary>
        /// Parses all Prepositions inside a DitransitiveVerbStmt,
        /// These consist of a "preposition" & indirect object, e.g. the recipient of an action
        /// Example: "give note 'to sally'"
        /// </summary>
        /// <returns>List<PrepositionStmt></returns>
        public static List<PrepositionStmt> ParsePrepositions(Parser parser)
        {
            TokenType[] prepositionEnds =
            [
                TokenType.Prep,
            TokenType.Default,
            TokenType.Verb,
            TokenType.Noun,
            TokenType.RCurly
            ];
            List<string> aliases = [];

            // id all aliases for this preposition.
            while (parser.Tokens.Peek(0).Type is TokenType.String)
            {
                aliases.Add(parser.Tokens.Consume(TokenType.String).Lexeme);
            }

            // Parse the indirect objects for this prep.
            Dictionary<string, IStmt> indirectObjects = [];
            while (parser.Tokens.Match(TokenType.Noun))
            {
                string name = parser.Tokens.Consume(TokenType.String).Lexeme;
                indirectObjects[name] = BlockStmt.Parse(parser, prepositionEnds);
            }

            // Prep block must end in Default.
            // Consume the "default" token & get the appropriate action block.
            parser.Tokens.Consume(TokenType.Default);
            indirectObjects["default"] = BlockStmt.Parse(parser, prepositionEnds);

            // Create an identical PrepStmt for each alias.
            List<PrepositionStmt> preps = [];
            foreach (string alias in aliases)
            {
                preps.Add(new PrepositionStmt(alias, indirectObjects));
            }
            return preps;
        }
    }
}