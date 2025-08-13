using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class TokenizerTest
    {
        // (say "hello world")
        // 1 + 2 == 3
        //scene "1"
        //["pancake" -> "1"]
        [Fact]
        public void TokenizerTxtFile()
        {
            // Arrange
            TokenList expectedTokens = new([
                // (say "hello world")
                new Token(TokenType.LParent, "(", 1, 1),
                new Token(TokenType.Identifier, "say", 1, 1),
                new Token(TokenType.String, "hello world", 1, 1),
                new Token(TokenType.RParent, ")", 1, 1),

                // 1 + 2 == 3
                new Token(TokenType.Number, "1", 1, 1),
                new Token(TokenType.Plus, "+", 1, 1),
                new Token(TokenType.Number, "2", 1, 1),
                new Token(TokenType.Equal, "==", 1, 1),
                new Token(TokenType.Number, "3", 1, 1),

                // scene "1"
                new Token(TokenType.Scene, "scene", 1, 1),
                new Token(TokenType.String, "1", 1, 1),

                // ["pancake" -> "1"]
                new Token(TokenType.LBracket, "[", 1, 1),
                new Token(TokenType.String, "pancake", 1, 1),
                new Token(TokenType.GoTo, "->", 1, 1),
                new Token(TokenType.String, "1", 1, 1),
                new Token(TokenType.RBracket, "]", 1, 1),
            ]);

            // Act
            Dictionary<string, TokenType> keywords = BaseKeywords.Keywords;
            Tokenizer tokenizer = new("./Tokenizer/TestFile.cya", keywords);
            TokenList tokens = tokenizer.Tokenize();

            // Assert
            Assert.Equivalent(expectedTokens, tokens);

        }
    }
}