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
                throw new GameException("Нельзя присоединиться к начатому матчу");

            if (GameOver)
                throw new GameException("Нельзя присоединиться к завершенному матчу");

            int count = players.Count;

            switch(count)
            {
                case 0:
                    players.Add(player);
                    break;
                case 1:
                    if (players[0].Id == player.Id)
                        throw new GameException("Вы уже присоединились к этому матчу");
                    players.Add(player);
                    break;
                case 2:
                    throw new GameException("В игре не может быть больше 2 игроков");
            }            
        }
    }
}
