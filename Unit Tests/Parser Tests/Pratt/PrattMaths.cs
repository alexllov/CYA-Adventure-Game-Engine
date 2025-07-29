using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class PrattMaths
    {
        // 1+2*3 => 1+(2*3)
        [Fact]
        public void ASTIsCorrectlyConstructedForMaths()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Plus, "+", 1, 1),
                                    new Token(TokenType.Number, "2", 1, 1),
                                    new Token(TokenType.Multiply, "*", 1, 1),
                                    new Token(TokenType.Number, "3", 1, 1)]);

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


        // (1+2)*3 => (1+2)*3
        [Fact]
        public void ASTIsCorrectlyConstructedForMathsInverted()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Plus, "+", 1, 1),
                                    new Token(TokenType.Number, "2", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.Multiply, "*", 1, 1),
                                    new Token(TokenType.Number, "3", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new ExprStmt
                (
                    new BinaryExpr
                    (
                        new BinaryExpr
                        (
                            new NumberLitExpr(1),
                            TokenType.Plus,
                            new NumberLitExpr(2)
                        ),
                        TokenType.Multiply,
                        new NumberLitExpr(3)
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