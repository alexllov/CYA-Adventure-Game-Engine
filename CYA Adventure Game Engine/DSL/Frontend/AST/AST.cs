using CYA_Adventure_Game_Engine.DSL.Frontend.AST.Statement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST
{
    /// <summary>
    /// Abstract Syntax Tree: Contains a list of top-level statements, which can contain other statements, and expressions.
    /// </summary>
    public class AST
    {
        public List<IStmt> Tree = new List<IStmt>();
        public AST(List<IStmt> statements)
        {
            Tree = statements;
        }
    }
}
