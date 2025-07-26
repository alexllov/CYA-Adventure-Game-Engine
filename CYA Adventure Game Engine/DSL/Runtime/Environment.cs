using CYA_Adventure_Game_Engine.DSL.AST.Statement;
using CYA_Adventure_Game_Engine.DSL.AST.Statement.VOXI;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    public class Environment
    {

        private Dictionary<string, IModule> Modules;

        /// <summary>
        /// Dict stores all assignments crated in interpretation.
        /// Pre-assigned values represent language defaults, e.g. Native Functions.
        /// </summary>
        private Dictionary<string, object> Env = new()
        {
            { "say", NativeFunctions.Say },
            { "ask", NativeFunctions.Ask },
            { "num", NativeFunctions.Num },
        };

        private Dictionary<string, SceneStmt> Scenes = [];

        private Dictionary<string, OverlayStmt> Overlays = [];

        private Dictionary<string, OverlayStmt> AccessibleOverlays = [];

        public List<ChoiceStmt> LocalChoices = [];

        public Dictionary<string, NounStmt> Nouns = [];

        public Dictionary<string, NounStmt> LocalNouns = [];

        private string Command = new("");

        public List<string> CommandErrors = [];

        private List<IStmt> SuccessfulCommands = [];

        private string GoTo = new("");

        private bool GoToFlag = false;

        private (bool, string) RunOverlayFlag = (false, "");

        private bool OverlayExitFlag = false;

        public Environment(Dictionary<string, IModule> modules)
        {
            Modules = modules;
        }


        public IModule GetModule(string name)
        {
            if (!Modules.TryGetValue(name.ToLower(), out IModule? value)) { throw new Exception($"Error, couldn't load module, {name}. Name not found."); }
            return value;
        }

        /// <summary>
        /// Set a value to a given alias.
        /// </summary>
        /// <param name="name">string variable alias.</param>
        /// <param name="value">object value.</param>
        public void SetVal(string name, object value)
        {
            Env[name] = value;
        }

        /// <summary>
        /// Retreive assigned value from alias.
        /// </summary>
        /// <param name="name">variable alias.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public object GetVal(string name)
        {
            if (!(Env.ContainsKey(name))) { throw new Exception($"Error, tried to use non-assigned variable, {name}"); }
            return Env[name];
        }

        public void SetScene(string name, SceneStmt value)
        {
            if (Scenes.ContainsKey(name)) { throw new Exception($"Error, a Scene with the name {name} has already been declared."); }
            Scenes[name] = value;
        }

        public SceneStmt GetScene(string name)
        {
            if (Scenes.TryGetValue(name, out SceneStmt? scene)) { return scene; }
            else { throw new Exception($"Error, requested a scene that does not exist: {name}"); }
        }

        public void SetOverlay(string name, OverlayStmt value)
        {
            Overlays[name] = value;
            if (value.KeyBind is not null) { AccessibleOverlays[value.KeyBind] = value; }
        }

        public bool CheckAccessibleOverlay(string input, out OverlayStmt? overlay)
        {
            return AccessibleOverlays.TryGetValue(input, out overlay);
        }

        // TODO: This should be expanded to clear local commands now too.
        // DONE: Need to check if working properly.
        public void ClearLocal()
        {
            LocalChoices = [];
            LocalNouns = [];
        }

        public void AddLocalChoice(ChoiceStmt interactable)
        {
            LocalChoices.Add(interactable);
        }

        public void AddLocalChoice(params ChoiceStmt[] interactables)
        {
            foreach (ChoiceStmt interactable in interactables)
            {
                LocalChoices.Add(interactable);
            }
        }

        public ChoiceStmt GetLocalChoice(int i)
        {
            if (LocalChoices.Count() < i) { throw new Exception("List index out of range."); }
            else { return LocalChoices[i]; }
        }

        public bool HasLocalChoice(int i)
        {
            if (i != 0 && LocalChoices.Count() >= i) { return true; }
            else { return false; }
        }

        public void AddNoun(NounStmt noun)
        {
            Nouns[noun.Noun] = noun;
        }

        public bool GetNoun(string noun, out NounStmt? value)
        {
            if (Nouns.TryGetValue(noun, out value))
            {
                return true;
            }
            return false;
        }

        // Single.
        public void AddLocalNoun(NounStmt noun)
        {
            LocalNouns[noun.Noun] = noun;
        }

        // Multiple overload for Overlay reset.
        public void AddLocalNoun(Dictionary<string, NounStmt> nouns)
        {
            LocalNouns = nouns;
        }

        public Dictionary<string, NounStmt> GetLocalNouns()
        {
            return LocalNouns;
        }

        // Check for presence of a noun & get it back if so.
        public bool HasLocalNoun(string noun, out NounStmt? value)
        {
            if (LocalNouns.TryGetValue(noun, out value)) { return true; }
            else { return false; }
        }

        public void SetCommand(string command)
        {
            Command = command;
        }
        public string GetCommand()
        {
            return Command;
        }

        public void AddCommandError(string error)
        {
            CommandErrors.Add(error);
        }

        public List<string> GetCommandErrors()
        {
            List<string> errors = CommandErrors;
            CommandErrors = [];
            return errors;
        }

        public void AddSuccessfulCommand(IStmt stmt)
        {
            SuccessfulCommands.Add(stmt);
        }

        public List<IStmt> GetSuccessfulCommands()
        {
            List<IStmt> stack = SuccessfulCommands;
            SuccessfulCommands = [];
            return stack;
        }

        public void SetGoTo(string address)
        {
            GoTo = address;
            GoToFlag = true;
        }

        public string GetGoTo()
        {
            GoToFlag = false;
            return GoTo;
        }

        public bool CheckGoToFlag()
        {
            return GoToFlag;
        }

        public void SetRunOverlayFlag(string name)
        {
            RunOverlayFlag = (true, name);
        }

        public bool CheckRunOverlayFlag(out OverlayStmt? overlay)
        {
            if (Overlays.TryGetValue(RunOverlayFlag.Item2, out overlay)
                && RunOverlayFlag.Item1)
            {
                RunOverlayFlag = (false, "");
                return true;
            }
            else { return false; }
        }

        public void SetOverlayExitFlag(bool value)
        {
            OverlayExitFlag = value;
        }

        public bool CheckOverlayExitFlag()
        {
            if (OverlayExitFlag)
            {
                OverlayExitFlag = false;
                return true;
            }
            return false;
        }
    }
}
