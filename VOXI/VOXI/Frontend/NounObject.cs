using System.Text.Json.Serialization;

namespace VOXI.Frontend
{
    public class NounObject
    {
        public string Name;
        public Dictionary<string, IVerb> Verbs;

        public NounObject(NounExpr nounExpr)
        {
            Name = nounExpr.Noun;
            Verbs = nounExpr.Verbs;
        }

        [JsonConstructor]
        public NounObject() { }


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
