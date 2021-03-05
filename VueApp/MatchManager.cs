using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueApp.Models;

namespace VueApp
{
    public static class MatchManager
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

        public static Match Add(string id) 
        {
            Match match = new Match(id);
            _matches.Add(match);
            return match;
        }

        public static void Delete(Match match) => _matches.Remove(match);

        public static bool PlayerInAnyMatch(string playerId) =>
            _matches.Any(x => x.Players.Any(y => y.Id == playerId));

        public static MatchDto MapToDto(Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                GameOver = match.GameOver,
                Winner = match.Winner,
                WhoseTurn = match.WhoseTurn,
                Started = match.Started,
                Players = match.Players.Select(x => new PlayerDto
                {
                    Id = x.Id,
                    Connected = x.Connected,
                    Ready = x.Ready
                }).ToList()
            };
        }
    }
}
