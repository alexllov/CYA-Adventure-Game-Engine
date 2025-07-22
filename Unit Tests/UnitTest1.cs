using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend;

namespace Unit_Tests
{
    public class UnitTest1
    {
        [Fact]
        public void BasicFactTest()
        {
            Assert.True(true);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void BasicTheoryTest(int value)
        {
            Assert.IsType<int>(value);
        }

        // 1+2*3 => 1+(2*3)
        [Fact]
        public void ASTIsCorrectlyConstructedForMaths()
        {
            // Arrange
            List<Token> tokens = [new Token(TokenType.Number, "1", 1, 1),
                                  new Token(TokenType.Plus, "+", 1, 1),
                                  new Token(TokenType.Number, "2", 1, 1),
                                  new Token(TokenType.Multiply, "*", 1, 1),
                                  new Token(TokenType.Number, "3", 1, 1)];

            AbstSyntTree expectedTree = new([
                new ExprStmt
                (
                    new BinaryExpr
                    (
                        new NumberLitExpr(1),
                        TokenType.Plus,
                        new BinaryExpr
                        (
                            new NumberLitExpr(2),
                            TokenType.Multiply,
                            new NumberLitExpr(3)
                        )
                    )
                )
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