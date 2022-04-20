using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    [Serializable]
    internal static class BaseMethods
    {
        public static float GradCulc(NNValue m, int i, float L1, float L2, float gradG, float gradClip)
        {
            float g = m.DifData[i];
            g += L2 * m[i] + (m[i] > 0 ? L1 : -L1); // Градиент с учетом регуляризации
            g *= gradG; // Учет батчей

            // gradient clip
            if (g > gradClip)
            {
                g = gradClip;
            }
            if (g < -gradClip)
            {
                g = -gradClip;
            }

            return g;
        }
    }
}
