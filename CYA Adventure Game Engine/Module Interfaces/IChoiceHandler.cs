namespace CYA_Adventure_Game_Engine
{
    public interface IChoiceHandler
    {
        // Process a user input.
        // Current: successfuls only added after all errors -> early return,
        // however this means that needs to be properly implemented by every module writer.
        public void HandleCommand(string choice);

        // Modify user prompt s.t. they know they can use this interaction type here.
        public string GetUserFacingText(string current);

        // Returns true if this choicer should be evaluated in scene input reading for the current scene.
        public bool IsActive();

        // Clear local scope.
        public void ClearLocal();

        // Move Local to a backup s.t. it can be cleared without being lost.
        public void StoreLocal();

        // Re-load Local from the backup
        public void DumpLocal();
    }
}


