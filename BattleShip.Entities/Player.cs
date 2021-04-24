namespace BattleShip.Entities
{
    public class Player
    {
        private Square[][] Board = new Square[10][];
        public readonly string Id;
        public bool Ready;
        public int HP { get; private set; } = 20;              

        public Player(string id)
        {
            Id = id;
            SetEmptyBoard();
        }

        private void SetEmptyBoard()
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
            if (square.Boom)
                throw new GameException("Вы уже стреляли по этой клетке!");

            square.Boom = true;
            if (square.Taken) HP--;
            return square.Taken;
        }

        public void Reset()
        {
            SetEmptyBoard();
            HP = 20;
            Ready = false;
        }        
    }
}
