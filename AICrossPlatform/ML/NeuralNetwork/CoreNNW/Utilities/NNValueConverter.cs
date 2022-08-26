using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Utilities
{
    [Serializable]
    public static class NNValueConverter
    {
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