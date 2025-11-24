using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    ///  RMSProp
    /// </summary>
    [Serializable]
    public class RMSProp : IOptimizer
    {

        /// <summary>
        /// Momentum
        /// </summary>
        public float DecayRate = 0.999f;
        /// <summary>
        ///  Коэф. сглаживания
        /// </summary>
        public float SmoothEpsilon = 1e-8f;

        /// <summary>
        /// Обновление параметров модели
        /// </summary>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="gradClip">Максимальное значение градиента по модулю</param>
        /// <param name="kG">Усиление градиента</param>
        /// <param name="L1">Коэф. L1 регуляризации</param>
        /// <param name="L2">Коэф. L2 регуляризации</param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float kG)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            _ = Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);


                    m.StepCache[i] = (m.StepCache[i] * DecayRate) + ((1 - DecayRate) * g * g);

                    m[i] -= learningRate * g / (float)Math.Sqrt(m.StepCache[i] + SmoothEpsilon);
                    m.DifData[i] = 0;
                }

            });
        }

        /// <summary>
        /// Сброс параметров обучения нейронной сети
        /// </summary>
        public void Reset()
        {
        }
    }
}
