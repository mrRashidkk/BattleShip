using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueApp.Models;

namespace VueApp
{
    public static class PlayerManager
    {
        private static List<PlayerInfo> _players = new List<PlayerInfo>();

        public static void Add(string id)
        {
            if (_players.Any(x => x.Id == id))
                throw new ArgumentException($"Player with ID {id} already exists.");

            _players.Add(new PlayerInfo(id));
        }

        public static PlayerInfo Get(string id)
        {
            return _players.FirstOrDefault(x => x.Id == id);
        }

        public static void Delete(PlayerInfo player) => _players.Remove(player);
    }
}
