using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class IfStmtTest
    {
        // [if 1==1 then (say "hello world")]
        [Fact]
        public void IfStmtNoElse()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.LBracket, "[", 1, 1),
                                    new Token(TokenType.If, "if", 1, 1),
                                    new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Equal, "==", 1, 1),
                                    new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Then, "then", 1, 1),
                                    new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Identifier, "say", 1, 1),
                                    new Token(TokenType.String, "hello world", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.RBracket, "]", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new IfStmt
                (
                    new BinaryExpr(new NumberLitExpr(1), TokenType.Equal, new NumberLitExpr(1)),
                    new ExprStmt(new FuncExpr(new VariableExpr("say"), [new StringLitExpr("hello world")]))
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }


        // [if 1==1 then (say "hello world") else (say "fail")]
        [Fact]
        public void IfStmtWithElse()
        {

            TokenList tokens = new([new Token(TokenType.LBracket, "[", 1, 1),
                                    new Token(TokenType.If, "if", 1, 1),
                                    new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Equal, "==", 1, 1),
                                    new Token(TokenType.Number, "1", 1, 1),
                                    new Token(TokenType.Then, "then", 1, 1),
                                    new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Identifier, "say", 1, 1),
                                    new Token(TokenType.String, "hello world", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.Else, "else", 1, 1),
                                    new Token(TokenType.LParent, "(", 1, 1),
                                    new Token(TokenType.Identifier, "say", 1, 1),
                                    new Token(TokenType.String, "fail", 1, 1),
                                    new Token(TokenType.RParent, ")", 1, 1),
                                    new Token(TokenType.RBracket, "]", 1, 1)]);

            AbstSyntTree expectedTree = new([
                new IfStmt
                (
                    new BinaryExpr(new NumberLitExpr(1), TokenType.Equal, new NumberLitExpr(1)),
                    new ExprStmt(new FuncExpr(new VariableExpr("say"), [new StringLitExpr("hello world")])),
                    new ExprStmt(new FuncExpr(new VariableExpr("say"), [new StringLitExpr("fail")]))
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