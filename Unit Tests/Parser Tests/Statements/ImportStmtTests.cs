using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{

    public class ImportStmtTest
    {
        // -> "test"
        [Fact]
        public void ImportNoAlias()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Import, "import", 1, 1, ""),
                                  new Token(TokenType.Identifier, "inventory", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new ImportStmt("inventory")
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }


        // -> Variable
        [Fact]
        public void ImportWithAlias()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Import, "import", 1, 1, ""),
                                  new Token(TokenType.Identifier, "inventory", 1, 1, ""),
                                  new Token(TokenType.As, "as", 1, 1, ""),
                                  new Token(TokenType.Identifier, "i", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new ImportStmt("inventory", "i")
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }

    }
}