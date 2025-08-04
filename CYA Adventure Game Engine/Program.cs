using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.Frontend.Parser;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using CYA_Adventure_Game_Engine.DSL.Runtime;
using CYA_Adventure_Game_Engine.Injection;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

// Find & load modules, create the initial game Env,
// do the first pass of interpretation & then begin game.
Dictionary<string, IModule> modules = ModuleInjection.LoadModules();

// Create initial game env & Add modules to it.
var env = new Environment(modules);
List<IParserExtender> ParserExtenders = [];
foreach (var module in modules)
{
    if (module.Value is IEnvironmentExtender ee)
    {
        ee.CreateEnvironmentWrapper(env);
    }
    if (module.Value is IParserExtender pe) { ParserExtenders.Add(pe); }
}



// Load up the keywords dict, find the local '.cya' game file, & Tokenize.
Dictionary<string, TokenType> keywords = BaseKeywords.Keywords;
var location = AppDomain.CurrentDomain.BaseDirectory;
string address = Directory.GetFiles(location, "./*.cya").FirstOrDefault()
    ?? throw new Exception("No .cya file found in the directory.");
Tokenizer tokenizer = new(address, keywords);
TokenList tokens = tokenizer.Tokenize();

// Construct the AST from the tokens.
Parser parser = new(tokens, ParserExtenders);
AbstSyntTree AST = parser.Parse();
//AST.Show();



// add "debug" for debug mode.
Interpreter interpreter = new(AST, env);
// First pass for top-level setup.
interpreter.Interpret();
// Actual game engine.
interpreter.RunGame();



