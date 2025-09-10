namespace VOXI.Runtime
{
    /// <summary>
    /// Used by the CommandHandler while attempting to handle player input.
    /// </summary>
    public enum PhraseType
    {
        Verb,
        Noun,
        Preposition,
        IndirectNoun,
        Article,
        Command,
    }

    public interface IPhrase
    {
        PhraseType Type { get; }
        List<string> Lexemes { get; }
        string String { get; }
    }

    public class VerbPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Verb;
        public List<string> Lexemes { get; }
        public string String { get; }

        public VerbPhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }

    public class NounPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Noun;
        public List<string> Lexemes { get; }
        public string String { get; }

        public NounPhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }

    public class PrepositionPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Preposition;
        public List<string> Lexemes { get; }
        public string String { get; }

        public PrepositionPhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }

    public class IndirectNounPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.IndirectNoun;
        public List<string> Lexemes { get; }
        public string String { get; }

        public IndirectNounPhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }

    public class ArticlePhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Article;
        public List<string> Lexemes { get; }
        public string String { get; }

        public ArticlePhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }

    public class CommandPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Command;
        public List<string> Lexemes { get; }
        public string String { get; }

        public CommandPhrase(List<string> lexemes)
        {
            Lexemes = lexemes;
            String = string.Join(' ', lexemes);
        }
    }
}
