using System;

namespace Cells
{
    public interface ICell<T> : IVar<T>, IEquatable<ICell<T>>
    {
        public event Action ValueChanged;

        public void Refresh();        
    }
}
