using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace External_Modules.VOXI.Frontend
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
            VOXI voxi = (VOXI)state.Modules["voxi"];
            // Get the command from the state.
            string command = voxi.Env.GetCommand();
            if (command == "")
            {
                throw new Exception("Preposition: Command not found! Engine error.");
            }
            else
            {
                // IN SCENE & DEFINED -> stmt
                // IN SCENE & NotDefined -> default
                // NOT IN SCENE -> NOT IN SCENE.
                Dictionary<string, NounObject> local = voxi.Env.GetLocalNouns();
                // Find the requested IndNoun in local nouns.
                if (local.TryGetValue(command, out NounObject? stmt))
                {
                    // If the Ind obj is defined.
                    if (IndirectObjects.TryGetValue(command, out IStmt? indirectObject))
                    {
                        // Set the new command without the indirect object
                        voxi.Env.SetCommand("");
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
            TokenType[] prepositionEndTokens = [TokenType.RCurly];
            string[] prepositionEndStrings = ["prep", "default", "verb", "noun"];

            List<string> aliases = [];

            // id all aliases for this preposition.
            while (parser.Tokens.Peek(0).Type is TokenType.String)
            {
                aliases.Add(parser.Tokens.Consume(TokenType.String).Lexeme);
            }

            // Parse the indirect objects for this prep.
            Dictionary<string, IStmt> indirectObjects = [];
            while (parser.Tokens.IdentLexemeMatch("noun"))
            {
                string name = parser.Tokens.Consume(TokenType.String).Lexeme;
                indirectObjects[name] = BlockStmt.Parse(parser, prepositionEndTokens, prepositionEndStrings);
            }

            // Prep block must end in Default.
            // Consume the "default" token & get the appropriate action block.
            Token token = parser.Tokens.Peek(0);
            if (!parser.Tokens.IdentLexemeMatch("default"))
            {
                throw new Exception("Warning, preposition blocks must always end in a 'default'");
            }

            indirectObjects["default"] = BlockStmt.Parse(parser, prepositionEndTokens, prepositionEndStrings);

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