using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Contains a list of subsoquent statements, allowing blocks to be placed in single-statement slots.
    /// </summary>
    public class BlockStmt : IStmt
    {
        public List<IStmt> Statements;
        public BlockStmt(List<IStmt> statements)
        {
            Statements = statements;
        }

        /// <summary>
        /// Debug Method.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"BlockStmt(Statements: [\n    {string.Join("\n    ", Statements)}])";
        }

        public void Interpret(Environment state)
        {
            foreach (IStmt stmt in Statements)
            {
                stmt.Interpret(state);
            }
        }

        /// <summary>
        /// Func to compile possible blocks of statements either into a BlockStmt or single Stmt depending on how many.
        /// Example use: IF stmt processing to collect all statements in the 'then' & 'else' branches
        /// Collects statements until it finds the dedicated 'stopping' token.
        /// </summary>
        /// <param name="stoppingPoint">Token types list representing end point for Stmt collection.</param>
        /// <returns>Stmt</returns>
        public static IStmt Parse(Parser parser, TokenType[] stoppingPoint)
        {
            parser.CurrentStmtParsing = "block statement";
            List<IStmt> stmts = [];
            Token token = parser.Tokens.Peek(0);
            while (!stoppingPoint.Contains(parser.Tokens.Peek(0).Type))
            {
                stmts.Add(parser.ParseStmt());
                token = parser.Tokens.Peek(0);
            }
            return stmts.Count switch
            {
                > 1 => new BlockStmt(stmts),
                1 => stmts[0],
                _ => throw new Exception($"Error, a statement body was required and not provided. Last token checked was on line: {token.position[0]}, in file: {token.SourceFile}"),
            };
        }

        /// <summary>
        /// Parse overload, allows for additional end-points to be declared.
        /// Main use: allows "foreign tokens" to be matched on (e.g. type: Identifier, lexeme: xyz)
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IStmt Parse(Parser parser, Func<Token, bool> predicate)
        {
            parser.CurrentStmtParsing = "block statement";
            List<IStmt> stmts = [];
            Token token = parser.Tokens.Peek(0);
            while (!predicate(token))
            {
                stmts.Add(parser.ParseStmt());
                token = parser.Tokens.Peek(0);
            }
            return stmts.Count switch
            {
                > 1 => new BlockStmt(stmts),
                1 => stmts[0],
                _ => throw new Exception($"Error, a statement body was required and not provided. Last token checked was on line: {token.position[0]}, in file: {token.SourceFile}"),
            };
        }
    }
}
