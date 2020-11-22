using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class Match
    {
        private List<PlayerInfo> players = new List<PlayerInfo>();

        public readonly string Id;
        public string WhoseTurn { get; set; }
        
        public bool Started { get; set; }
        public bool GameOver { get; set; }
        public string Winner { get; set; }

        public Match(string id)
        {
            Id = id;
        }

        public List<PlayerInfo> Players { get => players; }

        public void AddPlayer(PlayerInfo player)
        {
            if (Started)
                throw new Exception("Cannot add player to started match");

            if (GameOver)
                throw new Exception("Cannot add player to finished match");

            int count = players.Count;

            switch(count)
            {
                case 0:
                    players.Add(player);
                    break;
                case 1:
                    if (players[0].Id == player.Id)
                        throw new ArgumentException("Cannot add the same player twice");
                    players.Add(player);
                    break;
                case 2:
                    throw new Exception("Only 2 players can play!");
            }            
        }
    }
}
