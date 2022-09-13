using AI.DataStructs.Algebraic;
using AI.ML.Regression;
using System;

namespace AI.ML.SeqPredict
{
    /// <summary>
    /// Авторегрессионная модель
    /// </summary>
    [Serializable]
    public class AR : ISeqPredict
    {
        private readonly SeqPrediction prediction;

        /// <summary>
        /// Авторегрессионная модель
        /// </summary>
        /// <param name="windowLenght">Окно предыдущих состояний</param>
        public AR(int windowLenght)
        {
            MultipleRegression multipleRegression = new MultipleRegression(true);
            prediction = new SeqPrediction(multipleRegression, windowLenght);
        }

        /// <summary>
        /// Прогноз на n шагов вперед
        /// </summary>
        /// <param name="data"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public Vector Predict(Vector data, int n)
        {
            return prediction.Predict(data, n);
        }

        /// <summary>
        /// Метод не реализован
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double PredictTrain(Vector data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обучение на базе временного ряда
        /// </summary>
        /// <param name="data"></param>
        public void Train(Vector data)
        {
            prediction.Train(data);
        }
    }
}
