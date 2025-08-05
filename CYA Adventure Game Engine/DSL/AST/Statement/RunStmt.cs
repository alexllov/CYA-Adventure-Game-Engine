using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    public class RunStmt : IStmt
    {
        public string Overlay;
        public RunStmt(string overlay)
        {
            Overlay = overlay;
        }
        public void Interpret(Environment state)
        {
            state.SetRunOverlayFlag(Overlay);
        }
        public override string ToString()
        {
            return $"runStmt({Overlay})";
        }

        public static RunStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "run statement";
            // Consume the 'run' token.
            parser.Tokens.Advance();
            // Next token should be string with ID for code to run.
            Token ID = parser.Tokens.Consume(TokenType.String);
            return new RunStmt(ID.Lexeme);
        }
    }
}
