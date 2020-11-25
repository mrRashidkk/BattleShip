using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueApp.Models;

namespace VueApp
{
    public class MatchManager
    {
        private static List<Match> _matches = new List<Match>();

        public static Match GetMatchForPlayer(string playerId) =>
            _matches.FirstOrDefault(x => x.Players.Any(y => y.Id == playerId));

        public static Match GetById(string id) 
        {
            var match = _matches.FirstOrDefault(x => x.Id == id);
            if (match == null)
                throw new GameException($"Матч с ID {id} не найден.");

            return match;
        }

        public static void Add(Match match) => _matches.Add(match);

        public static void Delete(Match match) => _matches.Remove(match);

        public static bool PlayerInAnyMatch(string playerId) =>
            _matches.Any(x => x.Players.Any(y => y.Id == playerId));
    }
}
