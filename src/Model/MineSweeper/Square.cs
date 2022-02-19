namespace Model.MineSweeper
{
    /// <summary>
    /// Represents a square. A square can be either empty or contain a mine.
    /// </summary>
    public class Square
    {
        public int AmountOfMinesNear { get; }

        internal bool IsMine { get; }
        internal bool IsCovered { get; private set; }
        internal bool IsFlagged { get; private set; }

        internal Square(bool mine, int minesNear)
        {
            IsMine = mine;
            IsCovered = true;
            IsFlagged = false;
            AmountOfMinesNear = minesNear;
        }

        internal bool Uncover()
        {
            IsCovered = false;
            IsFlagged = false;
            return IsMine;
        }

        internal void ToggleFlag()
        {
            IsFlagged = !IsFlagged;
        }

        public override string ToString()
        {
            return IsCovered ? "" : IsFlagged ? "F" : IsMine ? "B" : AmountOfMinesNear.ToString();
        }
    }
}
