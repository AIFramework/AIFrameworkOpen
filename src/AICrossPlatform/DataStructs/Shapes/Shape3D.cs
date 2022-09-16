using System;
using System.Diagnostics;

namespace AI.DataStructs.Shapes
{
    /// <summary>
    /// Трехмерная форма
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Height = {Height}, Width = {Width}, Depth = {Depth}")]
    public sealed class Shape3D : Shape, IEquatable<Shape3D>
    {
        #region Поля и свойства
        /// <summary>
        /// Высота тензора
        /// </summary>
        public int Height => this[1];
        /// <summary>
        /// Ширина тензора
        /// </summary>
        public int Width => this[0];
        /// <summary>
        /// Глубина тензора
        /// </summary>
        public int Depth => this[2];
        /// <summary>
        /// Площадь (Произведение ширины на глубину)
        /// </summary>
        public int Area => Height * Width;
        /// <summary>
        /// Объем (произведение ширины, высоты и глубины) 
        /// </summary>
        public int Volume => Count;
        #endregion

        /// <summary>
        /// Трехмерная форма
        /// </summary>
        /// <param name="height">Высота</param>
        /// <param name="width">Ширина</param>
        /// <param name="depth">Глубина</param>
        public Shape3D(int height = 1, int width = 1, int depth = 1) : base(width, height, depth) { }

        #region Операторы
        /// <summary>
        /// Преобразование трехмерной формы в четырех мерную
        /// </summary>
        /// <param name="shape"></param>
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
        /// <summary>
        /// Преобразование формы в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[H:{Height}, W: {Width}, D: {Depth}]";
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public bool Equals(Shape3D other)
        {
            return Width == other.Width && Height == other.Height && Depth == other.Depth;
        }

        /// <summary>
        /// Проверка равенства
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Создание 3х мерной из многомерной
        /// </summary>
        public static Shape3D FromeShape(Shape shape) 
        {
            if (shape.Rank > 3) throw new Exception("Невозможно создать трехмерную форму, размерность входа больше 3");

            switch (shape.Rank) 
            {
                case 1:
                    return new Shape3D(shape[0]);
                case 2:
                    return new Shape3D(shape[0], shape[1]);
                case 3:
                    return new Shape3D(shape[0], shape[1], shape[2]);
            }

            throw new Exception("Неожиданная размерность");
        }
    }
}