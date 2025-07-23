using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend;

namespace Unit_Tests
{
    public class CommandStmtTests
    {
        /*
         * {"door":
         *      "open" [if (i.check "rod") then -> "outside home" else (say "You should probably grab your rod first!")],
         *      "examine" (say "Door's oak. There's a few nicks in it from fishhooks and whatnot. Mary's boy fixed it up for you a while back... That'd be about 20 years ago now.")
         * }
         */
        [Fact]
        public void AnnotationTest()
        {
            // Arrange
            List<Token> tokens = [new Token(TokenType.LCurly, "{", 1, 1),
                                  // Noun
                                  new Token(TokenType.String, "door", 1, 1),
                                  new Token(TokenType.Colon, ":", 1, 1),

                                  // Verb 1
                                  new Token(TokenType.String, "open", 1, 1),

                                  // Conditional Branch
                                  new Token(TokenType.LBracket, "[", 1, 1),
                                  new Token(TokenType.If, "if", 1, 1),
                                  new Token(TokenType.LParent, "(", 1, 1),
                                  new Token(TokenType.Identifier, "i", 1, 1),
                                  new Token(TokenType.Dot, ".", 1, 1),
                                  new Token(TokenType.Identifier, "check", 1, 1),
                                  new Token(TokenType.String, "rod", 1, 1),
                                  new Token(TokenType.RParent, ")", 1, 1),
                                  // Then.
                                  new Token(TokenType.Then, "then", 1, 1),
                                  new Token(TokenType.GoTo, "->", 1, 1),
                                  new Token(TokenType.String, "outside home", 1, 1),
                                  // Else.
                                  new Token(TokenType.Else, "else", 1, 1),
                                  new Token(TokenType.LParent, "(", 1, 1),
                                  new Token(TokenType.Identifier, "say", 1, 1),
                                  new Token(TokenType.String, "You should probably grab your rod first!", 1, 1),
                                  new Token(TokenType.RParent, ")", 1, 1),
                                  new Token(TokenType.RBracket, "]", 1, 1),
                                  new Token(TokenType.Comma, ",", 1, 1),

                                  // Verb 2
                                  new Token(TokenType.String, "examine", 1, 1),
                                  new Token(TokenType.LParent, "(", 1, 1),
                                  new Token(TokenType.Identifier, "say", 1, 1),
                                  new Token(TokenType.String, "Door's oak. There's a few nicks in it from fishhooks and whatnot. Mary's boy fixed it up for you a while back... That'd be about 20 years ago now.", 1, 1),
                                  new Token(TokenType.RParent, ")", 1, 1),
                                  new Token(TokenType.RCurly, "}", 1, 1)];

            AbstSyntTree expectedTree = new([
                new CommandStmt
                (
                    "door",
                    new Dictionary<string, IStmt>
                    {
                        // Verb 1
                        { "open", new IfStmt
                            (
                                new FuncExpr
                                (
                                    // if (i.check "rod")
                                    new DotExpr
                                    (
                                        new VariableExpr("i"),
                                        new VariableExpr("check")
                                    ),
                                    [new StringLitExpr("rod")]
                                ),
                                // then -> "outside home"
                                new GoToStmt(new StringLitExpr("outside home")),
                                // else (say "You should probably grab your rod first!")
                                new ExprStmt
                                (
                                    new FuncExpr
                                    (
                                        new VariableExpr("say"),
                                        [new StringLitExpr("You should probably grab your rod first!")]
                                    )
                                )
                            )
                        },
                        // Verb2
                        { "examine", new ExprStmt
                            (
                                new FuncExpr
                                (
                                        new VariableExpr("say"),
                                        [new StringLitExpr("Door's oak. There's a few nicks in it from fishhooks and whatnot. Mary's boy fixed it up for you a while back... That'd be about 20 years ago now.")]
                                )
                            )
                        }
    }
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