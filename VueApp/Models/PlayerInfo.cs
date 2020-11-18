using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class PlayerInfo
    {
        public readonly string Id;
        private Square[][] Board = new Square[10][];
        public int HP { get; private set; } = 20;
        public bool Ready { get; set; }

        public PlayerInfo(string clientId)
        {
            Id = clientId;
            CreateBoard();
        }

        private void CreateBoard()
        {
            for(int row = 0; row < 10; row++)
            {
                Square[] squaresRow = new Square[10];
                for (int col = 0; col < 10; col++)
                {
                    squaresRow[col] = new Square();
                }
                Board[row] = squaresRow;
            }
        }

        public void SetBoard(Square[][] board)
        {
            Board = board;
        }

        public bool GetFire(int row, int col)
        {
            var square = Board[row][col];
            square.Boom = true;
            if (square.Taken) HP--;
            return square.Taken;
        }
    }
}
