#if false
namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    public class CommandParser
    {
        List<string> Tokens;
        Environment State;

        string Verb = "";
        string DirectNoun = "";
        string Preposition = "";
        string IndirectNoun = "";

        public CommandParser(string choice, Environment state)
        {
            Tokens = [.. choice.ToLower().Split(' ')];
            State = state;
            Parse();
        }

        private void Parse()
        {
            Console.WriteLine($"Parsing command: {string.Join(' ', Tokens)}");

            // Get Verb.
            if (!TryGetVerb())
            {
                Console.WriteLine($"Error: Unrecognised verb: {Tokens[0]}.");
                return;
            }

            if (TryNotTooManyPrepositions())
            {
                Console.WriteLine("Error: Too many prepositions in command.");
                return;
            }



        }

        private bool TryGetVerb()
        {
            if (State.CheckVerbs(Tokens[0], out string? verb))
            {
                Verb = verb!;
                return true;
            }
            return false;
        }

        private bool TryNotTooManyPrepositions()
        {
            // Check for Preposision.
            List<string> intersectPrep = [.. Tokens[1..].Intersect(State.GetPrepositions())];
            switch (intersectPrep.Count)
            {
                case 0:
                    return true;
                case 1:
                    Preposition = intersectPrep[0];
                    return true;
                default:
                    return false;
            }
        }
    }
}




// Check for Preposision.
List<string> intersectPrep = [.. Tokens[1..].Intersect(State.GetPrepositions())];
string directNoun;
string indirectNoun;
string prep;
bool tooManyPrepositions = false;
switch (intersectPrep.Count)
{
    case 0:
        // No preposition.
        directNoun = string.Join(' ', Tokens[1..]);
        indirectNoun = "";
        prep = "";
        Console.WriteLine($"Direct Noun: {directNoun}, no prep");
        break;
    case 1:
        // One preposition.
        prep = intersectPrep[0];
        string input = string.Join(" ", Tokens[1..]);
        List<string> leftRight = [.. input.Split(intersectPrep[0])];
        directNoun = leftRight[0];
        indirectNoun = leftRight[1];
        Console.WriteLine($"Direct Noun: {directNoun}, Indirect Noun: {indirectNoun}, Preposition: {prep}");
        break;
    default:
        tooManyPrepositions = true;
        Console.WriteLine("Error: Too many prepositions in command.");
        break;
#endif