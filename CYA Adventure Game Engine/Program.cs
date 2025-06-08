// See https://aka.ms/new-console-template for more information
using CYA_Adventure_Game_Engine;
//using CYA_Adventure_Game_Engine.Modules;
using System.Collections.Concurrent;
using System.Collections.Generic;


SetupLoader setup = new("./Occult/setup.cya");
Dictionary<string, IModule> state = setup.State;

Tokenizer tokenizer = new ("./Occult/Occult v6.cya");
Parser parser = new(tokenizer);
Dictionary<string, Scene> data = parser.Data;
//parser.Show();

Engine engine = new(data, state);
engine.Run();


// TEST STUFF
//parser.Show();
//Inventory bag = new ();
//bag.Process("add", ["left shoe", "right shoe","moon rock", "mr whittaker", "pocket watch", "a small cat"]);
//bag.Process("remove", ["left shoe"]);
//Console.Write($"{bag}");

//state["i"].Process("add", ["left shoe", "right shoe", "moon rock", "mr whittaker", "pocket watch", "a small cat"]);
//Console.WriteLine($"Wowzer, im deffo this one: {state["i"]}");

