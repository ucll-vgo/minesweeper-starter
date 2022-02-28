using Model.Data;
using System;
using System.Collections.Generic;

namespace Model.MineSweeper
{
    /// <summary>
    /// Represents the different statuses a game can have.
    /// </summary>
    public enum GameStatus
    {
        InProgress,
        Won,
        Lost
    }

    /// <summary>
    /// Represents a game of Mine Sweeper.
    /// Objects of this type are immutable.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Static factory function to create new games.
        /// </summary>
        /// <param name="boardSize">Size of the board. See <see cref="MinimumBoardSize"/> and <see cref="MaximumBoardSize"/> for the allowed range.</param>
        /// <param name="flooding">Whether or not flooding is enabled.</param>
        /// <param name="seed">The seed for randomly generating mines. Entering null will generate a random seed.</param>
        /// <returns></returns>
        public static IGame CreateRandom( int boardSize, double mineProbability, bool flooding = true, int? seed = null )
        {
            var board = GameBoard.CreateRandom( boardSize, boardSize, seed ?? new Random().Next(), mineProbability );

            return new InProgressGame( board, flooding );
        }

        public static IGame Parse( IEnumerable<string> rows, bool flooding = true )
        {
            var board = GameBoard.Parse( rows );

            return new InProgressGame( board, flooding );
        }

        /// <summary>
        /// Game board.
        /// </summary>
        IGameBoard Board { get; }

        /// <summary>
        /// Returns the game's status. A game is either in progress, won or lost.
        /// </summary>
        GameStatus Status { get; }

        /// <summary>
        /// Uncovers a square of the board.
        /// </summary>
        /// <param name="position">Where to uncover a square.</param>
        /// <returns>A new game object with the updated board.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        IGame UncoverSquare( Vector2D position );

        /// <summary>
        /// Sets or removes a flag from a square of the board like a Toggle.
        /// </summary>
        /// <param name="position">Where to set or remove a flag.</param>
        /// <returns>A new game object with the updated board.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        IGame ToggleFlag( Vector2D position );

        /// <summary>
        /// Checks if square on <paramref name="position" /> is covered.
        /// </summary>
        /// <param name="position">The position of the square.</param>
        /// <returns>True if the square is covered at <paramref name="position"/>, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        bool IsSquareCovered( Vector2D position );

        /// <summary>
        /// Gets the adjacent mines of a square on <paramref name="position" /> when it's uncovered.
        /// </summary>
        /// <param name="position">The position of the square.</param>
        /// <returns>If the square is uncovered at <paramref name="position"/>, returns the amount of adjacent mines.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the square is covered is over.</exception>
        int GetAdjacentMines( Vector2D position );

        /// <summary>
        /// Returns the positions that make up the locations of the mines.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the game is not yet over.</exception>
        ISet<Vector2D> Mines { get; }

        /// <summary>
        /// Returns the positions that make up the locations of set flags.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        ISet<Vector2D> Flags { get; }

        /// <summary>
        /// Minimum board size.
        /// </summary>
        public const int MinimumBoardSize = GameBoard.MinimumSize;

        /// <summary>
        /// Maximum board size.
        /// </summary>
        public const int MaximumBoardSize = GameBoard.MaximumSize;
    }

    /// <summary>
    /// Represents a game board.
    /// </summary>
    public interface IGameBoard
    {
        /// <summary>
        /// Width of the board.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the board.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Returns the square at <paramref name="position"/>.
        /// Usage: <code>board[pos]</code>.
        /// </summary>
        /// <param name="position">Position of the square to return.</param>
        /// <returns>square at <paramref name="position"/></returns>
        Square this[Vector2D position] { get; }
    }

    internal abstract class Game : IGame
    {
        protected Game( GameBoard gameBoard )
        {
            Board = gameBoard;
        }

        IGameBoard IGame.Board => Board;

        public GameBoard Board { get; }

        public abstract GameStatus Status { get; }

        public abstract IGame UncoverSquare( Vector2D position );

        public abstract IGame ToggleFlag( Vector2D position );

        public abstract ISet<Vector2D> Mines { get; }

        public ISet<Vector2D> Flags => Board.Flags;

        public bool IsSquareCovered( Vector2D position ) => Board[position].IsCovered;

        public int GetAdjacentMines( Vector2D position )
        {
            if ( IsSquareCovered( position ) )
                throw new InvalidOperationException( "Square is still covered" );

            return Board[position].NeighboringMineCount;
        }
    }

    internal class InProgressGame : Game
    {
        private bool isFloodingEnabled;

        public InProgressGame( GameBoard gameBoard, bool flooding ) : base( gameBoard )
        {
            isFloodingEnabled = flooding;
        }

        public override GameStatus Status => GameStatus.InProgress;

        public override IGame UncoverSquare( Vector2D position )
        {
            if ( !IsSquareCovered( position ) )
            {
                throw new InvalidOperationException( "Square already uncovered" );
            }

            var nextBoard = Board.Copy();
            var square = nextBoard[position];

            square.Uncover();

            if ( square.ContainsMine )
            {
                return new LostGame( nextBoard );
            }

            if ( isFloodingEnabled )
            {
                nextBoard.FloodSquares( position );
            }

            if ( nextBoard.AreAllMineFreeSquaresUncovered )
            {
                return new WonGame( nextBoard );
            }
            else
            {
                return new InProgressGame( nextBoard, isFloodingEnabled );
            }
        }

        public override IGame ToggleFlag( Vector2D position )
        {
            if ( !IsSquareCovered( position ) )
            {
                throw new InvalidOperationException( "Square already uncovered" );
            }

            var nextBoard = Board.Copy();
            nextBoard[position].ToggleFlag();
            return new InProgressGame( nextBoard, isFloodingEnabled );
        }

        public override ISet<Vector2D> Mines => throw new InvalidOperationException( "Game is not over yet" );
    }

    internal abstract class FinishedGame : Game
    {
        public FinishedGame( GameBoard gameBoard ) : base( gameBoard )
        {
            // NOP
        }

        public override ISet<Vector2D> Mines => Board.Mines;

        public override IGame UncoverSquare( Vector2D position ) => throw new InvalidOperationException( "Game finished" );

        public override IGame ToggleFlag( Vector2D position ) => throw new InvalidOperationException( "Game finished" );
    }

    internal class WonGame : FinishedGame
    {
        public WonGame( GameBoard gameBoard ) : base( gameBoard )
        {
            // NOP
        }

        public override GameStatus Status => GameStatus.Won;
    }

    internal class LostGame : FinishedGame
    {
        public LostGame( GameBoard gameBoard ) : base( gameBoard )
        {
            // NOP
        }

        public override GameStatus Status => GameStatus.Lost;
    }
}
