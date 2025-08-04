using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace External_Modules.VOXI.Frontend
{
    public class TransitiveVerbStmt : IVerb
    {
        public string Verb { get; set; }
        public IStmt Action;

        public TransitiveVerbStmt(string verb, IStmt action)
        {
            Verb = verb;
            Action = action;
        }
        public override string ToString()
        {
            return $"TransitiveVerb({Verb}: {Action.ToString()})";
        }

        public void Interpret(Environment state)
        {
            VOXI voxi = (VOXI)state.Modules["voxi"];
            if (!(voxi.Env.GetCommand() == ""))
            {
                state.AddCommandError($"Error, cannot perform '{Verb} ... {voxi.Env.GetCommand()}', too many arguments.");
            }
            else
            {
                state.AddSuccessfulCommand(Action);
            }
        }
    }
}
