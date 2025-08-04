using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using VOXI.Frontend;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace VOXI
{
    public class VOXI : IModule, IEnvironmentExtender, IChoiceHandler, IParserExtender
    {
        public VOXIEnvironment Env;

        public VOXI() { }

        //Noun, Verb, Prep, Default
        public List<string> Tokens = ["noun", "verb", "prep", "default"];

        /// <summary>
        /// Parser Extender for custom Stmt types.
        /// </summary>
        /// <returns>bool of if parsing was successful, out the Stmt parsed if true</returns>
        public bool TryParseStmt(Parser parser, out IStmt? stmt)
        {
            stmt = null;
            switch (parser.Tokens.Peek(0), parser.Tokens.Peek(1))
            {
                case ({ Type: TokenType.Identifier, Lexeme: "noun" }, { Type: TokenType.Identifier }):
                    stmt = NounAssignStmt.Parse(parser);
                    return true;
                case ({ Type: TokenType.LCurly }, _):
                    stmt = AddNounStmt.Parse(parser);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates VOXIEnv wrapper for environment so that VOXI can track its own relevant state info,
        /// e.g. LocalNouns for command handling.
        /// </summary>
        public void CreateEnvironmentWrapper(Environment environment)
        {
            Env = new VOXIEnvironment(environment);
        }

        /// <summary>
        /// Modifies the input propmt displayed to the user
        /// </summary>
        public string GetUserFacingText(string current)
        {
            return (Env, current) switch
            {
                ({ LocalNouns.Count: 0 }, "") => "",
                ({ LocalNouns.Count: 0 }, not "") => current + "",
                ({ LocalNouns.Count: > 0 }, "") => "Enter your command",
                ({ LocalNouns.Count: > 0 }, not "") => current + " or command",
                ({ LocalNouns: _ }, _) => throw new Exception("How did we end up here"),
            };
        }

        /// <summary>
        /// Returns if VOXI should be considered as a handler for the latest command.
        /// </summary>
        public bool IsActive()
        {
            if (Env.LocalNouns.Count > 0) { return true; }
            return false;
        }

        public void HandleCommand(string choice)
        {
            CommandHandler.Handle(Env, choice);
        }

        public void ClearLocal()
        {
            Env.LocalNouns = [];
        }

        public void StoreLocal()
        {
            Env.LocalBackup = Env.LocalNouns;
        }

        public void DumpLocal()
        {
            Env.LocalNouns = Env.LocalBackup;
        }
    }
}
