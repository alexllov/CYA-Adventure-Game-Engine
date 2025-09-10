using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace VOXI.Frontend
{
    public class DitransitiveVerbStmt : IVerb
    {
        public string Verb { get; set; }
        public Dictionary<string, PrepositionStmt> Prepositions;

        public DitransitiveVerbStmt(string verb, Dictionary<string, PrepositionStmt> prepositions)
        {
            Verb = verb;
            Prepositions = prepositions;
        }

        public override string ToString()
        {
            List<string> preps = [];
            foreach (var kvp in Prepositions)
            {
                preps.Add($"\n    Preposition({kvp.Key}): {kvp.Value.ToString()}");
            }
            string prepString = string.Join(',', preps);
            return $"DitransitiveVerb({Verb}: {prepString})";
        }

        public void Interpret(Environment state)
        {
            VOXI voxi = (VOXI)state.Modules["voxi"];
            string command = voxi.Env.GetCommand();
            if (command == "")
            {
                throw new Exception("DitransitiveVerb: Command not found! Engine error.");
            }
            else
            {
                // ID The correct PrepositionStmt from the command.
                if (TryMatchPreposition(state, command, out PrepositionStmt prep))
                {
                    prep.Interpret(state);
                }
            }
        }

        /// <summary>
        /// Uses the remaining command string tracked in VOXIEnvironment to try & match the preposition.
        /// </summary>
        /// <returns>Bool, out PrepositionStmt if successful</returns>
        private bool TryMatchPreposition(Environment state, string command, out PrepositionStmt prep)
        {
            VOXI voxi = (VOXI)state.Modules["voxi"];
            // Split the command & first should be preposition.
            List<string> parts = [.. command.Split(' ')];
            string start = parts[0];
            if (Prepositions.TryGetValue(start, out prep))
            {
                string newCommand = string.Join(' ', parts[1..]);
                voxi.Env.SetCommand(newCommand);
                return true;
            }
            else
            {
                voxi.Env.AddVOXIError($"Couldn't understand {parts[0]} as a preposition.");
                return false;
            }
        }

        /// <summary>
        /// Parses DitransitiveVerbStmt & constructs a list of them for each alias provided.
        /// These consist of a verb alias, and a Preposition statement.
        /// Example "'give' note 'to sally'"
        /// </summary>
        /// <param name="verbAliases"></param>
        /// <returns>List<DitransitiveVerbStmt></returns>
        public static List<DitransitiveVerbStmt> ParseDitransitiveVerbs(Parser parser, List<string> verbAliases)
        {
            // Get all the prepositions & attached indr objects.
            Dictionary<string, PrepositionStmt> prepositions = [];

            Token token = parser.Tokens.Peek(0);
            while (token.Type is TokenType.Identifier
                   && token.Lexeme is "prep")
            {
                // Eat the 'prep'
                parser.Tokens.Advance();
                List<PrepositionStmt> preps = PrepositionStmt.ParsePrepositions(parser);
                foreach (PrepositionStmt prep in preps)
                {
                    prepositions[prep.Name] = prep;
                }
                // Update token.
                token = parser.Tokens.Peek(0);
            }

            // Construct a ditrans verb for each alias
            List<DitransitiveVerbStmt> ditransVerbs = [];
            foreach (string verb in verbAliases)
            {
                ditransVerbs.Add(new DitransitiveVerbStmt(verb, prepositions));
            }
            return ditransVerbs;
        }
    }
}
