using System;

namespace Cells
{
    internal class ReadonlyWrapper<T> : Cell<T>
    {
        private readonly Cell<T> _wrappedCell;

        public ReadonlyWrapper(Cell<T> wrappedCell)
        {
            _wrappedCell = wrappedCell;
            _wrappedCell.ValueChanged += NotifyObservers;
        }

        public override T Value
        {
            get => _wrappedCell.Value;
            set => throw new InvalidOperationException("Cannot modify value of readonly view of cell");
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
