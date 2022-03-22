using Model.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Model.MineSweeper
{
    internal class GameBoard : IGameBoard
    {
        private readonly Grid<Square> _board;

        public const int MinimumSize = 5;

        public const int MaximumSize = 20;

        public static GameBoard Parse(IEnumerable<string> rows)
        {
            if ( rows == null )
            {
                throw new ArgumentNullException( nameof( rows ) );
            }

            var parsed = rows.Select( ParseRow ).ToList();

            if ( parsed.Count == 0 )
            {
                throw new ArgumentException( "At least one row is expected", nameof( rows ) );
            }
            else if ( !parsed.All(row => row.Count == parsed[0].Count) )
            {
                throw new ArgumentException( "Rows should have the same width", nameof( rows ) );
            }

            var width = parsed[0].Count;
            var height = parsed.Count;

            if ( !IsValidWidth(width) )
            {
                throw new ArgumentException( "Invalid width", nameof( rows ) );
            }
            else if ( !IsValidHeight(height))
            {
                throw new ArgumentException( "Invalid height", nameof( rows ) );
            }
            
            var mines = new Grid<bool>( width, height, p => parsed[p.Y][p.X] );

            return CreateGameBoardFromMines( mines );
                

            IList<bool> ParseRow( string row ) => row.Select( c => c == '*' ).ToList();
        }

        public static GameBoard CreateRandom( int width, int height, int seed, double mineProbability )
        {
            if ( !IsValidWidth(width) )
            {
                throw new ArgumentException( $"width should be between {MinimumSize} and {MaximumSize}", nameof( width ) );
            }
            else if ( !IsValidHeight(height) )
            {
                throw new ArgumentException( $"height should be between {MinimumSize} and {MaximumSize}", nameof( height ) );
            }
            else if ( mineProbability < 0 || mineProbability > 1 )
            {
                throw new ArgumentException( $"mineProbability should be between 0 and 1", nameof( mineProbability ) );
            }

            var random = new Random( seed );
            var mines = new Grid<bool>( width, height, _ => random.NextDouble() < mineProbability );

            return CreateGameBoardFromMines( mines );
        }

        private static bool IsValidWidth( int width ) => MinimumSize <= width && width <= MaximumSize;

        private static bool IsValidHeight( int height ) => MinimumSize <= height && height <= MaximumSize;

        private static GameBoard CreateGameBoardFromMines(Grid<bool> mines)
        {
            var squareGrid = mines.Map( ( p, b ) => new Square( b, CountNeighboringMines( p ) ) );
            return new GameBoard( squareGrid );

            int CountNeighboringMines( Vector2D position ) => mines.Around( position ).Count( p => mines[p] );
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

        public GameBoard Copy() => new GameBoard(_board.Copy(s => new Square(s)));

        public int Width => _board.Width;

        public int Height => _board.Height;

        internal bool AreAllMineFreeSquaresUncovered => _board.Items.All(square => square.ContainsMine || !square.IsCovered );

        internal ISet<Vector2D> Mines => _board.Positions.Where(p => _board[p].ContainsMine).ToHashSet();

        internal ISet<Vector2D> Flags => _board.Positions.Where(p => _board[p].IsFlagged).ToHashSet();

        internal void FloodSquares(Vector2D position)
        {
            if ( _board[position].NeighboringMineCount == 0 && !_board[position].IsFlagged )
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

        public override string ToString()
        {
            return string.Join( "\n", _board.Rows.Select( row => string.Join( "", row.Items.Select( square => square.ToString() ) ) ) );
        }
    }
}
