using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class ChoiceStmtTests
    {
        // ["annotation text" (say "annotation selected")]
        [Fact]
        public void AnnotationTest()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LBracket, "[", 1, 1),
                                    new Token(TokenType.String, "annotation text", 1, 1),
                                    new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Identifier, "say", 1, 1),
                                    new Token(TokenType.String, "annotation selected", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.RBracket, "]", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new ChoiceStmt
                (
                    new StringLitExpr("annotation text"),
                    new ExprStmt
                    (
                        new FuncExpr
                        (
                            new VariableExpr("say"),
                            [new StringLitExpr("annotation selected")]
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


        // ["choice text" (say "choice selected") -> "next_scene"]
        [Fact]
        public void ChoiceTest()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LBracket, "[", 1, 1),
                                    new Token(TokenType.String, "choice text", 1, 1),
                                    new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Identifier, "say", 1, 1),
                                    new Token(TokenType.String, "choice selected", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.GoTo, "->", 1, 1),
                                    new Token(TokenType.String, "next_scene", 1, 1),
                                    new Token(TokenType.RBracket, "]", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new ChoiceStmt
                (
                    new StringLitExpr("choice text"),
                    new BlockStmt
                    ([
                        new ExprStmt
                        (
                            new FuncExpr
                            (
                                new VariableExpr("say"),
                                [new StringLitExpr("choice selected")]
                            )
                        ),
                        new GoToStmt(new StringLitExpr("next_scene"))
                    ])
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