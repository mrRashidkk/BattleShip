using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip.Common.Helpers;
using BattleShip.Entities;
using BattleShip.Interfaces;

namespace BattleShip.Services
{
    public class PlayerManager : IPlayerManager
    {
        private readonly List<Player> _players = new List<Player>();

        public int Count => _players.Count;

        public void Add(Player player)
        {
            if (_players.Any(x => x.Id.IsEqual(player.Id)))
                throw new ArgumentException($"Игрок с ID {player.Id} уже существует.");

            _players.Add(player);
        }

        public Player Get(string id) => _players.FirstOrDefault(x => x.Id.IsEqual(id));

        public void Remove(string id) => _players.RemoveAll(x => x.Id.IsEqual(id));
    }
}
