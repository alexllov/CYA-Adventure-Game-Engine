// See https://aka.ms/new-console-template for more information
using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Runtime;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;


//SetupLoader setup = new("./Occult/setup.cya");
//Dictionary<string, IModule> state = setup.State;

// "./Occult/Occult basic.cya"
Tokenizer tokenizer = new("./DSL/Scene_Tests.txt");
tokenizer.Tokenize();

Parser parser = new(tokenizer.Tokens);
parser.Parse();
//parser.Show();

//Console.WriteLine("Entering Interpreter.");
var env = new Environment();
// add "debug" for debug mode.
Interpreter interpreter = new(parser.AST, env);
interpreter.Interpret();
interpreter.RunGame();



