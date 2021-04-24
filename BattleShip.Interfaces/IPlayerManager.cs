using BattleShip.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleShip.Interfaces
{
    public interface IPlayerManager
    {
        Player Add(string id);
        Player Get(string id);
        void Delete(Player player);
    }
}
