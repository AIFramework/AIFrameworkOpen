using AI.DataStructs.Algebraic;

namespace AI.ML.Regression
{
    /// <summary>
    /// Интерфейс регрессии с многими выходами
    /// </summary>
    public interface IMultyRegression<T>
    {
        /// <summary>
        /// Обучение регрессии
        /// </summary>
        /// <param name="data">Входные векторы(признаки)</param>
        /// <param name="targets">Выходные векторы(целевые значения)</param>
        void Train(T[] data, Vector[] targets);
        /// <summary>
        /// Предсказание на базе модели
        /// </summary>
        /// <param name="data">Вектор признаков</param>
        Vector Predict(T data);
    }
}
