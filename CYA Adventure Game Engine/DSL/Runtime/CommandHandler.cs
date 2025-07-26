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
            (List<string> problems, Dictionary<(Verb, Noun), Command> identifiedCommands) = TryMatchVerbNouns(state, commands);
            // This is debugging for now.
            Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
            foreach (KeyValuePair<(Verb, Noun), Command> kvp in identifiedCommands)
            {
                Console.WriteLine($"Identified Command: {kvp.Value.Lexeme}");
            }
            if (problems.Count > 0)
            {
                Console.WriteLine("Found problems, early return");
                // TODO: ADD EARLY RETURN WITH THE ERRORS HERE.
                return;
            }

            // 4. Strip Verb-Noun from command.
            // At this point, properly formated transitive verb commands should have a command of "".
            Dictionary<(Verb, Noun), Command> cleanedCommands = StripVerbNounsFromCommand(identifiedCommands);
            foreach (KeyValuePair<(Verb, Noun), Command> kvp in cleanedCommands)
            {
                Console.WriteLine($"Cleaned Command: {kvp.Value.Lexeme}");
            }

            // 5. Identify the Trans & Ditrans Verbs/ get the actual Verbs from nouns.
            Dictionary<IVerb, Command> verbDict = GetVerbs(state, cleanedCommands);
            // TODO: BUILD AN OUT ON THIS TO GET ERR MESSAGES FOR THE BAD COMMANDS.
            if (!TryMatchVerbTypesAndCommand(verbDict))
            {
                //RETURN EARLY ON MISFORMED COMMAND.
                return;
            }

            // 6. Interpret the VerbStmts.
            foreach (KeyValuePair<IVerb, Command> kvp in verbDict)
            {
                IVerb verb = kvp.Key;
                Command command = kvp.Value;
                // Interpret the verb with the command string.
                state.SetCommand(command.Lexeme);
                verb.Interpret(state);
            }


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

        public static (List<string>, Dictionary<(Verb, Noun), Command>) TryMatchVerbNouns(Environment state, List<string> commands)
        {
            List<string> problems = [];
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
                        problems.Add($"Could not understand {command}");
                        break;
                    case 1:
                        // If only one match, is fine.
                        // TODO:: PUT THE COMMAND CONST HERE......
                        identifiedCommands[matchingVerbNouns.First().Key] = new Command(matchingVerbNouns.First().Value);
                        break;
                    default:
                        // If multiple matches, prompt user to choose.
                        problems.Add($"Multiple matches found for {command}. Please disambiguate the noun using 'the, a, or an'.");
                        break;
                }
            }
            return (problems, identifiedCommands);
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


        public static Dictionary<(Verb, Noun), Command> StripVerbNounsFromCommand(Dictionary<(Verb, Noun), Command> dirtyCommands)
        {
            Dictionary<(Verb, Noun), Command> cleanedCommands = [];
            foreach (KeyValuePair<(Verb, Noun), Command> kvp in dirtyCommands)
            {
                Verb verb = kvp.Key.Item1;
                Noun noun = kvp.Key.Item2;
                string command = kvp.Value.Lexeme;
                // Remove the verb and noun from the command.
                string cleanedCommand = command.Replace($"{verb.Lexeme} {noun.Lexeme}", "").Trim();
                cleanedCommands[(verb, noun)] = new Command(cleanedCommand);
            }
            return cleanedCommands;
        }

        public static Dictionary<IVerb, Command> GetVerbs(Environment state, Dictionary<(Verb, Noun), Command> cleanedCommands)
        {
            Dictionary<IVerb, Command> verbDict = [];
            foreach (KeyValuePair<(Verb, Noun), Command> kvp in cleanedCommands)
            {
                Verb verb = kvp.Key.Item1;
                Noun noun = kvp.Key.Item2;
                if (state.GetLocalNouns().TryGetValue(noun.Lexeme, out NounStmt? nStmt))
                {
                    if (nStmt.TryGetVerb(verb.Lexeme, out IVerb? vStmt))
                    {
                        verbDict[vStmt!] = kvp.Value;
                    }
                }
            }
            return verbDict;
        }

        public static bool TryMatchVerbTypesAndCommand(Dictionary<IVerb, Command> verbDict)
        {
            foreach (KeyValuePair<IVerb, Command> kvp in verbDict)
            {
                IVerb verb = kvp.Key;
                Command remainingCommand = kvp.Value;
                if (verb is TransitiveVerbStmt && remainingCommand.Lexeme == ""
                    || verb is DitransitiveVerbStmt && remainingCommand.Lexeme != "")
                {
                    continue;
                }
                else { return false; }
            }
            return true;
        }
    }
}