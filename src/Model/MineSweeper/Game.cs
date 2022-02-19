using Model.Data;
using System;
using System.Collections.Generic;

namespace Model.MineSweeper
{
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
        public static IGame Create(int boardSize, bool flooding = true, int? seed = null)
        {
            if (!seed.HasValue)
            {
                var temp = new Random();
                seed = temp.Next(int.MinValue, int.MaxValue);
            }

            return new InProgressGame(boardSize, flooding, seed.Value);
        }

        /// <summary>
        /// Game board.
        /// </summary>
        IGameBoard Board { get; }

        /// <summary>
        /// Returns true if the game is over, false otherwise.
        /// </summary>
        bool IsGameOver { get; }

        /// <summary>
        /// If the game is over and a mine was hit, returns the position of the mine. Returns null otherwise.
        /// </summary>
        Vector2D MineHit { get; }

        /// <summary>
        /// Uncovers a square of the board.
        /// </summary>
        /// <param name="position">Where to uncover a square.</param>
        /// <returns>A new game object with the updated board.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        IGame UncoverSquare(Vector2D position);

        /// <summary>
        /// Sets or removes a flag from a square of the board like a Toggle.
        /// </summary>
        /// <param name="position">Where to set or remove a flag.</param>
        /// <returns>A new game object with the updated board.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        IGame FlagSquare(Vector2D position);

        /// <summary>
        /// Checks if square on <paramref name="position" /> is covered.
        /// </summary>
        /// <param name="position">The position of the square.</param>
        /// <returns>True if the square is covered at <paramref name="position"/>, false otherwise.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the game is over.</exception>
        bool IsSquareCovered(Vector2D position);

        /// <summary>
        /// Gets the adjacent mines of a square on <paramref name="position" /> when it's uncovered.
        /// </summary>
        /// <param name="position">The position of the square.</param>
        /// <returns>If the square is uncovered at <paramref name="position"/>, returns the amount of adjacent mines.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the square is covered is over.</exception>
        int GetAdjacentMines(Vector2D position);

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

        /// <summary>
        /// Change of a mine appearing.
        /// </summary>
        public const double ChanceOfMine = 0.2;
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
        protected Game(GameBoard gameBoard)
        {
            Board = gameBoard;
        }

        IGameBoard IGame.Board => Board;

        public GameBoard Board { get; }

        public abstract bool IsGameOver { get; }

        public abstract Vector2D MineHit { get; }

        public abstract IGame UncoverSquare(Vector2D position);

        public abstract IGame FlagSquare(Vector2D position);

        public abstract ISet<Vector2D> Mines { get; }

        public ISet<Vector2D> Flags => Board.Flags;

        public bool IsSquareCovered(Vector2D position) => Board[position].IsCovered;

        public int GetAdjacentMines(Vector2D position)
        {
            if (IsSquareCovered(position))
                throw new InvalidOperationException("Square is still covered");

            return Board[position].AmountOfMinesNear;
        }
    }

    internal class InProgressGame : Game
    {
        private bool isFloodingEnabled;

        public InProgressGame(int boardSize, bool flooding, int seed) : this(new GameBoard(boardSize, boardSize, seed, IGame.ChanceOfMine), flooding) { }

        private InProgressGame(GameBoard gameBoard, bool flooding) : base(gameBoard)
        {
            isFloodingEnabled = flooding;
        }

        public override bool IsGameOver => false;

        public override IGame UncoverSquare(Vector2D position)
        {
            if (!IsSquareCovered(position))
                throw new InvalidOperationException("Square already uncovered");

            var nextBoard = Board.Copy();
            if (nextBoard[position].Uncover())
                return new FinishedGame(position, nextBoard);

            if (isFloodingEnabled)
                nextBoard.FloodSquares(position);

            var emptySquaresLeft = nextBoard.UncoveredEmptySquares;

            if (emptySquaresLeft == 0)
                return new FinishedGame(null, nextBoard);

            return new InProgressGame(nextBoard, isFloodingEnabled);
        }

        public override IGame FlagSquare(Vector2D position)
        {
            if (!IsSquareCovered(position))
                throw new InvalidOperationException("Square already uncovered");

            var nextBoard = Board.Copy();
            nextBoard[position].ToggleFlag();
            return new InProgressGame(nextBoard, isFloodingEnabled);
        }

        public override Vector2D MineHit => null;

        public override ISet<Vector2D> Mines => throw new InvalidOperationException("Game is not over yet");
    }

    internal class FinishedGame : Game
    {
        public FinishedGame(Vector2D mine, GameBoard gameBoard) : base(gameBoard)
        {
            MineHit = mine;
        }

        public override bool IsGameOver => true;

        public override Vector2D MineHit { get; }

        public override ISet<Vector2D> Mines => Board.Mines;

        public override IGame UncoverSquare(Vector2D position) => throw new InvalidOperationException("Game finished");

        public override IGame FlagSquare(Vector2D position) => throw new InvalidOperationException("Game finished");
    }
}
