namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    public class TableRow
    {
        public Dictionary<string, object> Data = [];

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

        public void Set(string name, object val)
        {
            if (Data.TryGetValue(name, out object? value))
            {
                Data[name] = val;
            }
            else { throw new Exception($"Error, tableRow does not contain a record: {name}"); }
        }

        public override string ToString()
        {
            List<string> header = [.. Data.Keys];
            List<string> body = [.. Data.Values.Select(i => i.ToString())];
            // Zip both, keeping the int length of the longest at each index.
            List<int> maxLen = [.. header.Zip(body, (h, b) => h.Length > b.Length ? h.Length : b.Length)];

            List<string> spacedHeads = [];
            List<string> spacedBody = [];
            // Calc the required extra spacing for each item based on the max length of the column its in
            // & add the required spacing.
            for (int i = 0; i < header.Count; i++)
            {
                int headPadding = maxLen[i] - header[i].Length;
                int bodyPadding = maxLen[i] - body[i].Length;
                spacedHeads.Add(header[i] + string.Concat(Enumerable.Repeat(" ", headPadding)));
                spacedBody.Add(body[i] + string.Concat(Enumerable.Repeat(" ", bodyPadding)));
            }
            return string.Join("\n", [string.Join(" | ", spacedHeads), string.Join(" | ", spacedBody)]);
        }
    }
}
