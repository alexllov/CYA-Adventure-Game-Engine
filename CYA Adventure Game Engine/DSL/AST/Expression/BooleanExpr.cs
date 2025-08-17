using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    internal class BooleanExpr : IExpr
    {
        public bool Value;
        public BooleanExpr(string val)
        {
            Value = bool.Parse(val);
        }
        public override string ToString()
        {
            return $"BooleanExpr({Value})";
        }

        public object Interpret(Environment state) { return Value; }
    }
}
