/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 14.09.2018
 * Время: 10:42
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */

using System;

namespace AI.Charts
{
    /// <summary>
    /// Описание для графика
    /// </summary>
    [Serializable]
    public class Description
    {
        /// <summary>
        /// Название оси X
        /// </summary>
        public string X;
        /// <summary>
        /// Название оси Y
        /// </summary>
        public string Y;
        /// <summary>
        /// Название графика
        /// </summary>
        public string Name;

        /// <summary>
        /// Описание графика
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// Описание графика
        /// </summary>
        /// <param name="xL">Название оси X</param>
        /// <param name="yL">Название оси Y</param>
        /// <param name="name">Название графика</param>
        public Description(string xL = "x", string yL = "f(x)", string name = "Chart")
        {
            X = xL;
            Y = yL;
            Name = name;
        }
    }
}
