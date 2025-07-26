namespace CYA_Adventure_Game_Engine.DSL.Runtime
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

    public class Verb : IPhrase
    {
        public PhraseType Type => PhraseType.Verb;
        public string Lexeme { get; }

        public Verb(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class Noun : IPhrase
    {
        public PhraseType Type => PhraseType.Noun;
        public string Lexeme { get; }

        public Noun(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class Preposition : IPhrase
    {
        public PhraseType Type => PhraseType.Preposition;
        public string Lexeme { get; }

        public Preposition(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class IndirectNoun : IPhrase
    {
        public PhraseType Type => PhraseType.IndirectNoun;
        public string Lexeme { get; }

        public IndirectNoun(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class Article : IPhrase
    {
        public PhraseType Type => PhraseType.Article;
        public string Lexeme { get; }

        public Article(string lexeme)
        {
            Lexeme = lexeme;
        }
    }

    public class Command : IPhrase
    {
        public PhraseType Type => PhraseType.Command;
        public string Lexeme { get; }

        public Command(string lexeme)
        {
            Lexeme = lexeme;
        }
    }
}
