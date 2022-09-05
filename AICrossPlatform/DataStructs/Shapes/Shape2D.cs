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
        public Shape2D(int height = 1, int width = 1) : base(width, height) { }

        #region Операторы
        /// <summary>
        /// Создание 3х мерной формы
        /// </summary>
        public static implicit operator Shape3D(Shape2D shape)
        {
            return new Shape3D(shape.Height, shape.Width);
        }
        /// <summary>
        /// Создание 4х мерной формы
        /// </summary>
        public static implicit operator Shape4D(Shape2D shape)
        {
            return new Shape4D(shape.Height, shape.Width);
        }
        #endregion

        /// <summary>
        /// Уменьшение размерности формы
        /// </summary>
        public override Shape Shrink()
        {
            return new Shape1D(Width);
        }
        /// <summary>
        /// Увеличение размерности формы
        /// </summary>
        public override Shape Expand(int newDimensionLength)
        {
            return new Shape3D(Height, Width, newDimensionLength);
        }

        #region Технические методы
        /// <summary>
        /// Перевод в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[H: {Height}, W: {Width}]";
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Сравнение
        /// </summary>
        public bool Equals(Shape2D other)
        {
            return Width == other.Width && Height == other.Height;
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