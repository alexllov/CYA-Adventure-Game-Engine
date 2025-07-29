using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    public class ExitStmt : IStmt
    {
        public void Interpret(Environment state)
        {
            state.SetOverlayExitFlag(true);
        }

        public override string ToString()
        {
            return "ExitStmt()";
        }

        public static ExitStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "exit statement";
            // Consume the 'exit' token.
            parser.Tokens.Advance();
            return new ExitStmt();
        }
    }
}
