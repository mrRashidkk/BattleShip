using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueApp.Models;

namespace VueApp
{
    public class MatchManager
    {
        public static List<Match> Matches = new List<Match>();

        public static Match GetMatch(string userName) => 
            Matches.FirstOrDefault(x => x.Players.Any(y => y.Id == userName));
    }
}
