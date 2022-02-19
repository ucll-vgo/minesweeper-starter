using System;

namespace Cells
{
    public static class CellExtensions
    {
        public static ICell<TResult> Derive<T, TResult>(this ICell<T> cell, Func<T, TResult> func) => 
            Cell.Derived(cell, func);

        public static ICell<bool> Negate(this ICell<bool> cell) => cell.Derive(x => !x);

        public static void Update<T>(this ICell<T> cell, Func<T, T> updater) => cell.Value = updater(cell.Value);
    }
}
