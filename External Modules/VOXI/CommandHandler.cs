
using External_Modules.VOXI.Frontend;
namespace External_Modules.VOXI
{
    public static class CommandHandler
    {
        public static void Handle(VOXIEnvironment VOXIState, string choice)
        {
            List<string> tokens = [.. choice.ToLower().Split(' ')];

            // 1. Strip the articles.
            tokens = StripArticles(tokens);

            // 2. Split on conjunctions.
            List<string> commands = SplitOnConjunctions(tokens);

            /*
             * 3. Construct simple Verb-Nouns & try to match on them.
             *    This is before we care about trans/ditrans, just looking for verb on noun.
             *    
             *    Errors 2 types:
             *    1. Commands that are not understood - no match found.
             *    2. Ambiguous commands - multiple matches found. 
             */
            Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> identifiedCommands = TryMatchVerbNouns(VOXIState, commands);
            // Check for errors to early return.
            if (VOXIState.BaseEnv.CommandErrors.Count > 0)
            {
                return;
            }

            // 4. Strip Verb-NounObject from command.
            // At this point, properly formated transitive verb commands should have a command of "".
            Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> cleanedCommands = StripVerbNounsFromCommand(identifiedCommands);

            // 5. Get the actual Verbs from nouns
            Dictionary<IVerb, (CommandPhrase, string)> verbDict = GetVerbs(VOXIState, cleanedCommands);

            // 6. Match Verbs with argument types
            MatchVerbsWithArgNumber(VOXIState, verbDict);
            // Check for errors to early return.
            if (VOXIState.BaseEnv.CommandErrors.Count > 0)
            {
                return;
            }

            /*
             * 7. Interpret the VerbStmts.
             * These may fail in the following ways:
             * - Ditransitive verb with invalid preposition.
             * - Ditransitive verb with invalid Indir Object.
             * All failures added to state properly.
             */
            foreach (KeyValuePair<IVerb, (CommandPhrase, string)> kvp in verbDict)
            {
                IVerb verb = kvp.Key;
                CommandPhrase command = kvp.Value.Item1;
                // Interpret the verb with the command string.
                VOXIState.SetCommand(command.Lexeme);
                verb.Interpret(VOXIState.BaseEnv);
            }
            return;
        }

        public static List<string> StripArticles(List<string> tokens)
        {
            List<string> articles = ["a", "an", "the"];
            return [.. tokens.Where(token => !articles.Contains(token))];
        }

        public static List<string> SplitOnConjunctions(List<string> tokens)
        {
            List<string> conjunctions = ["and", "then"];
            List<string> commands = [];
            List<string> command = [];
            foreach (string token in tokens)
            {
                if (conjunctions.Contains(token))
                {
                    if (command.Count > 0)
                    {
                        commands.Add(string.Join(' ', command));
                        command = [];
                    }
                }
                else
                {
                    command.Add(token);
                }
            }
            // Add the last command.
            commands.Add(string.Join(' ', command));
            return commands;
        }

        public static Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> TryMatchVerbNouns(VOXIEnvironment VOXIstate, List<string> commands)
        {
            Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> identifiedCommands = [];
            foreach (string command in commands)
            {
                // 3i. Construct all Verb-NounObject strings from local scope.
                Dictionary<(VerbPhrase, NounPhrase), string> verbNounStrings = CreateVerbNouns(VOXIstate);

                // 3ii. Match command to Verb-Nouns.
                Dictionary<(VerbPhrase, NounPhrase), string> matchingVerbNouns = MatchVerbNounsToCommand(command, verbNounStrings);
                switch (matchingVerbNouns.Count)
                {
                    case 0:
                        // TODO: Need cleaner message here somehow.
                        VOXIstate.BaseEnv.AddCommandError($"Could not understand {command}");
                        break;
                    case 1:
                        // If only one match, is fine.
                        // TODO:: PUT THE COMMAND CONST HERE......
                        identifiedCommands[matchingVerbNouns.First().Key] = new CommandPhrase(matchingVerbNouns.First().Value);
                        break;
                    default:
                        // If multiple matches, prompt user to choose.
                        VOXIstate.BaseEnv.AddCommandError($"Multiple matches found for {command}. Please disambiguate the noun using 'the, a, or an'.");
                        break;
                }
            }
            return identifiedCommands;
        }

