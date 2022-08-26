using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Represents 1D shape
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Width = {Width}")]
    public sealed class Shape1D : Shape, IEquatable<Shape1D>
    {
        /// <summary>
        /// Width in 1D
        /// </summary>
        public int Width => this[0];

        /// <summary>
        /// Creates 1D shape
        /// </summary>
        /// <param name="width"></param>
        public Shape1D(int width = 1) : base(width) { }

        #region Операторы
        public static implicit operator Shape2D(Shape1D shape)
        {
            return new Shape2D(1, shape.Width);
        }

        public static implicit operator Shape3D(Shape1D shape)
        {
            return new Shape3D(1, shape.Width);
        }

        public static implicit operator Shape4D(Shape1D shape)
        {
            return new Shape4D(1, shape.Width);
        }
        #endregion

        public override Shape Expand(int newDimensionLength)
        {
            return new Shape2D(newDimensionLength, Width);
        }

        #region Технические методы
        public override string ToString()
        {
            return $"[W: {Width}]";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Shape1D other)
        {
            return Width == other.Width;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
