using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public class Choice
    {
        public string Text;
        public string Target;
        public List<Action>? Actions;

        public Choice(string text, string target, List<Action>? actions)
        {
            Text = text;
            Target = target;
            Actions = actions;
        }

        public string DebugToString()
        {
            return $"\n\t\t> Option\n\t\t\tText: {Text}\n\t\t\tTarget: {Target}\n\t\t\tActions: {Actions?.ToPrettyString()}";
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
