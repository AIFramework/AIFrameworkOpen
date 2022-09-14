using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.Genetic.GeneticCore
{
    /// <summary>
    /// Клетка
    /// </summary>
    [Serializable]
    public class Cell
    {

        /// <summary>
        /// Набранные очки
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Параметры (хромосомы)
        /// </summary>
        public Vector Parametrs { get; set; }
        /// <summary>
        /// Полезная функция
        /// </summary>
        public Func<Vector, Vector, Vector> Function { get; set; }

        /// <summary>
        /// Клетка
        /// </summary>
        /// <param name="parametrsCount">Число параметров</param>
        /// <param name="function">Функция</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        /// <param name="valDown">Нижняя граница распределения</param>
        /// <param name="valUp">Верхняя граница распределения</param>
        public Cell(int parametrsCount, Func<Vector, Vector, Vector> function, Random rnd, double valDown, double valUp)
        {
            double picToPic = valUp - valDown;
            Parametrs = (picToPic * Statistics.Statistic.UniformDistribution(parametrsCount, rnd)) + valDown;
            Function = function;
        }

        /// <summary>
        /// Выход модели
        /// </summary>
        /// <param name="inpVector">Входной вектор</param>
        public Vector Output(Vector inpVector)
        {
            return Function(inpVector, Parametrs);
        }
    }
}
