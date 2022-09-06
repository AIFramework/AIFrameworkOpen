using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Represents 3D shape
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}, Depth = {Depth}")]
    public sealed class Shape3D : Shape, IEquatable<Shape3D>
    {
        #region Поля и свойства
        /// <summary>
        /// Height in 3D
        /// </summary>
        public int Height => this[1];
        /// <summary>
        /// Width in 3D
        /// </summary>
        public int Width => this[0];
        /// <summary>
        /// Depth in 3D
        /// </summary>
        public int Depth => this[2];
        /// <summary>
        /// Product of Height and Width
        /// </summary>
        public int Area => Height * Width;
        /// <summary>
        /// Product of Height, Width and Depth
        /// </summary>
        public int Volume => Count;
        #endregion

        /// <summary>
        /// Creates 3D shape
        /// </summary>
        /// <param name="height">Height</param>
        /// <param name="width">Width</param>
        /// <param name="depth">Depth</param>
        public Shape3D(int height = 1, int width = 1, int depth = 1) : base(width, height, depth) { }

        #region Операторы
        public static implicit operator Shape4D(Shape3D shape)
        {
            return new Shape4D(shape.Height, shape.Width, shape.Depth);
        }
        #endregion

        /// <summary>
        /// Сжатие формы
        /// </summary>
        public override Shape Shrink()
        {
            return new Shape2D(Height, Width);
        }

        /// <summary>
        /// Расширение формы
        /// </summary>
        /// <param name="newDimensionLength"></param>
        /// <returns></returns>
        public override Shape Expand(int newDimensionLength)
        {
            return new Shape4D(Height, Width, Depth, newDimensionLength);
        }

        #region Технические методы
        public override string ToString()
        {
            return $"[H:{Height}, W: {Width}, D: {Depth}]";
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Shape3D other)
        {
            return Width == other.Width && Height == other.Height && Depth == other.Depth;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}