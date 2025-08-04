using CYA_Adventure_Game_Engine.DSL.AST.Statement;

namespace VOXI.Frontend
{
    public interface IVerb : IStmt
    {
        string Verb { get; set; }
    }
}
