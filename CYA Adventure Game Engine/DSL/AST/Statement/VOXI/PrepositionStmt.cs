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
                // Remaining command should be the indirect object.
                if (IndirectObjects.TryGetValue(command, out IStmt? indirectObject))
                {
                    // Set the new command without the indirect object
                    state.SetCommand("");
                    indirectObject.Interpret(state);
                }
                else
                {
                    IndirectObjects["default"].Interpret(state);
                }
            }
        }
    }
}