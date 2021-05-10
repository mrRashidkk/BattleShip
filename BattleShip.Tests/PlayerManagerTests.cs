using BattleShip.Entities;
using BattleShip.Services;
using System;
using Xunit;

namespace BattleShip.Tests
{
    public class PlayerManagerTests
    {
        [Fact]
        public void Add_TheSamePlayerTwice_ShouldFail()
        {
            PlayerManager playerManager = new PlayerManager();
            Player player = new Player(Guid.NewGuid().ToString());

            playerManager.Add(player);

            Assert.Throws<ArgumentException>(() => playerManager.Add(player));
        }

        [Fact]
        public void Remove_OnePlayer_ShouldDecrementNumberOfPlayers()
        {
            PlayerManager playerManager = new PlayerManager();
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());
            playerManager.Add(player1);
            playerManager.Add(player2);
            int countBeforeRemove = playerManager.Count;

            playerManager.Remove(player2.Id);

            Assert.Equal(countBeforeRemove - 1, playerManager.Count);
        }
    }
}
