using CYA_Adventure_Game_Engine.DSL.AST.Statement;

namespace CYA_Adventure_Game_Engine.DSL.Runtime
{
    public class Environment
    {

        public readonly Dictionary<string, IModule> Modules;

        public List<IChoiceHandler> ChoiceHandlers =>
            [.. Modules
             .Values
             .Select(m => m as IChoiceHandler)
             .Where(m => m is not null)];

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

        // TODO: needs implementing.
        public void Save()
        {
            Console.Write("Enter Save Name: ");
            string saveName = Console.ReadLine();
            Console.WriteLine($"Saved as {saveName}");
            Console.WriteLine("---------->>>>><<<<<----------");
            SaveObject save = new(this);
            return;
        }

        // TODO: needs implementing.
        private void Load()
        {

        }

        private Dictionary<string, SceneStmt> Scenes = [];

        private Dictionary<string, OverlayStmt> Overlays = [];

        public Dictionary<string, OverlayStmt> AccessibleOverlays = [];

        public List<ChoiceStmt> LocalChoices = [];

        // Errors from IChoiceHandlers
        public List<string> CommandErrors = [];

        // Statements correctly identified to be ran from IChoiceHandlers
        private List<IStmt> SuccessfulCommands = [];

        private string GoTo = new("");

        private bool GoToFlag = false;

        private (bool, string) RunOverlayFlag = (false, "");

        private bool OverlayExitFlag = false;

        public Environment(Dictionary<string, IModule> modules)
        {
            Modules = modules;
            //SetVal("save", Save);
            //SetVal("load", Load);
        }

        /// <summary>
        /// Used by Import statements to bring modules into the game's variable environment.
        /// </summary>
        /// <returns>requiested module</returns>
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

        /// <summary>
        /// Attempts to find a variable.
        /// </summary>
        public bool TryGetVal(string name, out object target)
        {
            if (Env.TryGetValue(name, out target)) { return true; }
            return false;
        }

        /// <summary>
        /// Adds new scene to Scenes dictionary.
        /// Checks that name hasn't been used already.
        /// </summary>
        public void SetScene(string name, SceneStmt value)
        {
            if (Scenes.ContainsKey(name)) { throw new Exception($"Error, a Scene with the name {name} has already been declared."); }
            Scenes[name] = value;
        }

        /// <summary>
        /// Returns requested scene from Scenes if valid.
        /// </summary>
        public SceneStmt GetScene(string name)
        {
            if (Scenes.TryGetValue(name, out SceneStmt? scene)) { return scene; }
            else { throw new Exception($"Error, requested a scene that does not exist: {name}"); }
        }

        /// <summary>
        /// Adds Overlay to the Overlays dict.
        /// If it has a keybind, adds it to the AccessibleOverlays too.
        /// </summary>
        public void SetOverlay(string name, OverlayStmt value)
        {
            Overlays[name] = value;
            if (value.KeyBind is not null) { AccessibleOverlays[value.KeyBind] = value; }
        }

        /// <summary>
        /// Checks if string matches an access keybind for an accessible overlay.
        /// Returns the overlay if match found.
        /// </summary>
        public bool CheckAccessibleOverlay(string input, out OverlayStmt? overlay)
        {
            return AccessibleOverlays.TryGetValue(input, out overlay);
        }

        /// <summary>
        /// Clears the LocalChoices from Env.
        /// Calls on all IChoiceHandlers to clear their local environments too in order to reset.
        /// </summary>
        public void ClearLocal()
        {
            LocalChoices = [];
            foreach (IChoiceHandler choicer in ChoiceHandlers)
            {
                choicer.ClearLocal();
            }
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

        public bool CheckSuccessfulCommands()
        {
            if (SuccessfulCommands.Count > 0) { return true; }
            return false;
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
