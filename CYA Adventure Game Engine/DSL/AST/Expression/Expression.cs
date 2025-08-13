using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace CYA_Adventure_Game_Engine.DSL.AST.Expression
{
    // Expression: evaluates to a value.
    /// <summary>
    /// Expression: Contains a code-piece that can be evaluated to a value.
    /// Returns this value back out to a statement.
    /// 
    /// Methods:
    /// Interpret(Environment) - details how an expression should be evaluated given the information held in the environment.
    /// </summary>
    public interface IExpr
    {
        /// <summary>
        /// Interpreter method.
        /// </summary>
        /// <param name="state">Environment state object</param>
        /// <exception cref="Exception">Invalid Parameter as Variable Name Exception</exception>
        public abstract object Interpret(Environment state);
    }
}
