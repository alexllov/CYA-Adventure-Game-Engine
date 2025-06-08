namespace CYA_Adventure_Game_Engine
{
    public class Tokenizer
    {
        private List<string> Tokens;
        public List<string> SplitLines;

        public Tokenizer(string filepath)
        {
            string BareFile = ReadFile(filepath);
            SplitLines = SeparateToLines(BareFile);

            Tokens = new List<string>();
        }
        public void Show()
        {
            Console.WriteLine(String.Join("\n", SplitLines));
        }

        private string ReadFile(string filepath)
        {
            return File.ReadAllText(filepath);
        }

        private List<string> SeparateToLines(string BareFile)
        {
            List<string> lines = new List<string>(BareFile.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None));
            return lines;
        }
    }
}
