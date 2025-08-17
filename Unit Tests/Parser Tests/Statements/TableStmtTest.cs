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
    public class TableStmtTest
    {
        // table test
        // | "test" | "value"       |
        // | "this" | (say "pass")  |
        [Fact]
        public void TableConstruction()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Table, "table", 1, 1, ""),
                                    new Token(TokenType.Identifier, "test", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),
                                    new Token(TokenType.String, "test", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),
                                    new Token(TokenType.String, "value", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),
                                    new Token(TokenType.String, "this", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),
                                    new Token(TokenType.LParent, "(", 1, 1, ""),
                                    new Token(TokenType.Identifier, "say", 1, 1, ""),
                                    new Token(TokenType.String, "pass", 1, 1, ""),
                                    new Token(TokenType.RParent, ")", 1, 1, ""),
                                    new Token(TokenType.Pipe, "|", 1, 1, ""),]);

            AbstSyntTree expectedTree = new([
                new TableStmt
                (
                    // name.
                    new StringLitExpr("test"),
                    // records.
                    [
                        [new StringLitExpr("test"), new StringLitExpr("value")],
                        [new StringLitExpr("this"), new FuncExpr(new VariableExpr("say"), [new StringLitExpr("pass")])]
                    ]
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