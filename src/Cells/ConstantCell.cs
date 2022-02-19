using System;

namespace Cells
{
    internal class ConstantCell<T> : Cell<T>
    {
        private readonly T _value;

        public ConstantCell(T value)
        {
            this._value = value;
        }

        public override T Value
        {
            get => _value;
            set => throw new InvalidOperationException("Cannot set value of constant cell");
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
