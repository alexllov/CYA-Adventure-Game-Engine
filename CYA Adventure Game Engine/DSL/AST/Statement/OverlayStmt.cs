using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    public class OverlayStmt : IStmt
    {
        public string Name;
        public string? KeyBind;
        public BlockStmt Body;
        public OverlayStmt(string name, BlockStmt body, string? key = null)
        {
            Name = name;
            Body = body;
            KeyBind = key;
        }
        public override string ToString()
        {
            return $"OverlayStmt(\n  Name: {Name},\n  KeyBind: {KeyBind},\n  Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.SetOverlay(Name, this);
        }

        /// <summary>
        /// Creates an OverlayStmt.
        /// Overlay's act like scenes but are entered on-top, without leaving the current scene.
        /// Example: a game menu that you want to be accessible from any scene.
        /// </summary>
        /// <returns></returns>
        public static OverlayStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "overlay statement";
            //Consume overlay
            parser.Tokens.Advance();
            // Very next Token should be string with ID for scene.
            Token ID = parser.Tokens.Consume(TokenType.String);
            bool Accessible = false;
            string AccessString = "";
            if (parser.Tokens.Match(TokenType.Access))
            {
                Accessible = true;
                parser.Tokens.Advance();
                if (parser.Tokens.Peek(0).Type is TokenType.String)
                { AccessString = parser.Tokens.Peek(0).Lexeme; }
                parser.Tokens.Advance();
            }
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
            if (parser.Tokens.Peek(0).Type is TokenType.End)
            {
                parser.Tokens.Consume(TokenType.End);
            }

            // Convert List parts to BlockStmt.
            BlockStmt body = new(parts);
            if (Accessible) { return new OverlayStmt(ID.Lexeme, body, AccessString); }
            else { return new OverlayStmt(ID.Lexeme, body); }
        }
    }
}
