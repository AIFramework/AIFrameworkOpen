/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 28.06.2017
 * Время: 12:46
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI
{
    /// <summary>
    /// Данные интервалов
    /// </summary>
    [Serializable]
    public class IntervalData
    {
        private readonly List<int> bIntervals = new List<int>();
        private readonly List<int> endIntervals = new List<int>();


        /// <summary>
        /// Данные интервалов
        /// </summary>
        public IntervalData() { }

        /// <summary>
        /// Добавление интервала
        /// </summary>
        /// <param name="bI">Начало</param>
        /// <param name="eI">Конец</param>
        public void Add(int bI, int eI)
        {
            bIntervals.Add(bI);
            endIntervals.Add(eI);
        }


        /// <summary>
        /// Нарезка вектора по интервалам
        /// </summary>
        /// <param name="inputVector">Вектор входных данных</param>
        public Vector[] GetVects(Vector inputVector)
        {
            Vector[] outps = new Vector[bIntervals.Count];

            for (int i = 0; i < outps.Length; i++)
            {
                outps[i] = inputVector.GetInterval(bIntervals[i], endIntervals[i]);
            }

            return outps;
        }

        /// <summary>
        /// Нарезка вектора по интервалам + преобразование
        /// </summary>
        /// <param name="vect2doub">Функция для преобразования вектора в число</param>
        /// <param name="input">Вектор входных данных</param>
        public Vector GetVect(Func<Vector, double> vect2doub, Vector input)
        {
            Vector outps = new Vector(bIntervals.Count);

            for (int i = 0; i < outps.Count; i++)
            {
                outps[i] = vect2doub(input.GetInterval(bIntervals[i], endIntervals[i]));
            }

            return outps;
        }

    }

}
