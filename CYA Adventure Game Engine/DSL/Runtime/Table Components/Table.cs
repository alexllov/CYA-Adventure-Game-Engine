namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    public class Table
    {

        public Dictionary<string, TableRow> Data;
        public List<string> Attributes;

        public Table(List<string> cols, Dictionary<string, TableRow> data)
        {
            Attributes = cols;
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

        public TableRow GetRandom()
        {
            Random rand = new();
            int index = rand.Next(0, Data.Count);
            return Data.ElementAt(index).Value;
        }

        public override string ToString()
        {
            // Go through each record & calc the longest entry in each column for spacing.
            List<int> maxLenPerCol = [.. Attributes.Select(i => i.Length)];
            foreach (var row in Data)
            {
                var rowVal = row.Value;
                int index = 0;
                foreach (string key in Attributes)
                {
                    int newLen = rowVal.Data[key].ToString().Length;
                    if (newLen > maxLenPerCol[index]) { maxLenPerCol[index] = newLen; }
                    index++;
                }
            }

            // Calc the necessary padding for each record & create the padded records.
            List<string> rows = [];
            int i = 0;
            List<string> attrRow = [];
            // Header Row.
            foreach (string key in Attributes)
            {
                int paddingRequired = maxLenPerCol[i] - key.Length;
                i++;
                string spacing = string.Concat(Enumerable.Repeat(" ", paddingRequired));
                attrRow.Add(key + spacing);
            }
            rows.Add(string.Join(" | ", attrRow));

            // Record Rows.
            foreach (var row in Data)
            {
                List<string> currentRow = [];
                var rowVal = row.Value;
                int index = 0;
                foreach (string key in Attributes)
                {
                    string entry = rowVal.Data[key].ToString();
                    int paddingRequired = maxLenPerCol[index] - entry.Length;
                    index++;
                    string spacing = string.Concat(Enumerable.Repeat(" ", paddingRequired));
                    currentRow.Add(entry + spacing);
                }
                rows.Add(string.Join(" | ", currentRow));
            }
            return string.Join("\n", rows);
        }

    }
}