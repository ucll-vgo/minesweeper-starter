using System;

namespace Cells
{
    public interface IReactiveCell<T> : IVar<T>, IEquatable<IReactiveCell<T>>
    {
        public event Action ValueChanged;

        public void Refresh();        
    }
}
