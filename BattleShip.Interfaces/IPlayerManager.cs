using BattleShip.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip.Interfaces
{
    public interface IPlayerManager
    {
        int Count { get; }
        void Add(Player player);
        Player Get(string id);
        void Remove(string id);        
    }
}
