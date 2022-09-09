using AI.DataStructs.Algebraic;

namespace AI.ML.Regression
{
    /// <summary>
    /// Интерфейс регрессии с многими выходами
    /// </summary>
    public interface IRegression
    {
        /// <summary>
        /// Обучение регрессии
        /// </summary>
        /// <param name="data">Входные векторы(признаки)</param>
        /// <param name="targets">Выходной вектор(целевые значения)</param>
        void Train(Vector[] data, Vector targets);
        /// <summary>
        /// Предсказание на базе модели
        /// </summary>
        /// <param name="data">Вектор признаков</param>
        double Predict(Vector data);
    }
}
