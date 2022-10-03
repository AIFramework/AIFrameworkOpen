using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Adam
    /// </summary>
    [Serializable]
    public class Adam : IOptimizer
    {
        private readonly float b1;
        private readonly float b2;
        private readonly float SmoothEpsilon = 1e-8f;
        private float newB1, newB2;

        /// <summary>
        /// Adam
        /// </summary>
        public Adam(float b1 = 0.9f, float b2 = 0.999f)
        {
            this.b1 = b1;
            this.b2 = b2;
            newB1 = b1;
            newB2 = b2;
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
            List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {
                    float mt = 0, vt = 0;
                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);

                    m.StepCache[i] = (b1 * m.StepCache[i]) + ((1 - b1) * g);
                    m.StepCache2[i] = (b2 * m.StepCache2[i]) + ((1 - b2) * g * g);
                    mt = m.StepCache[i] / (1 - newB1);
                    vt = m.StepCache2[i] / (1 - newB2);

                    float datCh = learningRate * mt / ((float)Math.Sqrt(vt + SmoothEpsilon));

                    if (double.IsNaN(datCh))
                    {
                        datCh = 0;
                    }

                    m[i] -= datCh;
                    m.DifData[i] = 0;
                }
            });

            newB1 *= b1;
            newB2 *= b2;
        }

        /// <summary>
        /// Сброс параметров обучения нейронной сети
        /// </summary>
        public void Reset()
        {
            newB1 = b1;
            newB2 = b2;
        }
    }
}
