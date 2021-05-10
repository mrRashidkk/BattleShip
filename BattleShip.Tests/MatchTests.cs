using BattleShip.Common.Enums;
using BattleShip.Entities;
using System;
using Xunit;

namespace BattleShip.Tests
{
    public class MatchTests
    {
        [Fact]
        public void Start_ShouldChangeStateToInProgress()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());

            match.AddPlayer(player1);
            match.AddPlayer(player2);
            match.Start();

            Assert.Equal(MatchState.InProgress, match.State);
        }

        [Fact]
        public void NewMatch_ShouldHaveStateAwaitingOpponent()
        {
            Match match = new Match(Guid.NewGuid().ToString());            

            Assert.Equal(MatchState.AwaitingOpponent, match.State);
        }

        [Fact]
        public void Add_SecondPlayer_ShouldChangeStateToLaunchingShips()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());

            match.AddPlayer(player1);
            match.AddPlayer(player2);

            Assert.Equal(MatchState.LaunchingShips, match.State);
        }

        [Fact]
        public void Add_TheSamePlayerTwice_ShouldFail()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player = new Player(Guid.NewGuid().ToString());            

            match.AddPlayer(player);

            Assert.Throws<GameException>(() => match.AddPlayer(player));
        }

        [Fact]
        public void Add_ThirdPlayer_ShouldFail()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());
            Player player3 = new Player(Guid.NewGuid().ToString());

            match.AddPlayer(player1);
            match.AddPlayer(player2);

            Assert.Throws<GameException>(() => match.AddPlayer(player3));
        }

        [Fact]
        public void Add_PlayerToMatchInProgress_ShouldFail()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());            
            Player player3 = new Player(Guid.NewGuid().ToString());

            match.AddPlayer(player1);
            match.AddPlayer(player2);
            match.Start();

            Assert.Throws<GameException>(() => match.AddPlayer(player3));
        }

        [Fact]
        public void Add_PlayerToFinishedMatch_ShouldFail()
        {
            Match match = new Match(Guid.NewGuid().ToString());
            Player player1 = new Player(Guid.NewGuid().ToString());
            Player player2 = new Player(Guid.NewGuid().ToString());
            Player player3 = new Player(Guid.NewGuid().ToString());

            match.AddPlayer(player1);
            match.AddPlayer(player2);
            match.Start();
            match.Finish(player1.Id);

            Assert.Throws<GameException>(() => match.AddPlayer(player3));
        }
    }
}
