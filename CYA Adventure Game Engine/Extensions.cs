using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CYA_Adventure_Game_Engine
{
    public static class Extensions
    {
        public static string ToPrettyString<T>(this IEnumerable<T> values)
        {
            return $"[{String.Join(", ", values)}]";
        }

        public static string ToPrettyStringNL<T>(this IEnumerable<T> values)
        {
            return $"[{String.Join("\n", values)}]";
        }
    }
}
