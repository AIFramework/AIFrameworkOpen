using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Четырехмерная форма
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}, Depth = {Depth}, X = {X}")]
    public sealed class Shape4D : Shape, IEquatable<Shape4D>
    {
        #region Поля и свойства
        /// <summary>
        /// Высота in 4D
        /// </summary>
        public int Height => this[1];
        /// <summary>
        /// Ширина in 4D
        /// </summary>
        public int Width => this[0];
        /// <summary>
        /// Глубина in 4D
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
        /// Четырехмерная форма
        /// </summary>
        /// <param name="height">Высота</param>
        /// <param name="width">Ширина</param>
        /// <param name="depth">Глубина</param>
        /// <param name="x">четвертая размерность</param>
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
