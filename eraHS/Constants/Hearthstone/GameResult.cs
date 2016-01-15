using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eraHS.Constants.Hearthstone
{
    class GameResult
    {
        public static readonly Dictionary<string, int> dictionary = new Dictionary<string, int>
        {
            {"LOST", -1},
            {"WON", 1},
            {"TIE", 0}
        };
    }
}
