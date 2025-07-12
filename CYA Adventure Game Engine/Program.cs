// See https://aka.ms/new-console-template for more information
using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL.Frontend;
using CYA_Adventure_Game_Engine.DSL.Runtime;
using CYA_Adventure_Game_Engine.DSL.AST;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;


// "./Occult/Occult basic.cya"
Tokenizer tokenizer = new("./DSL/Scene_Tests.txt");
List<Token> tokens = tokenizer.Tokenize();

Parser parser = new(tokens);
AbstSyntTree AST = parser.Parse();
//AST.Show();

//Console.WriteLine("Entering Interpreter.");
var env = new Environment();
// add "debug" for debug mode.
Interpreter interpreter = new(AST, env);
interpreter.Interpret();
interpreter.RunGame();



