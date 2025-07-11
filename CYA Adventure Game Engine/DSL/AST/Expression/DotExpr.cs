using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            //throw new Exception("Not yet implemented Dot Exprs.");
            var left = Left.Interpret(state);
            var type = left.GetType();

            // TODO: build if stmt here to go through field, property & method(?)
            var field = type.GetField(Right.Value);
            if (field != null) 
            {
                Console.WriteLine($"Field found: {Right.Value}");
                var fieldItem = field.GetValue(left);
                return fieldItem;
            }

            var property = type.GetProperty(Right.Value);
            if (property != null)
            {
                Console.WriteLine($"Property found: {Right.Value}");
                var propertyItem = property.GetValue(left);
                return propertyItem;
            }

            var method = type.GetMethod(Right.Value);
            if (method != null)
            {
                Console.WriteLine($"Method found: {Right.Value}");
                return method;//.Invoke;
            }

            throw new Exception($"Error, requested right hand of dot expression {Right.Value}, not found as a field, property, or method within the class: {left}");
        }
    }
}
