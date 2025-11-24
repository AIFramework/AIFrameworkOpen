using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    /// <summary>
    /// Конвертер из значения нейронной сети
    /// </summary>
    [Serializable]
    public static class NNValueConverter
    {
        /// <summary>
        /// Тензор нейронной сети превращает в int (индекс максимального элемента)
        /// </summary>
        /// <param name="value">Тензор нейронки</param>
        public static int NNValueToClass(NNValue value)
        {
            int indMax = 0;

            for (int i = 1; i < value.Shape.Count; i++)
            {
                if (value[i] > value[indMax])
                {
                    indMax = i;
                }
            }

            return indMax;
        }

        /// <summary>
        /// Массив тензоров нейронной сети превращает в массив int (индекс максимального элемента)
        /// </summary>
        /// <param name="nNValues">Массив тензоров </param>
        public static int[] NNValuesToClasses(NNValue[] nNValues)
        {
            int[] classes = new int[nNValues.Length];

            for (int i = 0; i < classes.Length; i++)
            {
                classes[i] = NNValueToClass(nNValues[i]);
            }

            return classes;
        }
    }
}