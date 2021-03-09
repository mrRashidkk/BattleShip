using System.Collections.Generic;
using System.Linq;
using BattleShip.Entities;
using BattleShip.Models;

namespace BattleShip
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

        public static void Remove(Match match) => _matches.Remove(match);        

        public static MatchDto MapToDto(Match match)
        {
            return new MatchDto
            {
                Id = match.Id,
                Winner = match.Winner,
                WhoseTurn = match.WhoseTurn,
                State = match.State,
                Players = match.Players.Select(x => new PlayerDto
                {
                    Id = x.Id,
                    Ready = x.Ready
                }).ToList()
            };
        }
    }
}
