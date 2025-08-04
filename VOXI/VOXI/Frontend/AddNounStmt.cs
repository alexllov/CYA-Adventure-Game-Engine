using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using CYA_Adventure_Game_Engine.Modules;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace VOXI.Frontend
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
            VOXI voxi = (VOXI)state.Modules["voxi"];
            foreach (IExpr nounExpr in Nouns)
            {
                object noun = nounExpr.Interpret(state);
                if (noun is Inventory inv)
                {
                    inv.All().ForEach(i =>
                    {
                        if (state.TryGetVal(i, out object value)
                            && value is NounObject nObj)
                        {
                            voxi.Env.AddLocalNoun(nObj);
                        }
                    });
                }
                else
                {
                    if (noun is NounObject nObj)
                    {
                        voxi.Env.AddLocalNoun(nObj);
                    }
                    else
                    {
                        throw new Exception($"AddNounStmt: Noun doesnt exist.");
                    }
                }
            }
        }

        public static AddNounStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "add noun statement";
            parser.Tokens.Consume(TokenType.LCurly);
            List<IExpr> nouns = [];
            while (!parser.Tokens.Match(TokenType.RCurly))
            {
                nouns.Add(parser.ParseExpression(0));
            }
            return new AddNounStmt(nouns);
        }
    }
}
