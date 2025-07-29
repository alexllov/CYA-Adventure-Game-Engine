using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// If Statement: An expression is evaluated, controling if a "then" branch is ran or not. Optional 'else' branch.
    /// </summary>
    public class IfStmt : IStmt
    {
        public IExpr Condition;
        public IStmt ThenBranch;
        public IStmt? ElseBranch;

        public IfStmt(IExpr condition, IStmt thenBranch, IStmt? elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }
        public override string ToString()
        {
            return $"IfStmt(Condition: {Condition}, ThenBranch: {ThenBranch}, ElseBranch: {ElseBranch})";
        }

        public void Interpret(Environment state)
        {
            var condition = Condition.Interpret(state);
            if (condition is not bool) { throw new Exception("Error, If statement condition does not evaluate to true or false."); }
            if ((bool)condition)
            {
                ThenBranch.Interpret(state);
            }
            else if (ElseBranch is not null)
            {
                ElseBranch.Interpret(state);
            }
        }

        /// <summary>
        /// Creates an If statement, allowing for conditional branching down a 'then' and optional 'else' branch.
        /// </summary>
        /// <returns>IfStmt</returns>
        public static IfStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "if statement";
            // Consume the '[' & then 'if' token.
            parser.Tokens.Advance();
            parser.Tokens.Advance();
            IExpr condition = parser.ParseExpression(0);
            parser.Tokens.Consume(TokenType.Then);
            IStmt thenBranch = BlockStmt.Parse(parser, [TokenType.Else, TokenType.RBracket]);
            IStmt? elseBranch = null;
            if (parser.Tokens.Match(TokenType.Else))
            {
                elseBranch = BlockStmt.Parse(parser, [TokenType.RBracket]);
            }
            // TODO: Consider moving this into the Bracket Parsing space to avoid duplication.
            // Consume closing bracket.
            parser.Tokens.Consume(TokenType.RBracket);
            return new IfStmt(condition, thenBranch, elseBranch);
        }
    }
}
