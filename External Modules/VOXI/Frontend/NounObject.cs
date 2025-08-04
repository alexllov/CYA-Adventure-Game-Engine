namespace External_Modules.VOXI.Frontend
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
