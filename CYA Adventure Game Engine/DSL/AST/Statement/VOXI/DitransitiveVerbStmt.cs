using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI
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
                preps.Add($"Preposition({kvp.Key}): {kvp.Value.ToString()}");
            }
            string prepString = string.Join(',', preps);
            return $"DitransitiveVerb({Verb}: {prepString})";
        }

        public void Interpret(Environment state)
        {
            string command = state.GetCommand();
            if (command == "")
            {
                throw new Exception("DitransitiveVerb: Command not found! Engine error.");
            }
            else
            {
                PrepositionStmt prep = TryMatchPreposition(state, command);
                prep.Interpret(state);
            }
        }

        private PrepositionStmt TryMatchPreposition(Environment state, string command)
        {
            // Split the command & first should be preposition.
            List<string> parts = [.. command.Split(' ')];
            string start = parts[0];
            if (Prepositions.TryGetValue(start, out PrepositionStmt? prep))
            {
                string newCommand = string.Join(' ', parts[1..]);
                state.SetCommand(newCommand);
                return prep;
            }
            else
            {
                // TODO: FORMAT THIS TO RETURN A GRACEFUL ERROR MESSAGE RATHER THAN CRASHING.
                throw new Exception($"DitransitiveVerb: Preposition '{prep}' not found in DitransitiveVerb.");
            }
        }
    }
}
