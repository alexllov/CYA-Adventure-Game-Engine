namespace CYA_Adventure_Game_Engine.DSL.Runtime
{

    /// <summary>
    /// Contains Native built-in functions.
    /// </summary>
    public static class NativeFunctions
    {
        /// <summary>
        /// Basic WriteLine func.
        /// </summary>
        /// <param name="args">Args to be presented. Must have a valid string repr.</param>
        /// <returns>null</returns>
        public static object Say(List<object> args)
        {
            Console.WriteLine($"{string.Join("", args)}");
            return null;
        }

        /// <summary>
        /// Used take user input after giving the user an input prompt.
        /// </summary>
        /// <param name="args">Input prompt for the user. Must have a valid string repr.</param>
        /// <returns>user input</returns>
        public static object Ask(List<object> args)
        {
            Console.Write($"{string.Join("", args)}");
            string result = Console.ReadLine();
            return result;
        }

        /// <summary>
        /// Converts a string to number & returns the float. Throws error if unable.
        /// </summary>
        /// <param name="arg"></param>
        public static object Num(object arg)
        {
            if (float.TryParse(arg.ToString(), out float fl)) { return fl; }
            else { throw new Exception($"Error, could not convert arg: {arg} of type: {arg.GetType()} to num."); }
        }

        /// <summary>
        /// Returns bool of if input can be converted to a number.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static object IsNum(object arg)
        {
            if (float.TryParse(arg.ToString(), out float fl)) { return true; }
            return false;
        }

        /// <summary>
        /// Returns bool of if input is an integer.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static object IsInt(object arg)
        {
            if (int.TryParse(arg.ToString(), out int i)) { return true; }
            return false;
        }
    }
}