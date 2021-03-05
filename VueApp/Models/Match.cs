using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class Match
    {
        public readonly string Id;
        private List<PlayerInfo> _players = new List<PlayerInfo>();
        public IReadOnlyCollection<PlayerInfo> Players => _players;

        public string WhoseTurn;
        public string Winner;
        public MatchState State;

        public Match(string id)
        {
            Id = id;
            State = MatchState.AwaitingOpponent;
        }

        public void SetWhoseTurn()
        {
            int index = new Random().Next(2);
            WhoseTurn = _players[index].Id;
        }
        
        public void RemovePlayer(PlayerInfo player)
        {
            _players.Remove(player);
        }

        public void AddPlayer(PlayerInfo player)
        {
            if (State == MatchState.InProgress)
                throw new GameException("Нельзя присоединиться к начатому матчу");

            if (State == MatchState.Finished)
                throw new GameException("Нельзя присоединиться к завершенному матчу");

            int count = _players.Count;

            switch(count)
            {
                case 0:
                    _players.Add(player);
                    break;
                case 1:
                    if (_players[0].Id == player.Id)
                        throw new GameException("Вы уже присоединились к этому матчу");
                    _players.Add(player);
                    break;
                case 2:
                    throw new GameException("В игре не может быть больше 2 игроков");
            }            
        }
    }
}
