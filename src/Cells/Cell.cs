using System;
using System.ComponentModel;

namespace Cells
{
    internal abstract class Cell<T> : ICell<T>, INotifyPropertyChanged
    {
        public abstract T Value { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event Action ValueChanged
        {
            add => PropertyChanged += (obj, args) => value();
            remove => throw new NotSupportedException();
        }

        protected void NotifyObservers() =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( "Value" ));

        public override bool Equals(object? obj) => Equals(obj as ICell<T>);

        public bool Equals(ICell<T>? that)
        {
            if (that == null)  return false;
            else if (Value == null) return that.Value == null;
            else return Value.Equals(that.Value);
        }

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => $"CELL<{typeof(T).Name}>[{Value}]";

        public abstract void Refresh();
    }
}
