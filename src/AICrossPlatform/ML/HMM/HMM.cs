using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.HMM
{
    /// <summary>
    /// Дискретная Марковская цепь
    /// </summary>
    [Serializable]
    public class HMM
    {
        /// <summary>
        /// Матрица состояний
        /// </summary>
        public Matrix stateMatrix;
        /// <summary>
        /// Обратная матрица состояний (1-matr_state)
        /// </summary>
        public Matrix stateAlter;
        private int[] states;
        private readonly Random rnd = new Random();

        /// <summary>
        /// Марковская цепь
        /// </summary>
        public HMM()
        {

        }

        /// <summary>
        /// Обучение
        /// </summary>
        /// <param name="trainSeq">Изучаемая последовательность</param>
        public void Train(int[] trainSeq)
        {

            double[,] _stateMatrix, _stateAlter;
            states = trainSeq;


            _stateMatrix = new double[states.Length, states.Length];
            _stateAlter = new double[states.Length, states.Length];
            int len = states.Length * states.Length;


            for (int i = 0; i < trainSeq.Length - 1; i++)
            {
                for (int j = 0; j < states.Length; j++)
                {
                    for (int k = 0; k < states.Length; k++)
                    {
                        if (trainSeq[i] == states[j]
                        && trainSeq[i + 1] == states[k])
                        {
                            _stateMatrix[j, k]++;
                            _stateAlter[j, k]++;
                            break;
                        }
                    }
                }
            }


            double max = GetMax(_stateAlter);

            for (int j = 0; j < states.Length; j++)
            {
                for (int k = 0; k < states.Length; k++)
                {
                    _stateMatrix[j, k] /= trainSeq.Length;
                    _stateAlter[j, k] /= max;
                    _stateAlter[j, k] = (1 - _stateAlter[j, k]) * 0.9999;
                }
            }

            stateAlter = new Matrix(_stateAlter);
            stateMatrix = new Matrix(_stateMatrix);
        }

        /// <summary>
        /// Максимальная вероятность перехода
        /// </summary>
        /// <param name="matrix">Матрица состояний</param>
        private double GetMax(double[,] matrix)
        {
            double max = matrix[0, 0];

            for (int j = 0; j < states.Length; j++)
            {
                for (int k = 0; k < states.Length; k++)
                {
                    if (matrix[j, k] > max)
                    {
                        max = matrix[j, k];
                    }
                }
            }

            return max;
        }

        /// <summary>
        /// Генерация текста
        /// </summary>
        /// <param name="num">Число шагов</param>
        /// <param name="begin">Первое слово</param>
        public int[] Generate(int num, int begin)
        {
            int[] seq = new int[num];
            int ch;
            seq[0] = begin;


            for (int i = 1; i < num; i++)
            {

                while (true)
                {
                    ch = rnd.Next(states.Length);

                    if (rnd.NextDouble() > stateAlter[GetInd(seq[i - 1]), ch])
                    {
                        seq[i] = states[ch];
                        break;
                    }

                }
            }


            return seq;
        }

        // <summary>
        // Поиск индекса
        // </summary>
        // <param name="state">состояние состояния</param>
        private int GetInd(int state)
        {
            int ind = 0;

            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i])
                {
                    ind = i;
                    break;
                }
            }

            return ind;
        }

    }
}
