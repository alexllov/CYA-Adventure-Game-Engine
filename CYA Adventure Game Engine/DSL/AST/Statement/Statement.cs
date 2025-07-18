﻿using Environment = CYA_Adventure_Game_Engine.DSL.Runtime.Environment;

namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    // =============== Abstract ===============
    /// <summary>
    /// Statements: contain an action to be performed that modifies the environment & game flow.
    /// Does not return a value.
    /// 
    /// Methods:
    /// Interpret(Environment) - details how a statement should be interpreted given the information held in the environment.
    /// </summary>
    public interface IStmt
    {
        /// <summary>
        /// Interpreter method.
        /// </summary>
        /// <param name="state">Environment state object</param>
        /// <exception cref="Exception">Invalid Parameter as Variable Name Exception</exception>
        public abstract void Interpret(Environment state);
    }

}
