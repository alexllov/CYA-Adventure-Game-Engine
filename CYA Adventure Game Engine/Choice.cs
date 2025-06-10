using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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

        public (bool, string) QueryActions(Dictionary<string, IModule>  state)
        {
            //Collect Failures.
            List<string> failures = [];
            foreach (Action action in Actions)
            {
                (bool, string) result = action.Query(state);
                if (!result.Item1) { failures.Add(result.Item2); }
            }

            // Defaults assuming pass.
            bool flag = true;
            string msg = "";
            if (failures.Count > 0)
            {
                flag = false;
                msg = string.Join(", ", failures);
            }
            return (flag, msg);
        }

        public void ProcessActions(Dictionary<string, IModule> state)
        {
            foreach (Action action in Actions)
            {
                action.Process(state);
            }
        }
    }
}
