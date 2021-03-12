using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip.Entities;

namespace BattleShip
{
    public static class PlayerManager
    {
        private static List<Player> _players = new List<Player>();
        public static IReadOnlyCollection<Player> Players => _players;

        public static Player Add(string id)
        {
            if (_players.Any(x => x.Id == id))
                throw new ArgumentException($"Player with ID {id} already exists.");

            Player player = new Player(id);
            _players.Add(player);
            return player;
        }

        public static Player Get(string id) =>
            _players.FirstOrDefault(x => x.Id == id);
        

        public static void Delete(Player player) =>
            _players.Remove(player);
    }
}
