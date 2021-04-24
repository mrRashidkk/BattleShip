using System.Collections.Generic;
using System.Linq;
using BattleShip.Entities;
using BattleShip.Interfaces;
using BattleShip.Models;

namespace BattleShip.Services
{
    public class MatchManager : IMatchManager
    {
        private List<Match> _matches = new List<Match>();

        public Match GetMatchForPlayer(string playerId) =>
            _matches.FirstOrDefault(x => x.Players.Any(y => y.Id == playerId));

        public Match GetById(string id) 
        {
            var match = _matches.FirstOrDefault(x => x.Id == id);
            if (match == null)
                throw new GameException($"Матч с ID {id} не найден.");

            return match;
        }

        public Match Add(string id) 
        {
            Match match = new Match(id);
            _matches.Add(match);
            return match;
        }

        public void Remove(Match match) => _matches.Remove(match);        

        public MatchModel MapToDto(Match match)
        {
            return new MatchModel
            {
                Id = match.Id,
                Winner = match.Winner,
                WhoseTurn = match.WhoseTurn,
                State = match.State,
                Players = match.Players.Select(x => new PlayerModel
                {
                    Id = x.Id,
                    Ready = x.Ready
                }).ToList()
            };
        }
    }
}
