using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
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
