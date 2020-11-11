using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class PlayerInfo
    {
        public readonly string Name;
        private Square[,] Board = new Square[10,10];
        public int HP { get; set; }
        public bool Ready { get; set; }

        public PlayerInfo(string userName)
        {
            Name = userName;
            CreateBoard();
        }

        private void CreateBoard()
        {
            for(int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Board[row , col] = new Square();
                }
            }
        }

        public bool GetFire(int row, int col)
        {
            var square = Board[row,col];
            square.Boom = true;
            return square.Taken;
        }
    }
}
