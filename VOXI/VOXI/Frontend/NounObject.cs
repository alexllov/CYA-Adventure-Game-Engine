namespace VOXI.Frontend
{
    public class NounObject
    {
        public string Name;
        public Dictionary<string, IVerb> Verbs;

        public NounObject(string name, Dictionary<string, IVerb> verbs)
        {
            Name = name;
            Verbs = verbs;
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
