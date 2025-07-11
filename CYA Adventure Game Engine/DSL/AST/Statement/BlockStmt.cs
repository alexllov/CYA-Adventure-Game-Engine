using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
