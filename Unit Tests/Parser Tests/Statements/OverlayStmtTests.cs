using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;

namespace Unit_Tests
{
    public class OverlayStmtTests
    {
        //overlay "menu" access "m"
        [Fact]
        public void AccessibleOverlay()
        {
            // Arrange
            TokenList tokens = new([new Token(TokenType.Overlay, "overlay", 1, 1, ""),
                                    new Token(TokenType.String, "menu", 1, 1, ""),
                                    new Token(TokenType.Access, "access", 1, 1, ""),
                                    new Token(TokenType.String, "m", 1, 1, ""),
                                    new Token(TokenType.End, "end", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new OverlayStmt
                (
                    "menu",
                    new BlockStmt([]),
                    "m"
                )
            ]);

            var sut = new Parser(tokens);

            // Act
            AbstSyntTree tree = sut.Parse();

            // Assert
            Assert.Single(tree.Tree);
            Assert.Equivalent(expectedTree, tree);
        }


        [Fact]
        public void InaccessibleOverlay()
        {
            TokenList tokens = new([new Token(TokenType.Overlay, "overlay", 1, 1, ""),
                                    new Token(TokenType.String, "menu", 1, 1, ""),
                                    new Token(TokenType.End, "end", 1, 1, "")]);

            AbstSyntTree expectedTree = new([
                new OverlayStmt
                (
                    "menu",
                    new BlockStmt([])
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