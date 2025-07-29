using CYA_Adventure_Game_Engine.DSL.AST.Expression;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser.Pratt;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
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

        /// <summary>
        /// Unwraps AssignExpr into AssignStmt.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static AssignStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "assign statement";
            string name = parser.Tokens.Peek(0).Lexeme;
            parser.Tokens.Consume(TokenType.Identifier);
            parser.Tokens.Consume(TokenType.Assign);
            IExpr value = parser.ParseExpression(0);
            return new AssignStmt(name, value);
        }
    }
}
