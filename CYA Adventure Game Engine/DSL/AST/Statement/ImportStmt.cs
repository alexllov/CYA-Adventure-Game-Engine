using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;
namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    /// <summary>
    /// Handles the importing of optional modules. Optional aliasing support.
    /// </summary>
    public class ImportStmt : IStmt
    {
        public string Module;
        public string Alias;
        public ImportStmt(string module, string alias = null)
        {
            Module = module;
            Alias = alias ?? module;
        }
        public override string ToString()
        {
            return Alias == null ? $"ImportStmt(Module: {Module})" : $"ImportStmt(Module: {Module}, Alias: {Alias})";
        }

        public void Interpret(Environment state)
        {
            IModule module = state.GetModule(Module);
            state.SetVal(Alias, module);
        }

        /// <summary>
        /// Constructs Stmt for imports. Allows for optional aliasing.
        /// </summary>
        /// <returns>ImportStmt</returns>
        public static ImportStmt Parse(Parser parser)
        {
            parser.CurrentStmtParsing = "import statement";
            // Consume the 'import' token that IDd the stmt.
            parser.Tokens.Consume(TokenType.Import);
            Token module = parser.Tokens.Consume(TokenType.Identifier);
            if (parser.Tokens.Match(TokenType.As))
            {
                Token alias = parser.Tokens.Consume(TokenType.Identifier);
                return new ImportStmt(module.Lexeme, alias.Lexeme);
            }
            return new ImportStmt(module.Lexeme);
        }
    }
}
