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
        /// Smoothing factor
        /// </summary>
        public float SmoothEpsilon = 1e-8f;

        /// <summary>
        /// Updating model parameters 
        /// </summary>
        /// <param name="network"> Neural network </param>
        /// <param name="learningRate">Learning rate</param>
        /// <param name="gradClip"> Maximum gradient value </param>
        /// <param name="kG"> Gain of gradients </param>
        /// <param name="L1"> Regularization coefficient L1 </param>
        /// <param name="L2"> Regularization coefficient L1 </param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float kG)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);


                    m.StepCache[i] = m.StepCache[i] * DecayRate + (1 - DecayRate) * g * g;

                    m[i] -= learningRate * g / (float)Math.Sqrt(m.StepCache[i] + SmoothEpsilon);
                    m.DifData[i] = 0;
                }

            });
        }

        /// <summary>
        /// Resetting neural network training parameters
        /// </summary>
        public void Reset()
        {
        }
    }
}
