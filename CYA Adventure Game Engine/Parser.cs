using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CYA_Adventure_Game_Engine
{
    public class Parser
    {
        public Dictionary<string, Scene> Data;
        public Parser(Tokenizer tokenizer) 
        {
            List<string> lines = tokenizer.SplitLines;
            Data = ParseLines(lines);
        }

        private void Show()
        {
            foreach (var kvp in Data)
            {
                Console.WriteLine($"{kvp.Key}:\n{kvp.Value}");
            }
        }

        private List<Action> ParseActions(string line)
        {
            List<Action> actions = new();
            // TODO: this needs improving for nested actions later.
            // Split line into actions based on []
            // .Select(i => i[1..]) <- should hopefully knock off '['.
            string targetDelim = "]";
            List<string> lines = line
                .Split(targetDelim, StringSplitOptions.RemoveEmptyEntries)
                .Select(i => i.Trim())
                .ToList();

            foreach (string raw in lines)
            {
                string cleaned = raw;
                if (raw.Length > 1) {cleaned = raw[1..];} 
                // Set bases in case of missing parts.
                string address;
                string method;
                List<string> body;
                // TODO: check for base func vs modules
                // 1, Split address off [i.add ...]
                if (raw.Contains('.'))
                {
                    targetDelim = ".";
                    var splitOffText = cleaned.Split(targetDelim, 2).Select(i => i.Trim()).ToList();
                    address = splitOffText[0];
                    var rest = splitOffText[1];

                    // 2, Split method off [add shoe,...]
                    if (rest.Contains(' '))
                    {
                        targetDelim = " ";
                        splitOffText = rest.Split(targetDelim, 2).Select(i => i.Trim()).ToList();
                        method = splitOffText[0];
                        rest = splitOffText[1];

                        if (rest.Contains(','))
                        {
                            targetDelim = ",";
                            body = rest.Split(targetDelim).Select(i => i.Trim()).ToList();
                        }
                        else { body = [rest]; }
                    }
                    else { method = rest; body = [""]; }
                }
                else { address = ""; method = cleaned; body = [""]; }
                //Console.WriteLine($"Built Action:\nAddress: {address}\nMethod: {method}\nBody: {body}\n####################");
                Action action = new(address, method, body);
                actions.Add(action);
            }
            return actions;
        }

        private Choice ParseChoice(string line)
        {
            string text;
            string target;
            List<Action>? actions = null;

            line = line[1..].Trim();

            // id 1st ->
            string targetDelim = "->";
            var splitOffText = line.Split(targetDelim, 2).Select(i => i.Trim()).ToList();
            text = splitOffText[0];
            var rest = splitOffText[1];

            if (rest.Contains(']'))
            {
                List<string> parts = rest.Split(' ', 2).ToList();
                target = parts[0];
                // TODO: Scrat for now, gives RAW. Need to add Action processing.
                actions = ParseActions(parts[1]);
            }
            else
            {
                target = rest.Trim();
            }

            // USE THIS IN FUTURE.
            // Choice choice = new Choice(text, target, actions);
            // DEBUG
            //if (actions is not null)
            //{
            //Console.WriteLine("length actions = ", actions.Count);
            //}
            return new Choice (text, target, actions);
        }

        private Dictionary<string, Scene> ParseLines(List<string> lines)
        {
            Dictionary<string, Scene> data = new ();
            string? currentScene = null;
            List<string> texts = new ();
            List<Choice> choices = new ();
            List<Action> sceneActions = new ();

            foreach (string line in lines)
            {
                // Skip blanks & comments.
                if (line == "" || line.StartsWith("//")){
                    continue;
                }

                // Setting up new scene.
                else if (char.IsLetterOrDigit(line[0]))
                {
                    if (currentScene is not null)
                    {
                        data[currentScene] = new Scene ()
                        {
                            ID = currentScene,
                            Text = texts,
                            Choices = choices,
                            Actions = sceneActions
                        };
                    }
                    currentScene = line;
                    texts = [];
                    choices = [];
                    sceneActions = [];
                }

                // ID text lines.
                else if (line.StartsWith('"'))
                {
                    // Remove the string "" from start & end to clean.
                    string croppedLine = line[1..];
                    croppedLine = croppedLine[..(croppedLine.Length-1)];

                    texts.Add(croppedLine);
                }

                // ID Choices.
                else if (line.StartsWith('>'))
                {
                    Choice choice = ParseChoice(line);
                    choices.Add(choice);
                }
            }

            return data;
        }
    }
}
