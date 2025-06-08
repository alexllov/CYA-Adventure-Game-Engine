using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public class Scene
    {
        public required string ID;
        public required List<string> Text;
        public List<Choice> Choices = [];
        public List<Action> Actions = [];


        public string DebugToString()
        {
            return $"\tID: {ID}\n\tTexts: {Text.ToPrettyString()}\n\tChoices: {Choices?.ToPrettyString() ?? "NONE"}\n\tActions: {Actions?.ToPrettyString() ?? "NONE"}";
        }

        public override string ToString()
        {
            string text = String.Join("\n", Text);
            string choices;
            List<string> numberedChoices = new ();
            int idx = 0;
            if (Choices is not [])
            {
                foreach (Choice choice in Choices)
                {
                    idx += 1;
                    numberedChoices.Add($"{idx}. {choice}");
                }
                numberedChoices.Add("X. Enter x to open the Menu.");
                choices = String.Join("\n", numberedChoices);
            }
            else { choices = "There are no paths available to you anymore.\nX. Enter x to open the Menu."; }

            return $"{text}\nChoices:\n{choices}";
        }
    }
}
