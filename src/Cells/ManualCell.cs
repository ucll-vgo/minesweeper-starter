using System;

namespace Cells
{
    internal class ManualCell<T> : ConcreteCell<T>
    {
        private readonly Func<T> _reader;

        public ManualCell(Func<T> reader) : base(reader())
        {
            _reader = reader;
        }

        public override void Refresh() => base.Value = _reader();

        public override T Value
        {
            get => base.Value;
            set => throw new InvalidOperationException();
        }
    }

}
