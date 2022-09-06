using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Represents 4D shape
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}, Depth = {Depth}, X = {X}")]
    public sealed class Shape4D : Shape, IEquatable<Shape4D>
    {
        #region Поля и свойства
        /// <summary>
        /// Height in 4D
        /// </summary>
        public int Height => this[1];
        /// <summary>
        /// Width in 4D
        /// </summary>
        public int Width => this[0];
        /// <summary>
        /// Depth in 4D
        /// </summary>
        public int Depth => this[2];
        /// <summary>
        /// Fourth dimension length
        /// </summary>
        public int X => this[3];
        /// <summary>
        /// Product of Height and Width
        /// </summary>
        public int Area => Height * Width;
        /// <summary>
        /// Product of Height, Width and Depth
        /// </summary>
        public int Volume => Height * Width * Depth;
        #endregion

        /// <summary>
        /// Creates 4D shape
        /// </summary>
        /// <param name="height">Height</param>
        /// <param name="width">Width</param>
        /// <param name="depth">Depth</param>
        /// <param name="x">Fourth dimension</param>
        public Shape4D(int height = 1, int width = 1, int depth = 1, int x = 1) : base(width, height, depth, x) { }

        /// <summary>
        /// Сжатие формы
        /// </summary>
        /// <returns></returns>
        public override Shape Shrink()
        {
            return new Shape3D(Height, Width, Depth);
        }

        #region Технические методы

        /// <summary>
        /// Перевод в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[H:{Height}, W: {Width}, D: {Depth}, X: {X}]";
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        public bool Equals(Shape4D other)
        {
            return Width == other.Width && Height == other.Height && Depth == other.Depth && X == other.X;
        }

        /// <summary>
        /// Хэш-код
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
