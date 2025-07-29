using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains Scene object.
    /// </summary>
    public class SceneStmt : IStmt
    {
        public string Name;
        public BlockStmt Body;
        public SceneStmt(string name, BlockStmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString()
        {
            return $"SceneStmt(\n  Name: {Name}, \n  Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.SetScene(Name, this);
        }

        /// <summary>
        /// Creates a Scene statement.
        /// </summary>
        /// <returns>SceneStmt</returns>
        public static SceneStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "scene statement";
            // Consume the 'scene' token.
            parser.Tokens.Advance();
            // Very next Token should be string with ID for scene.
            Token ID = parser.Tokens.Consume(TokenType.String);
            List<IStmt> parts = [];
            while (!parser.HeaderEnds.Contains(parser.Tokens.Peek(0).Type))
            {
                // Scenes have special sugar for strings,
                // & can contain special components: interactables.
                // So we will filter for those.
                switch (parser.Tokens.Peek(0).Type)
                {
                    case TokenType.String:
                        FuncExpr say = new(new VariableExpr("say"), [new StringLitExpr(parser.Tokens.Peek(0).Lexeme)]);
                        ExprStmt sayStmt = new(say);
                        parts.Add(sayStmt);
                        // Advance needed as Stmt hand made, so string part isn't being consumed.
                        parser.Tokens.Advance();
                        break;

                    // Default -> ParseStmt using recursive calls to process.
                    default:
                        IStmt stmt = parser.ParseStmt();
                        parts.Add(stmt);
                        break;
                }
            }
            // Consume the End Token if found.
            parser.Tokens.Match(TokenType.End);

            // Convert List parts to BlockStmt.
            BlockStmt body = new(parts);
            return new SceneStmt(ID.Lexeme, body);
        }
    }
}
