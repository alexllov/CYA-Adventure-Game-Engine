// See https://aka.ms/new-console-template for more information
using CYA_Adventure_Game_Engine;
using CYA_Adventure_Game_Engine.DSL;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;


SetupLoader setup = new("./Occult/setup.cya");
Dictionary<string, IModule> state = setup.State;
//foreach (var kvp in state)
//{
//    Console.WriteLine($"{kvp.Key}:\n{kvp.Value}");
//}


// "./Occult/Occult basic.cya"
CYA_Adventure_Game_Engine.DSL.Tokenizer tokenizer = new ("./DSL/Math_Tests.txt");
//tokenizer.Show();

CYA_Adventure_Game_Engine.DSL.Parser parser = new(tokenizer.Tokens);
parser.Show();

//Dictionary<string, Scene> data = parser.Data;
//parser.Show();

//Engine engine = new(data, state);
//engine.Run();


// TEST STUFF
//parser.Show();
//Inventory bag = new ();
//bag.Process("add", ["left shoe", "right shoe","moon rock", "mr whittaker", "pocket watch", "a small cat"]);
//bag.Process("remove", ["left shoe"]);
//Console.Write($"{bag}");

//state["i"].Process("add", ["left shoe", "right shoe", "moon rock", "mr whittaker", "pocket watch", "a small cat"]);
//Console.WriteLine($"Wowzer, im deffo this one: {state["i"]}");

