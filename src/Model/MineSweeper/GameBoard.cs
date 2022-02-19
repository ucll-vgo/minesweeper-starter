using Model.Data;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Model.MineSweeper
{
    internal class GameBoard : IGameBoard
    {
        private readonly Grid<Square> _board;

        public const int MinimumSize = 6;

        public const int MaximumSize = 20;

        public GameBoard(int width, int height, int seed, double chanceOfMine) : this(new Grid<Square>(width, height))
        {
            if (width < MinimumSize || width > MaximumSize)
                throw new ArgumentException(nameof(width));

            if (height < MinimumSize || height > MaximumSize)
                throw new ArgumentException(nameof(height));

            Random random = new Random(seed);
            var minefield = new Grid<bool>(width, height, _ => random.NextDouble() < chanceOfMine);
            _board = new Grid<Square>(width, height, position => new Square(minefield[position], MinesNearby(position)));

            int MinesNearby(Vector2D position) => Vector2D.AllDirections
                .Select(d => position + d).Where(minefield.IsValidPosition).Count(p => minefield[p]);
        }

        private GameBoard(Grid<Square> board)
        {
            _board = board;
        }

        public Square this[Vector2D position]
        {
            get => _board[position];
            set => _board[position] = value;
        }

        public GameBoard Copy() => new GameBoard(_board.Copy());

        public int Width => _board.Width;
        public int Height => _board.Height;

        internal int UncoveredEmptySquares => _board.Positions.Count(p => !_board[p].IsCovered && !_board[p].IsMine);
        internal ISet<Vector2D> Mines => _board.Positions.Where(p => _board[p].IsMine).ToHashSet();
        internal ISet<Vector2D> Flags => _board.Positions.Where(p => _board[p].IsFlagged).ToHashSet();

        public void FloodSquares(Vector2D position)
        {
            if (_board[position].AmountOfMinesNear > 0 || _board[position].IsMine)
                return;

            var flooding = new List<Vector2D>();
            var seen = new HashSet<Vector2D>() { position };
            var checklist = new List<Vector2D>() { position };

            while (checklist.Count > 0)
            {
                var current = checklist[0];
                checklist.RemoveAt(0);

                var newNeighbours = Vector2D.AllDirections
                    .Select(d => current + d)
                    .Where(p => _board.IsValidPosition(p) && !seen.Contains(p));

                var cavedNeighbours = newNeighbours
                    .Where(p => !_board[p].IsMine && _board[p].AmountOfMinesNear == 0);

                flooding.AddRange(newNeighbours);
                checklist.AddRange(cavedNeighbours);
                foreach (var nb in newNeighbours) seen.Add(nb);
            }

            foreach (var square in flooding)
                _board[square].Uncover();
        }
    }
}
