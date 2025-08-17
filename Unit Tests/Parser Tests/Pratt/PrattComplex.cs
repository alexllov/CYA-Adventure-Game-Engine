using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class PrattComplex
    {
        /*
         * Sample dot expr & function
         * (inv.add "sword")
         */
        [Fact]
        public void FuncAndDotExpressionTest()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LParent, "(", 1, 1, ""),
                                    new Token(TokenType.Identifier, "inv", 1, 1, ""),
                                    new Token(TokenType.Dot, ".", 1, 1, ""),
                                    new Token(TokenType.Identifier, "add", 1, 1, ""),
                                    new Token(TokenType.String, "sword", 1, 1, ""),
                                    new Token(TokenType.RParent, ")", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new ExprStmt
                (
                    new FuncExpr
                    (
                        new DotExpr
                        (
                            new VariableExpr("inv"),
                            new VariableExpr("add")
                        ),
                        [new StringLitExpr("sword")]
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



        /*
         * Sample dot expr & function
         * (say "string" var 1+3*4 (d.roll "1d6"))
         */
        [Fact]
        public void ComplexSayStmt()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LParent, "(", 1, 1, ""),
                                    new Token(TokenType.Identifier, "say", 1, 1, ""),
                                    new Token(TokenType.String, "string", 1, 1, ""),
                                    new Token(TokenType.Identifier, "var", 1, 1, ""),

                                    new Token(TokenType.Number, "1", 1, 1, ""),
                                    new Token(TokenType.Plus, "+", 1, 1, ""),
                                    new Token(TokenType.Number, "3", 1, 1, ""),
                                    new Token(TokenType.Multiply, "*", 1, 1, ""),
                                    new Token(TokenType.Number, "4", 1, 1, ""),

                                    new Token(TokenType.LParent, "(", 1, 1, ""),
                                    new Token(TokenType.Identifier, "d", 1, 1, ""),
                                    new Token(TokenType.Dot, ".", 1, 1, ""),
                                    new Token(TokenType.Identifier, "roll", 1, 1, ""),
                                    new Token(TokenType.String, "1d6", 1, 1, ""),
                                    new Token(TokenType.RParent, ")", 1, 1, ""),

                                    new Token(TokenType.RParent, ")", 1, 1, "")]);

            //(say "string" var 1+3*4 (d.roll "1d6"))
            AbstSyntTree expectedTree = new([
                new ExprStmt
                (
                    new FuncExpr
                    (
                        new VariableExpr("say"),
                        [
                            new StringLitExpr("string"),
                            new VariableExpr("var"),
                            new BinaryExpr
                            (
                                new NumberLitExpr(1),
                                TokenType.Plus,
                                new BinaryExpr
                                (
                                    new NumberLitExpr(3),
                                    TokenType.Multiply,
                                    new NumberLitExpr(4)
                                )
                            ),
                            new FuncExpr
                            (
                                new DotExpr
                                (
                                    new VariableExpr("d"),
                                    new VariableExpr("roll")
                                ),
                                [new StringLitExpr("1d6")]
                            )
                        ]
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