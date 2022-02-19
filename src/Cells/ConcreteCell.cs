
using System.Collections.Generic;

namespace Cells
{
    internal class ConcreteCell<T> : Cell<T>
    {
        private T _value;

        public ConcreteCell(T initialValue)
        {
            _value = initialValue;
        }

        public override T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(this._value, value))
                {
                    this._value = value;
                    NotifyObservers();
                }
            }
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
