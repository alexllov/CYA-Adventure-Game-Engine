﻿namespace CYA_Adventure_Game_Engine.DSL.Runtime
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

        public static object Num(List<object> arg)
        {
            if (float.TryParse(arg[0].ToString(), out float fl)) { return fl; }
            else { throw new Exception($"Error, could not convert arg: {arg} of type: {arg.GetType()} to num."); }
        }

        // TODO: Needs work & integrating into Environment.
        public static void Set(Environment env, string name, object value)
        {
            env.SetVal(name, value);
        }
    }
}