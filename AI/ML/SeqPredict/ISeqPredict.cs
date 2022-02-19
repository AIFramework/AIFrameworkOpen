using AI.DataStructs.Algebraic;

namespace AI.ML.SeqPredict
{
    /// <summary>
    /// Прогнозирование последовательностей
    /// </summary>
    public interface ISeqPredict
    {


        /// <summary>
        /// Обучение
        /// </summary>
        /// <param name="data">Данные</param>
        void Train(Vector data);

        /// <summary>
        /// Прогноз
        /// </summary>
        Vector Predict(Vector data, int n);

        /// <summary>
        /// Обучение и прогноз на 1
        /// </summary>
        double PredictTrain(Vector data);
    }
}
