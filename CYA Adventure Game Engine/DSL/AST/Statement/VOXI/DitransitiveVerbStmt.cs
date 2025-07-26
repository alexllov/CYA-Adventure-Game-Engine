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
                preps.Add($"\n    Preposition({kvp.Key}): {kvp.Value.ToString()}");
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
                if (TryMatchPreposition(state, command, out PrepositionStmt prep))
                {
                    prep.Interpret(state);
                }
            }
        }

        private bool TryMatchPreposition(Environment state, string command, out PrepositionStmt prep)
        {
            // Split the command & first should be preposition.
            List<string> parts = [.. command.Split(' ')];
            string start = parts[0];
            if (Prepositions.TryGetValue(start, out prep))
            {
                string newCommand = string.Join(' ', parts[1..]);
                state.SetCommand(newCommand);
                return true;
            }
            else
            {
                Console.WriteLine($"Couldn't understand {parts[0]} as a preposition.");
                state.AddCommandError($"Couldn't understand {parts[0]} as a preposition.");
                return false;
            }
        }
    }
}
