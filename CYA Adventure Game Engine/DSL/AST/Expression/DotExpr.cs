using System.Reflection;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains an outer object (left) and inner property (right).
    /// </summary>
    public class DotExpr : IExpr
    {
        // TODO: CHANGE THIS S.T U CAN HAVE 1.2.3.4... work properly BCS it wont work rn
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

            // TODO: Need to add a check in here to find if left is an IModule or not.

            var member = type.GetMember(Right.Value,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance
                | BindingFlags.IgnoreCase);

            switch (member)
            {
                case []:
                    throw new Exception($"Failed to find a member of {type} with the name {Right.Value}");
                case [MethodInfo method, ..]:
                    Func<object[], object?> func = (object[] args) =>
                    method.Invoke(left, args);
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
