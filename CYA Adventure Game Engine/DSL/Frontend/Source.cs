namespace CYA_Adventure_Game_Engine.DSL.Frontend
{
    public class Source
    {
        public string FilePath;
        public string RelativePath;
        public string Content;

        public Source(string filePath)
        {
            FilePath = filePath;
            RelativePath = Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, filePath);
            Content = File.ReadAllText(FilePath);
        }

        public static List<Source> LoadSources()
        {
            var location = AppDomain.CurrentDomain.BaseDirectory;
            string mainScript = Directory.GetFiles(location, "./*.cya").FirstOrDefault()
                ?? throw new Exception("No .cya file found in the directory.");
            List<string> extraScripts = [];
            try
            {
                extraScripts = [.. Directory.GetFiles(location, "scripts\\*.cya")];
            }
            catch (DirectoryNotFoundException dirEx) { }

            List<string> addresses = [mainScript, .. extraScripts];
            return [.. addresses.Select(a => new Source(a))];
        }
    }
}
