using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace VOXI.Frontend
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
            VOXIEnvironment env = voxi.Env;
            if (!(env.GetCommand() == ""))
            {
                env.AddVOXIError($"Error, cannot perform '{Verb} ... {env.GetCommand()}', too many arguments.");
            }
            else
            {
                env.AddSuccessfulVOXICommand(Action);
            }
        }
    }
}
