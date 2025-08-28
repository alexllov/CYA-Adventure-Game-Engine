using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    /*
     * Type of assignment allowed:
     * Identifier = Expression
     */
    public class SceneStmtTest
    {
        // scene "test"
        // "string 1"
        // ["choice 1" -> "next"]

        [Fact]
        public void SceneConstruction()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Scene, "scene", 1, 1, ""),
                                    new Token(TokenType.String, "test", 1, 1, ""),
                                    new Token(TokenType.String, "string 1", 1, 1, ""),
                                    new Token(TokenType.LBracket, "[", 1, 1, ""),
                                    new Token(TokenType.String, "choice 1", 1, 1, ""),
                                    new Token(TokenType.GoTo, "->", 1, 1, ""),
                                    new Token(TokenType.String, "next", 1, 1, ""),
                                    new Token(TokenType.RBracket, "]", 1, 1, ""),]);

            AbstSyntTree expectedTree = new([
                new SceneStmt
                (
                    // name.
                    "test",
                    // records.
                    new BlockStmt(
                        [
                            new ExprStmt( new FuncExpr(new VariableExpr("say"), [new StringLitExpr("string 1")])),
                            new ChoiceStmt(new StringLitExpr("choice 1"), new GoToStmt(new StringLitExpr("next")))
                        ]
                    )
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equal(expectedTree.ToString(), tree.ToString());
        }
    }
}