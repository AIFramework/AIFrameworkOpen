using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Adagrad
    /// </summary>
    [Serializable]
    public class Adagrad : IOptimizer
    {
        private readonly float SmoothEpsilon = 1e-2f;
        /// <summary>
        /// Сброс параметров обучения нейронной сети
        /// </summary>
        public void Reset()
        {

        }
        /// <summary>
        /// Обновление параметров модели
        /// </summary>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="gradClip">Максимальное значение градиента по модулю</param>
        /// <param name="kG">Усиление градиента</param>
        /// <param name="L1">Коэф. L1 регуляризации</param>
        /// <param name="L2"> Regularization coefficient L2 </param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float kG)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);

                    float delt = learningRate * g / (float)Math.Sqrt(m.StepCache[i] + SmoothEpsilon);
                    m[i] -= delt;
                    m.StepCache[i] += g * g;
                    m.DifData[i] = 0;
                }

            });
        }
    }
}
