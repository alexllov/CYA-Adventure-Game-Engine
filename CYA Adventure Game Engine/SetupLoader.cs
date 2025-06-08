using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CYA_Adventure_Game_Engine
{
    public class SetupLoader
    {
        private List<string> Lines;
        public Dictionary<string, IModule> State;

        public SetupLoader(string filepath)
        {
            Tokenizer tokens = new(filepath);
            Lines = tokens.SplitLines;
            State = ConstructReferences();

        }

        public void Show()
        {
            foreach (var kvp in State)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }

        private Dictionary<string, IModule> ConstructReferences ()
        {
            Dictionary<string, IModule> state = new();
            foreach (string line in Lines)
            {
                // TODO: should integrate this properly.
                if (line.Contains("load"))
                {
                    //List<string> parts = [.. line.Split(' ', 2)];
                    //state[parts[1]] = parts[1];
                }
                // TODO: fix commented out portion to replace match case nonesense.
                else if (line.Contains('='))
                {
                    List<string> parts = [.. line.Split('=', 2).Select(i => i.Trim())];
                    //state[parts[0]] = parts[1];
                    switch (parts[1])
                    {
                        case "Inventory":
                            state[parts[0]] = new Inventory();
                            break;
                        case "Sound":
                            state[parts[0]] = new Sound();
                            break;
                        default:
                            break;
                    }

                }
        
            }
            return state;
        }
        
    }
}
