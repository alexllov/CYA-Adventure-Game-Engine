namespace CYA_Adventure_Game_Engine
{
    public interface IChoiceHandler
    {
        // Process a user input.
        public void HandleCommand(string choice);

        // Modify user prompt s.t. they know they can use this interaction type here.
        public string GetUserFacingText(string current);

        // Clear local scope.
        public void ClearLocal();
    }
}


