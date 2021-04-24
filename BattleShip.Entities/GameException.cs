using System;

namespace BattleShip.Entities
{
    public class GameException : Exception
    {
        public GameException(string message) : base(message)
        {
        }
    }
}
