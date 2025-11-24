using AI.DataStructs.Algebraic;
using AI.ML.Clustering;
using System;
using System.Linq;

namespace AI.ML.HMM
{
    /// <summary>
    /// Скрытая марковская модель с выделением состояний на основе алгоритма k-средних
    /// </summary>
    [Serializable]
    public class KMeanHMM
    {
        /// <summary>
        /// Алгоритм выделения состояний на основе алгоритма k-средних
        /// </summary>
        public KMeans KMean { get; set; }

        private readonly HMM hmm = new HMM();

        /// <summary>
        /// Скрытая марковская модель с выделением состояний на основе алгоритма k-средних
        /// </summary>
        public KMeanHMM(int numClasters = 3)
        {
            KMean = new KMeans(numClasters);
        }

        /// <summary>
        /// Получение матрицы вероятностей переходов между состояниями
        /// </summary>
        /// <param name="seq">Последовательность векторов</param>
        public Matrix GetTransitionMatrix(Vector[] seq)
        {
            int[] states = KMean.Classify(seq).ToArray();
            hmm.Train(states);
            return hmm.stateMatrix;
        }

        /// <summary>
        /// Получение вектора вероятностей переходов между состояниями
        /// </summary>
        /// <param name="seq">Последовательность векторов</param>
        public Vector GetTransitionVector(Vector[] seq)
        {
            return GetTransitionMatrix(seq).Data;
        }

        /// <summary>
        /// Model training
        /// </summary>
        /// <param name="seqInp">Последовательность векторов</param>
        public void Train(Vector[] seqInp)
        {
            KMean.Train(seqInp);
            int[] states = KMean.Classify(seqInp).ToArray();
            hmm.Train(states);
        }

        /// <summary>
        /// Обучение только Марковской модели(без кластеризации)
        /// </summary>
        /// <param name="seqInp">Последовательность векторов</param>
        public void TrainHMM(Vector[] seqInp)
        {
            int[] states = KMean.Classify(seqInp).ToArray();
            hmm.Train(states);
        }

        /// <summary>
        /// Генерация последовательности состояний
        /// </summary>
        /// <param name="start">Вектор начала</param>
        /// <param name="len">Длинна последовательности</param>
        public int[] Generate(Vector start, int len)
        {
            int state = KMean.Classify(start);
            return hmm.Generate(len, state);
        }
    }
}
