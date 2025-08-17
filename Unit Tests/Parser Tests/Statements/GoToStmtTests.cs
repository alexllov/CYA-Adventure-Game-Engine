using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{

    public class GoToStmtTest
    {
        // -> "test"
        [Fact]
        public void StringLitGoTo()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.GoTo, "->", 1, 1, ""),
                                  new Token(TokenType.String, "test", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new GoToStmt
                (
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


        // -> Variable
        [Fact]
        public void VariableExprGoTo()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.GoTo, "->", 1, 1, ""),
                                  new Token(TokenType.Identifier, "variable", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new GoToStmt
                (
                    new VariableExpr("variable")
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }

        // START "scene"
        [Fact]
        public void StartGoTo()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.GoTo, "START", 1, 1, ""),
                                  new Token(TokenType.Identifier, "scene", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new GoToStmt
                (
                    new VariableExpr("scene")
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