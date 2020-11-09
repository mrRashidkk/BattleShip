using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueApp.Models
{
    public class Square
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool Taken { get; set; }
        public bool Boom { get; set; }

        public Square(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
