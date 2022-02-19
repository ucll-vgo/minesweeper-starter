using System;
using System.Collections.Generic;
using System.Linq;

namespace Cells
{
    public static class Cell
    {
        public static ICell<T> Create<T>(T initialValue)
        {
            return new ConcreteCell<T>(initialValue);
        }

        public static ICell<T> Constant<T>(T value)
        {
            return new ConstantCell<T>(value);
        }

        private static void RegisterObserver<T, R>(ManualCell<R> derived, ICell<T> cell)
        {
            cell.ValueChanged += derived.Refresh;
        }

        internal static ICell<TResult> Derived<T, TResult>(ICell<T> cell, Func<T, TResult> function)
        {
            var derived = new ManualCell<TResult>(() => function(cell.Value));

            RegisterObserver(derived, cell);

            return derived;
        }

        public static ICell<TResult> Derived<T1, T2, TResult>(ICell<T1> c1, ICell<T2> c2, Func<T1, T2, TResult> function)
        {
            if (c1 == null) throw new ArgumentNullException(nameof(c1));
            if (c2 == null) throw new ArgumentNullException(nameof(c2));

            var derived = new ManualCell<TResult>(() => function(c1.Value, c2.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);

            return derived;
        }

        public static ICell<TResult> Derived<T1, T2, T3, TResult>(ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, Func<T1, T2, T3, TResult> function)
        {
            if (c1 == null) throw new ArgumentNullException(nameof(c1));
            if (c2 == null) throw new ArgumentNullException(nameof(c2));
            if (c3 == null) throw new ArgumentNullException(nameof(c3));

            var derived = new ManualCell<TResult>(() => function(c1.Value, c2.Value, c3.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);

            return derived;
        }

        public static ICell<TResult> Derived<T1, T2, T3, T4, TResult>(ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, ICell<T4> c4, Func<T1, T2, T3, T4, TResult> function)
        {
            if (c1 == null) throw new ArgumentNullException(nameof(c1));
            if (c2 == null) throw new ArgumentNullException(nameof(c2));
            if (c3 == null) throw new ArgumentNullException(nameof(c3));
            if (c4 == null) throw new ArgumentNullException(nameof(c4));

            var derived = new ManualCell<TResult>(() => function(c1.Value, c2.Value, c3.Value, c4.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);
            RegisterObserver(derived, c4);

            return derived;
        }

        public static ICell<TResult> Derived<T1, T2, T3, T4, T5, TResult>(ICell<T1> c1, ICell<T2> c2, ICell<T3> c3, ICell<T4> c4, ICell<T5> c5, Func<T1, T2, T3, T4, T5, TResult> function)
        {
            if (c1 == null) throw new ArgumentNullException(nameof(c1));
            if (c2 == null) throw new ArgumentNullException(nameof(c2));
            if (c3 == null) throw new ArgumentNullException(nameof(c3));
            if (c4 == null) throw new ArgumentNullException(nameof(c4));
            if (c5 == null) throw new ArgumentNullException(nameof(c5));

            var derived = new ManualCell<TResult>(() => function(c1.Value, c2.Value, c3.Value, c4.Value, c5.Value));

            RegisterObserver(derived, c1);
            RegisterObserver(derived, c2);
            RegisterObserver(derived, c3);
            RegisterObserver(derived, c4);
            RegisterObserver(derived, c5);

            return derived;
        }

        public static ICell<TResult> Derived<T, TResult>(IEnumerable<ICell<T>> cells, Func<IEnumerable<T>, TResult> function)
        {
            if (cells == null) throw new ArgumentNullException(nameof(cells));

            var derived = new ManualCell<TResult>(() => function(cells.Select(cell => cell.Value)));

            foreach (var cell in cells)
            {
                RegisterObserver(derived, cell);
            }

            return derived;
        }
    }
}
