using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.AST.Statement
{
    internal class OverlayStmt
    {
        string Name;
        string KeyBind;
        BlockStmt Body;
        public OverlayStmt() { }


    }
}
