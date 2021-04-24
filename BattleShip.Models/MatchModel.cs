using System.Collections.Generic;
using BattleShip.Common.Enums;

namespace BattleShip.Models
{
    public class MatchModel
    {
        public string Id { get; set; }
        public string Winner { get; set; }
        public string WhoseTurn { get; set; }
        public MatchState State { get; set; }
        public List<PlayerModel> Players { get; set; }
    }
}
