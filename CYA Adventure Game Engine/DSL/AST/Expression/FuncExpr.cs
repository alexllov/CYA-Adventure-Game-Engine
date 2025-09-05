using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    /// <summary>
    /// Contains a function call:
    /// Method - the function being called upon.
    /// Arguments - the inputs to the function.
    /// </summary>
    public class FuncExpr : IExpr
    {
        public IExpr Method;
        public List<IExpr> Arguments;
        public FuncExpr(IExpr method, List<IExpr> arguments = null)
        {
            Method = method;
            Arguments = arguments ?? [];
        }
        public override string ToString()
        {
            if (Arguments != null)
            {
                return $"FuncExpr(Method: {Method}, Arguments: [{string.Join(", ", Arguments)}])";
            }
            else
            {
                return $"FuncExpr(Method: {Method}";
            }
        }

        public object Interpret(Environment state)
        {
            var function = Method.Interpret(state);
            List<object> args = [];

            foreach (IExpr arg in Arguments)
            {
                args.Add(arg.Interpret(state));
            }

            switch (function)
            {
                case Func<object, object> singleArgFunc:
                    if (args.Count > 1) { throw new Exception("Error, too many arguments passed in function call."); }
                    return singleArgFunc(args[0]);

                case Func<List<object>, object> multArgFunc:
                    return multArgFunc(args);

                case Action<List<object>> action:
                    action(args);
                    return null;

                case Action arglessAction:
                    arglessAction();
                    return null;

                // Curried function from DotExpr.
                case Func<object[], object?> DotExprFunc:
                    return DotExprFunc([.. args]);

                default:
                    throw new Exception("Function call of unsupported argument type found.");
            }
        }
    }
}
