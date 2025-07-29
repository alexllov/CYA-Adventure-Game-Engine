using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// GoTo: Controls Flow between scenes.
    /// </summary>
    public class GoToStmt : IStmt
    {
        public IExpr Location;

        public GoToStmt(IExpr loc)
        {
            Location = loc;
        }
        public override string ToString()
        {
            return $"GoToStmt({Location})";
        }

        public void Interpret(Environment state)
        {
            string loc = (string)Location.Interpret(state);
            state.SetGoTo(loc);
        }

        /// <summary>
        /// Creates GoTo statement.
        /// </summary>
        /// <returns>GoToStmt</returns>
        /// <exception cref="Exception"></exception>
        public static GoToStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "goto statement";
            // Consume "GoTo" token.
            parser.Tokens.Consume(TokenType.GoTo);
            IExpr loc = parser.ParseExpression(0);
            // Allowing for VariableExprs allows for -> aliasing for reusable overlays & scenes that -> different locations.
            if (loc is not StringLitExpr && loc is not VariableExpr)
            {
                throw new Exception($"Unexpected Expr type following GoTo ('START' or '->'). Expected String Literal or variable, received {loc} of type {loc.GetType()}.");
            }
            return new GoToStmt(loc);
        }
    }
}
