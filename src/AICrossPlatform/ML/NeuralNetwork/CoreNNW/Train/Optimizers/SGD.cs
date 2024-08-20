using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Стохастический градиентный спуск (SGD)
    /// </summary>
    [Serializable]
    public class SGD : IOptimizer
    {
        /// <summary>
        /// Momentum
        /// </summary>
        public float Momentum { get; set; }

        /// <summary>
        /// Стохастический градиентный спуск (SGD), moment = 0
        /// </summary>
        public SGD()
        {
            Momentum = 0;
        }

        /// <summary>
        /// Стохастический градиентный спуск (SGD)
        /// </summary>
        /// <param name="momentum">Moment</param>
        public SGD(float momentum)
        {
            float m = Math.Abs(momentum);
            Momentum = (m > 0.999) ? 0.999f : m;
        }

        /// <summary>
        /// Сброс параметров обучения
        /// </summary>
        public void Reset()
        {

        }

        /// <summary>
        /// Обновление параметров
        /// </summary>
        /// <param name="network">Нейронная сеть</param>
        /// <param name="learningRate">Скорость обучения</param>
        /// <param name="gradClip"> Максимальное значение градиента</param>
        /// <param name="gradGain">Усиление градиента (множитель)</param>
        /// <param name="L1">L1 регуляризация</param>
        /// <param name="L2">L2 регуляризация</param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float gradGain)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            _ = Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, gradGain, gradClip);


                    float delt = (learningRate * g) + (Momentum * m.StepCache[i]);
                    m[i] -= delt;
                    m.StepCache[i] = delt;
                    m.DifData[i] = 0;
                }

            });
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return $"SGD, Momentum = {Momentum}";
        }
    }
}
