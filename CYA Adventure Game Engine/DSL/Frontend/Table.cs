using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    public class Table
    {

        public Dictionary<string, TableRow> Data = new();

        public Table(Dictionary<string, TableRow> data)
        {
            Data = data;
        }

        public TableRow Get(string name)
        {
            if (Data.TryGetValue(name, out TableRow? value))
            {
                return value;
            }
            else { throw new Exception($"Error, table does not contain a record: {name}"); }
        }

        public override string ToString()
        {
            List<string> text = new();
            foreach (var row in Data) 
            {
                text.Add($"Name: {row.Key}, Value: {row.Value}");
            }
            return string.Join("\n", text);
        }

    }
}