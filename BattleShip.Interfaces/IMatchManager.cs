using BattleShip.Entities;
using BattleShip.Models;

namespace BattleShip.Interfaces
{
    public interface IMatchManager
    {
        int Count { get; }
        Match GetMatchForPlayer(string playerId);
        Match GetById(string id);
        void Add(Match match);
        void Remove(string id);
        MatchModel MapToModel(Match match);
    }
}
