using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;

namespace CYA_Adventure_Game_Engine
{
    public interface IParserExtender
    {
        public bool TryParseStmt(Parser parser, out IStmt? stmt);
    }
}


