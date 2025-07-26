using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI
{
    public class NounStmt : IStmt
    {
        public string Noun;
        public Dictionary<string, IVerb> Verbs;

        public NounStmt(string noun, Dictionary<string, IVerb> verbs)
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
            return $"NounStmt({Noun}: {actString})";
        }

        public void Interpret(Environment state)
        {
            state.AddNoun(this);
            state.AddLocalNoun(this);
        }

        public bool TryGetVerb(string verb, out IVerb? vStmt)
        {
            if (Verbs.TryGetValue(verb, out vStmt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
