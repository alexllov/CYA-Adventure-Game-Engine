using CYA_Adventure_Game_Engine.DSL.AST.Statement;

namespace CYA_Adventure_Game_Engine.DSL.AST
{
    /// <summary>
    /// Abstract Syntax Tree: Contains a list of top-level statements, which can contain other statements, and expressions.
    /// </summary>
    public class AbstSyntTree : IEnumerable<IStmt>, IEquatable<AbstSyntTree>
    {
        public List<IStmt> Tree = new List<IStmt>();
        public AbstSyntTree(List<IStmt> statements)
        {
            Tree = statements;
        }

        /// <summary>
        /// Debug method.
        /// </summary>
        public void Show()
        {
            Console.WriteLine("AST Statements:");
            foreach (var stmt in Tree)
            {
                Console.WriteLine(stmt);
            }
        }

        public override string ToString()
        {
            List<string> contents = [];
            foreach (var stmt in Tree) { contents.Add(stmt.ToString()); }
            return $"AST: {string.Join(' ', contents)}";
        }

        // Enumerator Logic s.t. Statements can be iterated over.
        public IEnumerator<IStmt> GetEnumerator()
        {
            return Tree.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Tree.GetEnumerator();
        }

        public bool Equals(AbstSyntTree? other)
        {
            if (other is not null
                && ToString().Equals(other.ToString()))
            {
                return true;
            }
            return false;
        }
    }
}
