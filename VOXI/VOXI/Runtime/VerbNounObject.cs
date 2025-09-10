using VOXI.Frontend;
namespace VOXI.Runtime
{
    /// <summary>
    /// Used by the CommandHandler while attempting to handle player input.
    /// </summary>
    public class VerbNounObject
    {
        public Dictionary<(VerbPhrase, NounPhrase), List<string>> VNPhraseDict = [];
        public List<List<string>> Verbs = [];
        public List<List<string>> Nouns = [];

        public VerbNounObject(VOXIEnvironment VOXIstate)
        {
            Dictionary<string, NounObject> nouns = VOXIstate.GetLocalNouns();
            foreach (NounObject noun in nouns.Values)
            {
                foreach (string verb in noun.Verbs.Keys)
                {
                    List<string> verbParts = [.. verb.Split(' ')];
                    List<string> nounParts = [.. noun.Name.Split(' ')];
                    VerbPhrase pVerb = new(verbParts);
                    NounPhrase pNoun = new(nounParts);
                    VNPhraseDict[(pVerb, pNoun)] = [.. pVerb.Lexemes.Concat(pNoun.Lexemes)];
                    Verbs.Add(verbParts);
                    Nouns.Add(nounParts);
                }
            }
        }

        public Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> FindMatchingVerbNounsFromCommand(CommandPhrase command)
        {
            Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> matches = [];
            foreach (KeyValuePair<(VerbPhrase, NounPhrase), List<string>> record in VNPhraseDict)
            {
                if (Enumerable.SequenceEqual(record.Value, command.Lexemes.Take(record.Value.Count)))
                {
                    matches[record.Key] = command;
                }
            }
            return matches;
        }
    }
}
