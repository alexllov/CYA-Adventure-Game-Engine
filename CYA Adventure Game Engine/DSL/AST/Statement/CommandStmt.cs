using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    public class CommandStmt : IStmt
    {
        public string Noun;
        public Dictionary<string, IStmt> Verbs;

        public CommandStmt(string noun, Dictionary<string, IStmt> verbs)
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
            string actString = string.Join(',', actions);
            return $"Comand({Noun}: {actString})";
        }

        public void Interpret(Environment state)
        {
            state.AddLocalCommand(this);
        }
    }
}
