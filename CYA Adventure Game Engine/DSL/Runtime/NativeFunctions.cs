﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return Console.ReadLine();
        }
    }
}