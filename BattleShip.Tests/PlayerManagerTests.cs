using BattleShip.Entities;
using System;
using System.Linq;
using Xunit;

namespace BattleShip.Tests
{
    public class PlayerManagerTests
    {
        [Fact]
        public void Add_TheSamePlayerShouldFail()
        {
            string id = Guid.NewGuid().ToString();
            
            PlayerManager.Add(id);

            Assert.Throws<ArgumentException>(() => PlayerManager.Add(id));
        }

        [Fact]
        public void Delete_ShouldDecreaseNumberOfPlayersByOne()
        {
            PlayerManager.Add(Guid.NewGuid().ToString());
            Player player = PlayerManager.Add(Guid.NewGuid().ToString());
            PlayerManager.Delete(player);

            Assert.Equal(1, PlayerManager.Players.Count);
        }
    }
}
