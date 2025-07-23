using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend;

namespace Unit_Tests
{
    /*
     * Type of assignment allowed:
     * Identifier = Expression
     */
    public class AssignStmtTests
    {
        // string = "test"
        [Fact]
        public void StringLitAssign()
        {
            // Arrange
            List<Token> tokens = [new Token(TokenType.Identifier, "string", 1, 1),
                                  new Token(TokenType.Assign, "=", 1, 1),
                                  new Token(TokenType.String, "test", 1, 1)];

            AbstSyntTree expectedTree = new([
                new AssignStmt
                (
                    "string",
                    new StringLitExpr("test")
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }



        // num = 173
        [Fact]
        public void NumLitAssign()
        {
            // Arrange
            List<Token> tokens = [new Token(TokenType.Identifier, "num", 1, 1),
                                  new Token(TokenType.Assign, "=", 1, 1),
                                  new Token(TokenType.Number, "173", 1, 1)];

            AbstSyntTree expectedTree = new([
                new AssignStmt
                (
                    "num",
                    new NumberLitExpr(173)
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }

        // math = 1+2*3
        [Fact]
        public void MathBinaryExprAssign()
        {
            // Arrange
            List<Token> tokens = [new Token(TokenType.Identifier, "math", 1, 1),
                                  new Token(TokenType.Assign, "=", 1, 1),
                                  new Token(TokenType.Number, "1", 1, 1),
                                  new Token(TokenType.Plus, "+", 1, 1),
                                  new Token(TokenType.Number, "2", 1, 1),
                                  new Token(TokenType.Multiply, "*", 1, 1),
                                  new Token(TokenType.Number, "3", 1, 1)];

            AbstSyntTree expectedTree = new([
                new AssignStmt
                (
                    "math",
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