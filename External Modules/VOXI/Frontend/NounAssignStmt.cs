using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace External_Modules.VOXI.Frontend
{
    public class NounAssignStmt : IStmt
    {
        string Name;
        NounExpr Noun;

        public NounAssignStmt(string name, NounExpr value)
        {
            Name = name;
            Noun = value;
        }

        /// <summary>
        /// Debug Method.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"NounAssignStmt(Name: {Name}, Noun: {Noun})";
        }

        // Build the actual NounObject & save to variable in Env
        public void Interpret(Environment state)
        {
            NounObject noun = (NounObject)Noun.Interpret(state);
            state.SetVal(Name, noun);
        }

        /// <summary>
        /// Unwraps AssignExpr into AssignStmt.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static NounAssignStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "noun assign statement";
            // Consume 'noun'
            parser.Tokens.Advance();
            string name = parser.Tokens.Peek(0).Lexeme;
            parser.Tokens.Consume(TokenType.Identifier);
            parser.Tokens.Consume(TokenType.Assign);
            NounExpr value = NounExpr.Parse(parser);

            return new NounAssignStmt(name, value);
        }
    }
}
