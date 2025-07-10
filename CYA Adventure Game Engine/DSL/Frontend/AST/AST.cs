using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL.Frontend.AST
{
    public class AST
    {
        public List<Stmt> Tree = new List<Stmt>();
        public AST(List<Stmt> statements)
        {
            Tree = statements;
        }
    }
}
