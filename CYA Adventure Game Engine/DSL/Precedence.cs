using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine.DSL
{
    public class Precedence
    {
        public const int ASSIGNMENT = 1;
        public const int OR = 2;
        public const int AND = 3;
        public const int CONDITIONAL = 4;
        public const int SUM = 5;
        public const int PRODUCT = 6;
        public const int EXPONENT = 7;
        public const int PREFIX = 8;
        public const int POSTFIX = 9;
        public const int PARENT = 10;
        public const int DOT = 11;
    }
}

