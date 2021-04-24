using System.Collections.Generic;
using System.Linq;
using BattleShip.Common.Helpers;
using BattleShip.Entities;
using BattleShip.Interfaces;
using BattleShip.Models;

namespace BattleShip.Services
{
    public class MatchManager : IMatchManager
    {
        private readonly List<Match> _matches = new List<Match>();

        public int Count => _matches.Count;

        public Match GetMatchForPlayer(string playerId) =>
            _matches.FirstOrDefault(x => x.Players.Any(y => y.Id.IsEqual(playerId)));

        public Match GetById(string id) 
        {
            var match = _matches
                .FirstOrDefault(x => x.Id.IsEqual(id));
            if (match == null)
                throw new GameException($"Матч с ID {id} не найден.");

            return match;
        }

        public void Add(Match match) => _matches.Add(match);

        public void Remove(string id) => _matches.RemoveAll(x => x.Id.IsEqual(id));        

        public MatchModel MapToModel(Match match)
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
