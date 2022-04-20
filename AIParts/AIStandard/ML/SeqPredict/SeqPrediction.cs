using AI.DataStructs.Algebraic;
using AI.ML.Regression;
using System;

namespace AI.ML.SeqPredict
{
    /// <summary>
    /// Прогнозирование последовательности
    /// </summary>
    [Serializable]
    public class SeqPrediction : ISeqPredict
    {
        private readonly IRegression regressor;
        private readonly int window;

        /// <summary>
        /// Прогнозирование последовательности
        /// </summary>
        /// <param name="reg">Алгоритм прогнозирования</param>
        /// <param name="w">Window width</param>
        public SeqPrediction(IRegression reg, int w)
        {
            regressor = reg;
            window = w;
        }

        /// <summary>
        /// Обучение модели
        /// </summary>
        /// <param name="data">Последовательность</param>
        public void Train(Vector data)
        {
            Vector seq = data.Clone();
            Vector Y = new Vector(seq.Count - window);

            for (int i = window; i < seq.Count; i++)
            {
                Y[i - window] = seq[i];
            }

            Vector[] X = Vector.GetWindows(seq, window, 1);
            regressor.Train(X, Y);
        }

        /// <summary>
        /// Прогноз
        /// </summary>
        /// <param name="data">Известная часть последовательности</param>
        /// <param name="n">Сколько шагов предсказать</param>
        public Vector Predict(Vector data, int n)
        {
            Vector outp = new Vector(data);

            for (int i = 0; i < n; i++)
            {
                int end = outp.Count;
                int start = end - window;
                outp.Add(regressor.Predict(outp.GetInterval(start, end)));
            }

            return outp;
        }

        public double PredictTrain(Vector data)
        {
            throw new System.NotImplementedException();
        }
    }
}
