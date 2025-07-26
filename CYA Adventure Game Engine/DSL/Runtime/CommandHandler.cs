using CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI;
namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    internal static class CommandHandler
    {
        public static void Handle(Environment state, string choice)
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
            Dictionary<(Verb, Noun), Command> identifiedCommands = TryMatchVerbNouns(state, commands);
            // Check for errors to early return.
            if (state.CommandErrors.Count > 0)
            {
                return;
            }

            // 4. Strip Verb-Noun from command.
            // At this point, properly formated transitive verb commands should have a command of "".
            Dictionary<(Verb, Noun), (Command, string)> cleanedCommands = StripVerbNounsFromCommand(identifiedCommands);

            // 5. Get the actual Verbs from nouns
            Dictionary<IVerb, (Command, string)> verbDict = GetVerbs(state, cleanedCommands);

            // 6. Match Verbs with argument types
            MatchVerbsWithArgNumber(state, verbDict);
            // Check for errors to early return.
            if (state.CommandErrors.Count > 0)
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
            foreach (KeyValuePair<IVerb, (Command, string)> kvp in verbDict)
            {
                IVerb verb = kvp.Key;
                Command command = kvp.Value.Item1;
                // Interpret the verb with the command string.
                state.SetCommand(command.Lexeme);
                verb.Interpret(state);
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

        public static Dictionary<(Verb, Noun), Command> TryMatchVerbNouns(Environment state, List<string> commands)
        {
            Dictionary<(Verb, Noun), Command> identifiedCommands = [];
            foreach (string command in commands)
            {
                // 3i. Construct all Verb-Noun strings from local scope.
                Dictionary<(Verb, Noun), string> verbNounStrings = CreateVerbNouns(state);

                // 3ii. Match command to Verb-Nouns.
                Dictionary<(Verb, Noun), string> matchingVerbNouns = MatchVerbNounsToCommand(command, verbNounStrings);
                switch (matchingVerbNouns.Count)
                {
                    case 0:
                        // TODO: Need cleaner message here somehow.
                        state.AddCommandError($"Could not understand {command}");
                        break;
                    case 1:
                        // If only one match, is fine.
                        // TODO:: PUT THE COMMAND CONST HERE......
                        identifiedCommands[matchingVerbNouns.First().Key] = new Command(matchingVerbNouns.First().Value);
                        break;
                    default:
                        // If multiple matches, prompt user to choose.
                        state.AddCommandError($"Multiple matches found for {command}. Please disambiguate the noun using 'the, a, or an'.");
                        break;
                }
            }
            return identifiedCommands;
        }

        public static Dictionary<(Verb, Noun), string> CreateVerbNouns(Environment state)
        {
            Dictionary<(Verb, Noun), string> verbNounStrings = [];
            Dictionary<string, NounStmt> nouns = state.GetLocalNouns();
            foreach (NounStmt noun in nouns.Values)
            {
                foreach (string verb in noun.Verbs.Keys)
                {
                    Verb mVerb = new(verb);
                    Noun mNoun = new(noun.Noun);
                    verbNounStrings[(mVerb, mNoun)] = $"{verb} {noun.Noun}";
                }
            }
            return verbNounStrings;
        }

        public static Dictionary<(Verb, Noun), string> MatchVerbNounsToCommand(string command, Dictionary<(Verb, Noun), string> verbNounStrings)
        {
            Dictionary<(Verb, Noun), string> successfulVerbNouns = [];
            foreach (KeyValuePair<(Verb, Noun), string> record in verbNounStrings)
            {
                if (command.StartsWith(record.Value))
                {
                    // Pass command here to preserve the whole: eg. 'take rod from shelf'.
                    successfulVerbNouns[record.Key] = command;
                }
            }
            return successfulVerbNouns;
        }


        public static Dictionary<(Verb, Noun), (Command, string)> StripVerbNounsFromCommand(Dictionary<(Verb, Noun), Command> dirtyCommands)
        {
            Dictionary<(Verb, Noun), (Command, string)> cleanedCommands = [];
            foreach (KeyValuePair<(Verb, Noun), Command> kvp in dirtyCommands)
            {
                Verb verb = kvp.Key.Item1;
                Noun noun = kvp.Key.Item2;
                string command = kvp.Value.Lexeme;
                // Remove the verb and noun from the command.
                string cleanedCommand = command.Replace($"{verb.Lexeme} {noun.Lexeme}", "").Trim();
                cleanedCommands[(verb, noun)] = (new Command(cleanedCommand), command);
            }
            return cleanedCommands;
        }

        public static Dictionary<IVerb, (Command, string)> GetVerbs(Environment state, Dictionary<(Verb, Noun), (Command, string)> cleanedCommands)
        {
            Dictionary<IVerb, (Command, string)> verbDict = [];
            foreach (KeyValuePair<(Verb, Noun), (Command, string)> kvp in cleanedCommands)
            {
                Verb verb = kvp.Key.Item1;
                Noun noun = kvp.Key.Item2;
                if (state.GetLocalNouns().TryGetValue(noun.Lexeme, out NounStmt? nStmt))
                {
                    if (nStmt.TryGetVerb(verb.Lexeme, out IVerb? vStmt))
                    {
                        verbDict[vStmt!] = (kvp.Value);
                    }
                }
            }
            return verbDict;
        }

        private static void MatchVerbsWithArgNumber(Environment state, Dictionary<IVerb, (Command, string)> verbDict)
        {
            foreach (KeyValuePair<IVerb, (Command, string)> kvp in verbDict)
            {
                // Too many args error.
                if (kvp.Key is TransitiveVerbStmt && kvp.Value.Item1.Lexeme != "")
                {
                    state.AddCommandError($"{kvp.Value.Item2} has too many arguments.");
                }
                // Too few args error.
                else if (kvp.Key is DitransitiveVerbStmt && kvp.Value.Item1.Lexeme == "")
                {
                    state.AddCommandError($"{kvp.Value.Item2} has too few arguments.");
                }
                // Just Right :)
            }
        }
    }
}