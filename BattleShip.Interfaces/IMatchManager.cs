using BattleShip.Entities;
using BattleShip.Models;

namespace BattleShip.Interfaces
{
    public interface IMatchManager
    {
        Match GetMatchForPlayer(string playerId);
        Match GetById(string id);
        Match Add(string id);
        void Remove(Match match);
        MatchModel MapToDto(Match match);
    }
}
