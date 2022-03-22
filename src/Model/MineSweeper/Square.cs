namespace Model.MineSweeper
{
    /// <summary>
    /// Enum that represents the different possible statuses of a square.
    /// </summary>
    public enum SquareStatus
    {
        /// <summary>
        /// Square is covered and unflagged.
        /// </summary>
        Covered,
        /// <summary>
        /// Square is covered and flagged.
        /// </summary>
        Flagged,
        /// <summary>
        /// Square is uncovered and contains a mine.
        /// </summary>
        Mine,
        /// <summary>
        /// Square is uncovered and contains no mine.
        /// </summary>
        Uncovered
    }

    /// <summary>
    /// Represents a square. A square can be either empty or contain a mine.
    /// </summary>
    public class Square
    {
        internal bool ContainsMine { get; }

        internal bool IsCovered { get; private set; }

        internal bool IsFlagged { get; private set; }

        internal Square(bool containsMine, int neighboringMineCount)
        {
            this.ContainsMine = containsMine;
            this.IsCovered = true;
            this.IsFlagged = false;
            this.NeighboringMineCount = neighboringMineCount;
        }

        internal Square(Square square)
        {
            this.ContainsMine = square.ContainsMine;
            this.IsCovered = square.IsCovered;
            this.IsFlagged = square.IsFlagged;
            this.NeighboringMineCount = square.NeighboringMineCount;
        }

        internal void Uncover()
        {
            IsCovered = false;
            IsFlagged = false;
        }

        internal void ToggleFlag()
        {
            IsFlagged = !IsFlagged;
        }

        public override string ToString()
        {
            return IsFlagged ? "F" : IsCovered ? "?" : ContainsMine ? "M" : NeighboringMineCount.ToString();
        }

        public int NeighboringMineCount { get; }

        public SquareStatus Status
        {
            get
            {
                if ( IsFlagged )
                {
                    return SquareStatus.Flagged;
                }
                else if ( IsCovered )
                {
                    return SquareStatus.Covered;
                }
                else if ( ContainsMine )
                {
                    return SquareStatus.Mine;
                }
                else
                {
                    return SquareStatus.Uncovered;
                }
            }
        }
    }
}
