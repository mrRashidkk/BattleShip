using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class Match
    {
        public readonly string Id;
        public List<PlayerInfo> Players { get; } = new List<PlayerInfo>();

        public string WhoseTurn;
        public bool Started;
        public bool GameOver;
        public string Winner;

        public Match(string id)
        {
            Id = id;
        }
        

        public void AddPlayer(PlayerInfo player)
        {
            if (Started)
                throw new GameException("Нельзя присоединиться к начатому матчу");

            if (GameOver)
                throw new GameException("Нельзя присоединиться к завершенному матчу");

            int count = Players.Count;

            switch(count)
            {
                case 0:
                    Players.Add(player);
                    break;
                case 1:
                    if (Players[0].Id == player.Id)
                        throw new GameException("Вы уже присоединились к этому матчу");
                    Players.Add(player);
                    break;
                case 2:
                    throw new GameException("В игре не может быть больше 2 игроков");
            }            
        }
    }
}
