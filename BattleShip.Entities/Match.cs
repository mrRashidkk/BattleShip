using BattleShip.Common.Enums;
using BattleShip.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip.Entities
{
    public class Match
    {
        private readonly List<Player> _players = new List<Player>();
        public readonly string Id;        
        public IReadOnlyCollection<Player> Players => _players;
        public string WhoseTurn { get; private set; }
        public string Winner { get; private set; }
        public MatchState State { get; private set; }

        public Match(string id)
        {
            Id = id;
            State = MatchState.AwaitingOpponent;
        }

        public void Start()
        {
            State = MatchState.InProgress;
            SetWhoseTurn();
        }

        private void SetWhoseTurn()
        {
            int index = new Random().Next(2);
            WhoseTurn = _players[index].Id;
        }

        public void Finish(string winnerId)
        {
            State = MatchState.Finished;
            Winner = winnerId;
        }

        public void SwitchTurn()
        {
            WhoseTurn = _players.First(x => !x.Id.IsEqual(WhoseTurn)).Id;
        }

        public void RemovePlayer(string playerId) 
        {
            _players.RemoveAll(x => x.Id.IsEqual(playerId));
            State = MatchState.OnePlayerDisconnected;
        }

        public void AddPlayer(Player player)
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
                    State = MatchState.LaunchingShips;
                    break;
                case 2:
                    throw new GameException("В игре не может быть больше 2 игроков");
            }            
        }
    }
}
