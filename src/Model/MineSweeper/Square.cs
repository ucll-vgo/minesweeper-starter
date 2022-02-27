namespace Model.MineSweeper
{
    /// <summary>
    /// Represents a square. A square can be either empty or contain a mine.
    /// </summary>
    public class Square
    {
        public int NeighboringMineCount { get; }

        public bool IsMine { get; }

        public bool IsCovered { get; private set; }

        public bool IsFlagged { get; private set; }

        internal Square(bool mine, int neighboringMineCount)
        {
            IsMine = mine;
            IsCovered = true;
            IsFlagged = false;
            NeighboringMineCount = neighboringMineCount;
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
            return IsCovered ? "?" : IsFlagged ? "F" : IsMine ? "B" : NeighboringMineCount.ToString();
        }
    }
}
