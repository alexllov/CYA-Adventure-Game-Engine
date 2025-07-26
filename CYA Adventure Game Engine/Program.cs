// See https://aka.ms/new-console-template for more information
using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL.AST;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Frontend.Tokenizer;
using CYA_Adventure_Game_Engine.DSL.Runtime;
using CYA_Adventure_Game_Engine.Injection;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;


// Load up the keywords dict, find the local '.cya' game file, & Tokenize.
Dictionary<string, TokenType> keywords = BaseKeywords.Keywords;
var location = AppDomain.CurrentDomain.BaseDirectory;
string address = Directory.GetFiles(location, "./*.cya").FirstOrDefault()
    ?? throw new Exception("No .cya file found in the directory.");
Tokenizer tokenizer = new(address, keywords);
List<Token> tokens = tokenizer.Tokenize();

// Construct the AST from the tokens.
Parser parser = new(tokens);
AbstSyntTree AST = parser.Parse();
AST.Show();

// Find & load modules, create the initial game Env,
// do the first pass of interpretation & then begin game.
Dictionary<string, IModule> modules = ModuleInjection.LoadModules();
var env = new Environment(modules);
// add "debug" for debug mode.
Interpreter interpreter = new(AST, env);
interpreter.Interpret();
interpreter.RunGame();



