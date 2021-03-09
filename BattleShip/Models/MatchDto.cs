using System.Collections.Generic;
using BattleShip.Entities;

namespace BattleShip.Models
{
    public class MatchDto
    {
        public string Id { get; set; }
        public string Winner { get; set; }
        public string WhoseTurn { get; set; }
        public MatchState State { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}
