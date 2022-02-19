using System;
using System.Collections.Generic;

namespace Model.Data
{
    /// <summary>
    /// A Vector2D consists of two integers named X and Y.
    /// </summary>
    public class Vector2D : IEquatable<Vector2D>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X-coordinate of the Vector2D
        /// </summary>
        public int X { get; }

        /// <summary>
        /// X-coordinate of the Vector2D
        /// </summary>
        public int Y { get; }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(Vector2D? other) => other != null && this.X == other.X && this.Y == other.Y;

        public override bool Equals(object? obj) => Equals(obj as Vector2D);

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        /// <summary>
        /// Vector addition.
        /// </summary>
        /// <param name="u">First vector</param>
        /// <param name="v">Second vector</param>
        /// <returns>Sum of both vectors</returns>
        public static Vector2D operator +(Vector2D u, Vector2D v) => new Vector2D(u.X + v.X, u.Y + v.Y);

        /// <summary>
        /// Vector negation.
        /// </summary>
        /// <param name="v">Vector to be negated</param>
        /// <returns>Negation of the vector</returns>
        public static Vector2D operator -(Vector2D v) => new Vector2D(-v.X, -v.Y);

        /// <summary>
        /// Vector subtraction.
        /// </summary>
        /// <param name="u">Vector to subtract from</param>
        /// <param name="v">Vector to subtract</param>
        /// <returns>Subtraction of v from u</returns>
        public static Vector2D operator -(Vector2D u, Vector2D v) => new Vector2D(u.X - v.X, u.Y - v.Y);

        /// <summary>
        /// Multiplication between vector and number
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="f">Factor</param>
        /// <returns>Product</returns>
        public static Vector2D operator *(Vector2D v, int f) => new Vector2D(v.X * f, v.Y * f);

        /// <summary>
        /// Multiplication between vector and number
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="f">Factor</param>
        /// <returns>Product</returns>
        public static Vector2D operator *(int f, Vector2D v) => v * f;

        public static readonly Vector2D NORTH = new Vector2D(0, 1);

        public static readonly Vector2D SOUTH = new Vector2D(0, -1);

        public static readonly Vector2D EAST = new Vector2D(1, 0);

        public static readonly Vector2D WEST = new Vector2D(-1, 0);

        public static readonly Vector2D NORTHEAST = NORTH + EAST;

        public static readonly Vector2D SOUTHEAST = SOUTH + EAST;

        public static readonly Vector2D NORTHWEST = NORTH + WEST;

        public static readonly Vector2D SOUTHWEST = SOUTH + WEST;

        /// <summary>
        /// Returns a list of vectors pointing in all 8 directions.
        /// </summary>
        public static IEnumerable<Vector2D> AllDirections
        {
            get
            {
                yield return NORTH;
                yield return NORTHEAST;
                yield return EAST;
                yield return SOUTHEAST;
                yield return SOUTH;
                yield return SOUTHWEST;
                yield return WEST;
                yield return NORTHWEST;
            }
        }
    }
}
