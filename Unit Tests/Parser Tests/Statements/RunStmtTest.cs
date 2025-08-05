using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    /*
     * Type of assignment allowed:
     * Identifier = Expression
     */
    public class RunStmtTest
    {
        // run "test" -> RunStmt("test")
        [Fact]
        public void StringTargetRunStmt()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Run, "run", 1, 1),
                                  new Token(TokenType.String, "test", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new RunStmt("test")
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