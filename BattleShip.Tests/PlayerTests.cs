using BattleShip.Entities;
using System;
using Xunit;

namespace BattleShip.Tests
{
    public class PlayerTests
    {
        [Fact]
        public void GetFire_TheSameSquareTwice_ShouldFail()
        {
            Player player = new Player(Guid.NewGuid().ToString());

            player.GetFire(0, 0);

            Assert.Throws<GameException>(() => player.GetFire(0, 0));
        }

        [Fact]
        public void GetFire_ToTakenSquare_ShouldDecrementHP()
        {
            Player player = new Player(Guid.NewGuid().ToString());
            var board = GenerateEmptyBoard();
            board[0][0].Taken = true;
            player.SetBoard(board);
            int initialHP = player.HP;

            player.GetFire(0, 0);

            Assert.Equal(initialHP - 1, player.HP);
        }        

        private static Square[][] GenerateEmptyBoard()
        {
            Square[][] board = new Square[10][];
            for (int row = 0; row < 10; row++)
            {
                Square[] squaresRow = new Square[10];
                for (int col = 0; col < 10; col++)
                {
                    squaresRow[col] = new Square();
                }
                board[row] = squaresRow;
            }
            return board;
        }
    }
}
