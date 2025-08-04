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
                //foreach (Expr arg in Arguments)
                //{
                //    Console.WriteLine($"arg: {arg}, type: {arg.GetType()}");
                //}
                return $"FuncExpr(Method: {Method}, Arguments: [{string.Join(", ", Arguments)}])";
            }
            else
            {
                return $"FuncExpr(Method: {Method}";
            }
        }

        public object Interpret(Environment state)
        {
            // TODO: this needs reworign s.t. proper funcs & dot funcs will work & other things can throw appropriate errors.
            var function = Method.Interpret(state);
            List<object> args = [];
            foreach (IExpr arg in Arguments)
            {
                args.Add(arg.Interpret(state));
            }
            if (function is Func<List<object>, object> multArgFunc)
            {
                return multArgFunc(args);
            }
            else if (function is Func<List<object>, object> arglessFunc)
            {
                return arglessFunc(args);
            }
            else if (function is Action<List<object>> action)
            {
                action(args);
                return null;
            }
            else if (function is Action arglessAction)
            {
                arglessAction();
                return null;
            }
            else if (function is Func<object[], object?> DotExprFunc)
            {
                return DotExprFunc([.. args]);
            }
            else { throw new Exception("Function call of unsupported argument type found."); }
        }
    }
}
