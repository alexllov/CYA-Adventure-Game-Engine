namespace External_Modules.VOXI
{
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
        string Lexeme { get; }
    }

    public class VerbPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Verb;
        public string Lexeme { get; }

        public VerbPhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class NounPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Noun;
        public string Lexeme { get; }

        public NounPhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class PrepositionPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Preposition;
        public string Lexeme { get; }

        public PrepositionPhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class IndirectNounPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.IndirectNoun;
        public string Lexeme { get; }

        public IndirectNounPhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class ArticlePhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Article;
        public string Lexeme { get; }

        public ArticlePhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class CommandPhrase : IPhrase
    {
        public PhraseType Type => PhraseType.Command;
        public string Lexeme { get; }

        public CommandPhrase(string lexeme)
        {
            Lexeme = lexeme;
        }
    }
}
