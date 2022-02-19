using System;

namespace Cells
{
    public static class CellExtensions
    {
        public static IReactiveCell<TResult> Derive<T, TResult>(this IReactiveCell<T> cell, Func<T, TResult> func) => 
            ReactiveCell.Derived(cell, func);

        public static IReactiveCell<bool> Negate(this IReactiveCell<bool> cell) => cell.Derive(x => !x);

        public static void Update<T>(this IReactiveCell<T> cell, Func<T, T> updater) => cell.Value = updater(cell.Value);
    }
}
