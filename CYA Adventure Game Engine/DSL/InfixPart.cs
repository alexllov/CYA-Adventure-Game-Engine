using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    internal interface InfixPart
    {
        Expr Parse(Parser parser, Expr left, Token token);
    }
}
