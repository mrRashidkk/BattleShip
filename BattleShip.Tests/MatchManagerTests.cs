using BattleShip.Entities;
using BattleShip.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BattleShip.Tests
{
    public class MatchManagerTests
    {
        [Fact]
        public void Remove_OneMatch_ShouldDecreaseNumberOfMatchesByOne()
        {
            MatchManager matchManager = new MatchManager();
            Match match1 = new Match(Guid.NewGuid().ToString());
            Match match2 = new Match(Guid.NewGuid().ToString());
            matchManager.Add(match1);
            matchManager.Add(match2);
            int countBeforeRemove = matchManager.Count;

            matchManager.Remove(match2.Id);

            Assert.Equal(countBeforeRemove - 1, matchManager.Count);
        }
    }
}
