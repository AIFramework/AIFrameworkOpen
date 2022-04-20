using AI.DataStructs.Algebraic;
using System;

namespace AI.DSP
{
    /// <summary>
    /// Базовый интерфейс реализации спектров
    /// </summary>
    public interface ISpectr
    {
        /// <summary>
        /// Отсчеты частоты
        /// </summary>
        Vector Freq { get; set; }
        /// <summary>
        /// Данные спектра
        /// </summary>
        Vector Data { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Название шкалы Y
        /// </summary>
        string YLable { get; set; }
        /// <summary>
        /// Название шкалы X
        /// </summary>
        string XLable { get; set; }
        /// <summary>
        /// Выводится ли шкала данные по Y в децибелах
        /// </summary>
        bool IsDbScale { get; set; }
        /// <summary>
        /// Логарифмическая ли шкала частот
        /// </summary>
        bool LogScaleX { get; set; }
    }

    /// <summary>
    /// Амплитудный спектр
    /// </summary>
    public class AmplitudeSpectr : ISpectr
    {
        /// <summary>
        /// Отсчеты частоты
        /// </summary>
        public Vector Freq { get; set; }
        /// <summary>
        /// Амплитуды спектра
        /// </summary>
        public Vector Data { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Название шкалы Y
        /// </summary>
        public string YLable { get; set; }
        /// <summary>
        /// Название шкалы X
        /// </summary>
        public string XLable { get; set; }
        /// <summary>
        /// Выводится ли шкала данные по Y в децибелах
        /// </summary>
        public bool IsDbScale { get; set; }
        /// <summary>
        /// Логарифмическая ли шкала частот
        /// </summary>
        public bool LogScaleX { get; set; }

        /// <summary>
        /// Амплитудный спектр
        /// </summary>
        /// <param name="channel">Channel</param>
        /// <param name="isDbScale">Выражать ли в db, 20log(x)</param>
        public AmplitudeSpectr(Channel channel, bool isDbScale = false)
        {
            IsDbScale = isDbScale;
            Freq = channel.Freq();
            Data = (IsDbScale) ? channel.GetSpectr().Transform(x => 20 * Math.Log10(x)) : channel.GetSpectr();
            Name = "Спектр [\"" + channel.Name + "\"]";
            XLable = "Частота [Гц]";
            YLable = "Амплитуда " + ((IsDbScale) ? "[db]" : channel.YName());
        }

        /// <summary>
        /// Амплитудный спектр
        /// </summary>
        /// <param name="channel">Channel</param>
        /// <param name="windowWFunc">Оконная функция</param>
        /// <param name="isDbScale">Выражать ли в db, 20log(x)</param>
        public AmplitudeSpectr(Channel channel, Func<int, Vector> windowWFunc, bool isDbScale = false)
        {
            IsDbScale = isDbScale;
            Freq = channel.Freq();
            Data = (IsDbScale) ? channel.GetSpectr(windowWFunc).Transform(x => 20 * Math.Log10(x)) : channel.GetSpectr(windowWFunc);
            Name = "Спектр [\"" + channel.Name + "\"]";
            XLable = "Частота [Гц]";
            YLable = "Амплитуда " + ((IsDbScale) ? "[db]" : channel.YName());
        }
    }
}
