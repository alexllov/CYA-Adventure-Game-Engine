using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains Choice object.
    /// </summary>
    public class ChoiceStmt : IStmt
    {
        public IExpr Name;
        public IStmt Body;
        public ChoiceStmt(IExpr name, IStmt body)
        {
            Name = name;
            Body = body;
        }
        public override string ToString()
        {
            return $"ChoiceStmt(Name: {Name}, Body: {Body})";
        }

        public void Interpret(Environment state)
        {
            state.AddLocalChoice(this);
        }

        /// <summary>
        /// Creates Choice statement.
        /// </summary>
        /// <returns>InteractableStmt</returns>
        public static ChoiceStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "choice statement";
            // Consume the '['
            parser.Tokens.Advance();
            IExpr name = parser.ParseExpression(0);
            IStmt body = BlockStmt.Parse(parser, [TokenType.RBracket]);
            parser.Tokens.Consume(TokenType.RBracket);
            return new ChoiceStmt(name, body);
        }
    }
}
