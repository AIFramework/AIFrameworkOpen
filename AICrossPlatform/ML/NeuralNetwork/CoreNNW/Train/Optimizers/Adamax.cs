using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Adamax
    /// </summary>
    [Serializable]
    public class Adamax : IOptimizer
    {
        private readonly float b1 = 0.9f;
        private readonly float b2 = 0.999f;
        private readonly float SmoothEpsilon = 1e-8f;
        private float newB1, newB2;
        private readonly int p = 10;

        /// <summary>
        /// Adamax
        /// </summary>
        public Adamax()
        {
            newB1 = b1;
            newB2 = b2;
        }

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
            float b2p = (float)Math.Pow(b2, p);
            float mt = 0, vt = 0;

            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);

                    m.StepCache[i] = (b1 * m.StepCache[i]) + ((1 - b1) * g);
                    m.StepCache2[i] = (b2p * m.StepCache2[i]) + ((1 - b2p) * (float)Math.Pow(Math.Abs(g), p));

                    mt = m.StepCache[i] / (1 - newB1);
                    vt = (float)Math.Pow(m.StepCache2[i], 1.0 / p) / (1 - newB2);

                    m[i] -= learningRate * mt / ((float)Math.Sqrt(vt + SmoothEpsilon));

                    m.DifData[i] = 0;
                }

            });

            newB1 *= b1;
            newB2 *= b2;
        }

        /// <summary>
        /// Resetting neural network training parameters
        /// </summary>
        public void Reset()
        {
            newB1 = b1;
            newB2 = b2;
        }
    }
}