        public static Dictionary<(VerbPhrase, NounPhrase), string> CreateVerbNouns(VOXIEnvironment VOXIstate)
        {
            Dictionary<(VerbPhrase, NounPhrase), string> verbNounStrings = [];
            Dictionary<string, NounObject> nouns = VOXIstate.GetLocalNouns();
            foreach (NounObject noun in nouns.Values)
            {
                foreach (string verb in noun.Verbs.Keys)
                {
                    VerbPhrase mVerb = new(verb);
                    NounPhrase mNoun = new(noun.Name);
                    verbNounStrings[(mVerb, mNoun)] = $"{verb} {noun.Name}";
                }
            }
            return verbNounStrings;
        }

        public static Dictionary<(VerbPhrase, NounPhrase), string> MatchVerbNounsToCommand(string command, Dictionary<(VerbPhrase, NounPhrase), string> verbNounStrings)
        {
            Dictionary<(VerbPhrase, NounPhrase), string> successfulVerbNouns = [];
            foreach (KeyValuePair<(VerbPhrase, NounPhrase), string> record in verbNounStrings)
            {
                if (command.StartsWith(record.Value))
                {
                    // Pass command here to preserve the whole: eg. 'take rod from shelf'.
                    successfulVerbNouns[record.Key] = command;
                }
            }
            return successfulVerbNouns;
        }


        public static Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> StripVerbNounsFromCommand(Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> dirtyCommands)
        {
            Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> cleanedCommands = [];
            foreach (KeyValuePair<(VerbPhrase, NounPhrase), CommandPhrase> kvp in dirtyCommands)
            {
                VerbPhrase verb = kvp.Key.Item1;
                NounPhrase noun = kvp.Key.Item2;
                string command = kvp.Value.Lexeme;
                // Remove the verb and noun from the command.
                string cleanedCommand = command.Replace($"{verb.Lexeme} {noun.Lexeme}", "").Trim();
                cleanedCommands[(verb, noun)] = (new CommandPhrase(cleanedCommand), command);
            }
            return cleanedCommands;
        }

        public static Dictionary<IVerb, (CommandPhrase, string)> GetVerbs(VOXIEnvironment VOXIState, Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> cleanedCommands)
        {
            Dictionary<IVerb, (CommandPhrase, string)> verbDict = [];
            foreach (KeyValuePair<(VerbPhrase, NounPhrase), (CommandPhrase, string)> kvp in cleanedCommands)
            {
                VerbPhrase verb = kvp.Key.Item1;
                NounPhrase noun = kvp.Key.Item2;
                if (VOXIState.GetLocalNouns().TryGetValue(noun.Lexeme, out NounObject? nObj))
                {
                    if (nObj.TryGetVerb(verb.Lexeme, out IVerb? vStmt))
                    {
                        verbDict[vStmt!] = (kvp.Value);
                    }
                }
            }
            return verbDict;
        }

        private static void MatchVerbsWithArgNumber(VOXIEnvironment VOXIState, Dictionary<IVerb, (CommandPhrase, string)> verbDict)
        {
            foreach (KeyValuePair<IVerb, (CommandPhrase, string)> kvp in verbDict)
            {
                // Too many args error.
                if (kvp.Key is TransitiveVerbStmt && kvp.Value.Item1.Lexeme != "")
                {
                    VOXIState.BaseEnv.AddCommandError($"{kvp.Value.Item2} has too many arguments.");
                }
                // Too few args error.
                else if (kvp.Key is DitransitiveVerbStmt && kvp.Value.Item1.Lexeme == "")
                {
                    VOXIState.BaseEnv.AddCommandError($"{kvp.Value.Item2} has too few arguments.");
                }
                // Just Right :)
            }
        }
    }
}