using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Adadelta
    /// </summary>
    [Serializable]
    public class Adadelta : IOptimizer
    {
        private readonly float DecayRate = 0.999f;
        private readonly float SmoothEpsilon = 1e-8f;
        private readonly float SmoothEpsilon2 = 1e-4f;
        /// <summary>
        /// Resetting neural network training parameters
        /// </summary>
        public void Reset()
        {

        }
        /// <summary>
        /// Updating model parameters 
        /// </summary>
        /// <param name="network"> Neural network </param>
        /// <param name="learningRate">Learning rate</param>
        /// <param name="gradClip"> Maximum gradient value </param>
        /// <param name=" gradG"> Gain of gradients </param>
        /// <param name="L1"> Regularization coefficient L1 </param>
        /// <param name="L2"> Regularization coefficient L2 </param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float gradG)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {
                    float g = BaseMethods.GradCulc(m, i, L1, L2, gradG, gradClip);

                    m.StepCache2[i] = (m.StepCache2[i] == 0) ? SmoothEpsilon2 : m.StepCache2[i];

                    m.StepCache[i] = (m.StepCache[i] * DecayRate) + ((1 - DecayRate) * g * g);
                    float delta = (float)(g * Math.Sqrt(m.StepCache2[i] + SmoothEpsilon) / Math.Sqrt(m.StepCache[i] + SmoothEpsilon));
                    m.StepCache2[i] = (m.StepCache2[i] * DecayRate) + ((1 - DecayRate) * delta * delta);

                    m[i] -= learningRate * delta;
                    m.DifData[i] = 0;
                }
            });
        }

    }
}
