using Newtonsoft.Json;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    internal static class SaveLoad
    {
        //////////
        // SAVE //
        //////////
        public static string Serialize(Dictionary<string, object> globalState)
        {
            Dictionary<string, object> serializables = FindSerializables(globalState);
            JsonSerializerSettings settings = new()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
            };
            string json = JsonConvert.SerializeObject(serializables, settings);
            return json;
        }

        private static Dictionary<string, object> FindSerializables(Dictionary<string, object> origin)
        {
            Dictionary<string, object> serializables = new();
            foreach (var kvp in origin)
            {
                try
                {
                    JsonConvert.SerializeObject(kvp.Value);
                    serializables[kvp.Key] = kvp.Value;
                }
                catch { /* Skip over the others. */ }
            }
            return serializables;
        }

        //////////
        // LOAD //
        //////////
        public static bool TrySelectSaveFile(out string jsonPath)
        {
            jsonPath = "";
            // Grab the save jsons & create tuples of full file & name for display.
            string location = AppDomain.CurrentDomain.BaseDirectory;
            string[] saveFiles = Directory.GetFiles(location, "saves\\*.json");
            List<(string, string)> fileChoices = new();
            foreach (string file in saveFiles)
            {
                fileChoices.Add((Path.GetFileNameWithoutExtension(file), file));
            }
            // Add an escape option.
            fileChoices.Add(("BACK", "BACK"));

            // Display saves like choices.
            if (fileChoices.Count > 1)
            {
                Console.WriteLine("\nOptions:");
                int num = 1;
                foreach ((string name, string path) in fileChoices)
                {
                    Console.WriteLine($"{num}. {name}");
                    num++;
                }
            }
            else
            {
                Console.WriteLine("No save files found.");
                return false;
            }

            // User selection loop.
            while (true)
            {
                Console.Write("Select a save to load, or BACK to return: ");
                var choice = Console.ReadLine();

                if (int.TryParse(choice, out int i)
                    && i > 0
                    && i <= fileChoices.Count)
                {
                    jsonPath = fileChoices[i - 1].Item2;
                    break;
                }
                else { Console.WriteLine("Error, Invalid entry."); }
            }
            // Early return for BACK selection.
            if (jsonPath == "BACK") { return false; }

            return true;
        }

        public static Dictionary<string, object> LoadSave(string path)
        {
            string json = File.ReadAllText(path);
            JsonSerializerSettings settings = new()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
            };
            Dictionary<string, object> val = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);
            return val;
        }
    }
}
