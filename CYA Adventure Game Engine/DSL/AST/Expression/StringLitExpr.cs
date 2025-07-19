using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// StringLitExpr: Holds a string literal.
    /// </summary>
    public class StringLitExpr : IExpr
    {
        public string Value;
        public StringLitExpr(string val)
        {
            Value = val;
        }
        public override string ToString()
        {
            // TODO: Set up a DebugToString for parser purposes that contians this return.
            return $"StringLitExpr({Value})";
        }

        public object Interpret(Environment state) { return Value; }
    }
}
