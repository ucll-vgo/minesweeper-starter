using System;
using System.Linq;
using System.Collections.Generic;

namespace Model.Data
{
    internal class Grid<T>
    {
        private readonly T[,] _grid;

        public Grid(int width, int height, Func<Vector2D, T> initializer)
        {
            if (width <= 0) throw new ArgumentException("Should be at least 1", nameof(width));
            if (height <= 0) throw new ArgumentException("Should be at least 1", nameof(height));

            _grid = new T[height, width];

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    _grid[y, x] = initializer(new Vector2D(x, y));
        }

        public Grid(int width, int height, T initial = default) : this(width, height, _ => initial)
        {
            // NOP
        }

        public T this[Vector2D position]
        {
            get => _grid[position.Y, position.X];
            set => _grid[position.Y, position.X] = value;
        }

        public int Width => _grid.GetLength(1);

        public int Height => _grid.GetLength(0);

        public Grid<T> Copy() => new Grid<T>(Width, Height, p => this[p]);

        public bool IsValidPosition(Vector2D position) =>
            0 <= position.X && position.X < Width && 0 <= position.Y && position.Y < Height;

        public GridSlice<T> Row(int rowIndex) => new GridSlice<T>(this, new Vector2D(0, rowIndex), Vector2D.EAST);

        public GridSlice<T> Column(int columnIndex) => new GridSlice<T>(this, new Vector2D(columnIndex, 0), Vector2D.NORTH);

        public IEnumerable<GridSlice<T>> Rows => Enumerable.Range(0, Width).Select(Row);

        public IEnumerable<GridSlice<T>> Columns => Enumerable.Range(0, Width).Select(Column);

        public GridSlice<T> Slice(Vector2D startPosition, Vector2D direction) =>
            new GridSlice<T>(this, startPosition, direction);

        public IEnumerable<Vector2D> Positions =>
            Enumerable.Range(0, Height).SelectMany(y => Enumerable.Range(0, Width).Select(x => new Vector2D(x, y)));

        public Grid<U> Map<U>( Func<T, U> mapper ) => new Grid<U>( this.Width, this.Height, p => mapper( this[p] ) );

        public Grid<U> Map<U>( Func<Vector2D, T, U> mapper ) => new Grid<U>( this.Width, this.Height, p => mapper( p, this[p] ) );

        public IEnumerable<Vector2D> Around( Vector2D position ) => Vector2D.AllDirections.Select( dp => position + dp ).Where( IsValidPosition );

        public IEnumerable<T> Items => Positions.Select( p => this[p] );
    }

    internal class GridSlice<T>
    {
        private readonly Grid<T> _grid;

        private readonly Vector2D _position;

        private readonly Vector2D _direction;

        public GridSlice(Grid<T> grid, Vector2D position, Vector2D direction)
        {
            _grid = grid;
            _position = position;
            _direction = direction;
        }

        public T this[int index]
        {
            get => _grid[PositionAt(index)];
            set => _grid[PositionAt(index)] = value;
        }

        public bool IsValidIndex(int index) => _grid.IsValidPosition(PositionAt(index));
        public Vector2D PositionAt(int index) => _position + index * _direction;
    }    
}
