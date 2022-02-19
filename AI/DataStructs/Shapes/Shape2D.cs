using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Represents 2D shape
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}")]
    public sealed class Shape2D : Shape, IEquatable<Shape2D>
    {
        #region Поля и свойства
        /// <summary>
        /// Height in 2D
        /// </summary>
        public int Height => this[1];
        /// <summary>
        /// Width in 2D
        /// </summary>
        public int Width => this[0];
        /// <summary>
        /// Product of Height and Width
        /// </summary>
        public int Area => Count;
        #endregion

        /// <summary>
        /// Creates 2D shape
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        public Shape2D(int height = 1, int width = 1) : base(width, height) {}

        #region Операторы
        public static implicit operator Shape3D(Shape2D shape)
        {
            return new Shape3D(shape.Height, shape.Width);
        }

        public static implicit operator Shape4D(Shape2D shape)
        {
            return new Shape4D(shape.Height, shape.Width);
        }
        #endregion

        public override Shape Shrink()
        {
            return new Shape1D(Width);
        }

        public override Shape Expand(int newDimensionLength)
        {
            return new Shape3D(Height, Width, newDimensionLength);
        }

        #region Технические методы
        public override string ToString()
        {
            return $"[H: {Height}, W: {Width}]";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Shape2D other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}