using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip.Entities;
using BattleShip.Interfaces;

namespace BattleShip.Services
{
    public class PlayerManager : IPlayerManager
    {
        private readonly List<Player> _players = new List<Player>();

        public Player Add(string id)
        {
            if (_players.Any(x => x.Id == id))
                throw new ArgumentException($"Игрок с ID {id} уже существует.");

            Player player = new Player(id);
            _players.Add(player);
            return player;
        }

        public Player Get(string id) =>
            _players.FirstOrDefault(x => x.Id == id);
        

        public void Delete(Player player) =>
            _players.Remove(player);
    }
}
