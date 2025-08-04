using VOXI.Frontend;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace VOXI
{
    public class VOXIEnvironment
    {
        public Environment BaseEnv;
        public VOXIEnvironment(Environment environment)
        {
            BaseEnv = environment;
        }

        public Dictionary<string, NounObject> Nouns = [];

        public Dictionary<string, NounObject> LocalNouns = [];

        public Dictionary<string, NounObject> LocalBackup = [];

        private string Command = new("");

        public bool GetNoun(string noun, out NounObject? value)
        {
            if (Nouns.TryGetValue(noun, out value))
            {
                return true;
            }
            return false;
        }

        // Single.
        public void AddLocalNoun(NounObject noun)
        {
            LocalNouns[noun.Name] = noun;
        }

        // Multiple overload for Overlay reset.
        public void AddLocalNoun(Dictionary<string, NounObject> nouns)
        {
            LocalNouns = nouns;
        }

        public Dictionary<string, NounObject> GetLocalNouns()
        {
            return LocalNouns;
        }

        // Check for presence of a noun & get it back if so.
        public bool HasLocalNoun(string noun, out NounObject? value)
        {
            if (LocalNouns.TryGetValue(noun, out value)) { return true; }
            else { return false; }
        }

        public void SetCommand(string command)
        {
            Command = command;
        }
        public string GetCommand()
        {
            return Command;
        }
    }
}



