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

        public static GameBoard Parse(IEnumerable<string> rows)
        {
            var parsed = rows.Select( ParseRow ).ToList();
            var width = parsed[0].Count;
            var height = parsed.Count;
            var mines = new Grid<bool>( width, height, p => parsed[p.Y][p.X] );
            var board = mines.Map( ( p, b ) => new Square( b, CountNeighboringMines( p ) ) );

            return new GameBoard( board );
                

            IList<bool> ParseRow( string row ) => row.Select( c => c == '*' ).ToList();

            int CountNeighboringMines( Vector2D position ) => mines.Around(position).Count( p => mines[p] );
        }

        public static GameBoard CreateRandom(int width, int height, int seed, double chanceOfMine)
        {
            if (width < MinimumSize || width > MaximumSize)
                throw new ArgumentException(nameof(width));

            if (height < MinimumSize || height > MaximumSize)
                throw new ArgumentException(nameof(height));

            var random = new Random(seed);
            var minefield = new Grid<bool>(width, height, _ => random.NextDouble() < chanceOfMine);
            var board = new Grid<Square>(width, height, position => new Square(minefield[position], CountNeighboringMines(position)));

            return new GameBoard( board );


            int CountNeighboringMines( Vector2D position ) => minefield.Around( position ).Count( p => minefield[p] );
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

        internal bool AreAllBomblessSquaresUncovered => _board.Items.All(square => square.IsMine || !square.IsCovered );

        internal ISet<Vector2D> Mines => _board.Positions.Where(p => _board[p].IsMine).ToHashSet();

        internal ISet<Vector2D> Flags => _board.Positions.Where(p => _board[p].IsFlagged).ToHashSet();

        internal void FloodSquares(Vector2D position)
        {
            if ( _board[position].NeighboringMineCount == 0 )
            {
                foreach ( var p in _board.Around(position) )
                {
                    var square = _board[p];

                    if ( square.IsCovered )
                    {
                        _board[p].Uncover();
                        FloodSquares( p );
                    }
                }
            }
        }
    }
}
