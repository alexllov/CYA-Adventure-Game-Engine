using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    public class TableRow
    {
        public Dictionary<string, object> Data = new();

        public TableRow(Dictionary<string, object> data)
        {
            Data = data;
        }

        public object Get(string name)
        {
            if (Data.TryGetValue(name, out object? value))
            {
                return value;
            }
            else { throw new Exception($"Error, tableRow does not contain a record: {name}"); }
        }

        public override string ToString()
        {
            List<string> textList = new();
            foreach (var kvp in Data)
            {
                textList.Add($"Key: {kvp.Key}, Val: {kvp.Value}");
            }
            return string.Join("\n", textList);
        }
    }
}
