using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI
{
    public class AddNounStmt : IStmt
    {
        List<IExpr> Nouns;

        public AddNounStmt(List<IExpr> nouns)
        {
            Nouns = nouns;
        }
        public override string ToString()
        {
            return $"AddNounStmt: {string.Join(", ", Nouns)}";
        }

        public void Interpret(Environment state)
        {
            foreach (IExpr nounExpr in Nouns)
            {
                string noun = nounExpr.Interpret(state).ToString();
                if (state.GetNoun(noun, out NounStmt nStmt))
                {
                    state.AddLocalNoun(nStmt);
                }
                else
                {
                    throw new Exception($"AddNounStmt: Noun doesnt exist.");
                }
            }
        }
    }
}
