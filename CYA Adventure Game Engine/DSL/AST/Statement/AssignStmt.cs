using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Handles the assignment of a value to a variable name.
    /// </summary>
    public class AssignStmt : IStmt
    {
        public string Name;
        public IExpr Value;
        public AssignStmt(string name, IExpr value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Debug Method.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return $"AssignStmt(Name: {Name}, Value: {Value})";
        }

        public void Interpret(Environment state)
        {
            object value = Value.Interpret(state);
            state.SetVal(Name, value);
        }
    }
}
