using System.Reflection;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains an outer object (left) and inner property (right).
    /// </summary>
    public class DotExpr : IExpr
    {
        public IExpr Left;
        public VariableExpr Right;
        public DotExpr(IExpr left, VariableExpr right)
        {
            Left = left;
            Right = right;
        }
        public override string ToString()
        {
            return $"DotExpr({Left}, {Right})";
        }

        public object Interpret(Environment state)
        {
            // Left must be interpreted first to find outermost object.
            var left = Left.Interpret(state);
            var type = left.GetType();

            // Flags: must be Public & (Static or Instance) - covers all the publics,
            // IgnoreCase - to allow for case insensitive access.
            var member = type.GetMember(Right.Value,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance
                | BindingFlags.IgnoreCase);

            switch (member)
            {
                case []:
                    throw new Exception($"Failed to find a member of {type} with the name {Right.Value}");
                // Method -> Create a standardised curried func call using Invoke() & the 'left' object.
                // This is used for pattern matching in FuncExpr.
                case [MethodInfo method, ..]:
                    Func<object[], object?> func = (object[] args) => method.Invoke(left, args);
                    return func;
                case [FieldInfo field, ..]:
                    return field.GetValue(left);
                case [PropertyInfo property, ..]:
                    return property.GetValue(left);
                default:
                    throw new Exception($"Unimplemented Member type found for attribute {Right.Value} within {left}.");
            }
        }
    }
}
