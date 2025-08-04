
using External_Modules.VOXI.Frontend;
using External_Modules.VOXI.Runtime;
namespace External_Modules.VOXI
{
    public static class CommandHandler
    {
        public static void Handle(VOXIEnvironment VOXIState, string choice)
        {
            // If env localNouns is empty, then a VOXI input shouldn't be allowed.
            if (VOXIState.LocalNouns.Count == 0)
            {
                return;
            }

            List<string> tokens = [.. choice.ToLower().Split(' ')];

            // 1. Strip the articles.
            tokens = StripArticles(tokens);

            // 2. Split on conjunctions.
            List<string> commandStrings = SplitOnConjunctions(tokens);
            List<CommandPhrase> commands = [.. commandStrings.Select(c => new CommandPhrase([.. c.Split(' ')]))];

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
                VOXIState.SetCommand(command.String);
                verb.Interpret(VOXIState.BaseEnv);
            }
            return;
        }

        // Articles arejust removed.
        // A better implementation would use these as indicators for NP identification.
        public static List<string> StripArticles(List<string> tokens)
        {
            List<string> articles = ["a", "an", "the"];
            return [.. tokens.Where(token => !articles.Contains(token))];
        }

        // CONJ used as delimiterto split on & separate into a list of commands.
        public static List<string> SplitOnConjunctions(List<string> tokens)
        {
            List<string> conjunctions = ["and", "then"];
            List<string> commands = [];
            List<string> command = [];
            foreach (string token in tokens)
            {
                if (conjunctions.Contains(token))
                {
                    // This avoids creating an empty command if a user enters "xyz and then xyz"
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


        public static Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> TryMatchVerbNouns(VOXIEnvironment VOXIstate, List<CommandPhrase> commands)
        {
            Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> identifiedCommands = [];

            // 3i. Construct all Verb-NounObject strings from local scope.
            VerbNounObject localVerbNouns = CreateVerbNouns(VOXIstate);

            foreach (CommandPhrase command in commands)
            {
                // 3ii. Match command to Verb-Nouns.
                Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> matches = localVerbNouns.FindMatchingVerbNounsFromCommand(command);
                switch (matches.Count)
                {
                    case 0:
                        // TODO: Need cleaner message here somehow.
                        VOXIstate.BaseEnv.AddCommandError($"Could not understand {command.String}");
                        break;
                    case 1:
                        // If only one match, is fine.
                        // TODO:: PUT THE COMMAND CONST HERE......
                        identifiedCommands[matches.First().Key] = matches.First().Value;
                        break;
                    default:
                        // If multiple matches, prompt user to choose.
                        VOXIstate.BaseEnv.AddCommandError($"Multiple matches found for {command}. Please disambiguate the noun using 'the, a, or an'.");
                        break;
                }
            }
            return identifiedCommands;
        }

        /// <summary>
        /// Creates a dictionary of available VP-NP (Verb-Noun) strings
        /// from the Nouns available in local scope & all verbs applicable to them.
        /// </summary>
        /// <param name="VOXIstate"></param>
        /// <returns>Dictionary<(VerbPhrase, NounPhrase), string> VP-NP strings w/ Noun, Verb key</returns>
        public static VerbNounObject CreateVerbNouns(VOXIEnvironment VOXIstate)
        {
            return new VerbNounObject(VOXIstate);
        }

        public static Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> StripVerbNounsFromCommand(Dictionary<(VerbPhrase, NounPhrase), CommandPhrase> dirtyCommands)
        {
            Dictionary<(VerbPhrase, NounPhrase), (CommandPhrase, string)> cleanedCommands = [];
            foreach (KeyValuePair<(VerbPhrase Verb, NounPhrase Noun), CommandPhrase> kvp in dirtyCommands)
            {
                int VerbAndNounPhraseLength = kvp.Key.Verb.Lexemes.Count() + kvp.Key.Noun.Lexemes.Count();
                List<string> command = kvp.Value.Lexemes;
                // Remove the verb and noun from the command.
                // TODO: Test slice position here.
                CommandPhrase leftoverCommand = new(command[VerbAndNounPhraseLength..]);
                cleanedCommands[kvp.Key] = (leftoverCommand, string.Join(' ', command));
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
                if (VOXIState.GetLocalNouns().TryGetValue(noun.String, out NounObject? nObj))
                {
                    if (nObj.TryGetVerb(verb.String, out IVerb? vStmt))
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
                if (kvp.Key is TransitiveVerbStmt && kvp.Value.Item1.String != "")
                {
                    VOXIState.BaseEnv.AddCommandError($"{kvp.Value.Item2} has too many arguments.");
                }
                // Too few args error.
                else if (kvp.Key is DitransitiveVerbStmt && kvp.Value.Item1.String == "")
                {
                    VOXIState.BaseEnv.AddCommandError($"{kvp.Value.Item2} has too few arguments.");
                }
                // Just Right :)
            }
        }
    }
}