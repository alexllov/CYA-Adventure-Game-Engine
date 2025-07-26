using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI
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
            if (!(state.GetCommand() == ""))
            {
                state.AddCommandError($"Error, cannot perform '{Verb} ... {state.GetCommand()}', too many arguments.");
            }
            else
            {
                state.AddSuccessfulCommand(Action);
            }
        }
    }
}
