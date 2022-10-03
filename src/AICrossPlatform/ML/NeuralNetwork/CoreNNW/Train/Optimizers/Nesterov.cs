using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Метод Нестерова
    /// </summary>
    [Serializable]
    public class Nesterov : IOptimizer
    {
        private float Momentum { get; set; }

        private readonly float MomentumInv;

        /// <summary>
        /// Метод Нестерова
        /// </summary>
        public Nesterov()
        {
            Momentum = 0;
            MomentumInv = 1;
        }
        /// <summary>
        /// Метод Нестерова
        /// </summary>
        public Nesterov(float momentum)
        {
            float m = Math.Abs(momentum);
            Momentum = (m > 0.99) ? 0.99f : m;
            MomentumInv = 1 - Momentum;
        }


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

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);

                    float delt = (MomentumInv * (learningRate * g)) + (Momentum * m.StepCache[i]);
                    m[i] -= delt;
                    m.StepCache[i] = delt;
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
