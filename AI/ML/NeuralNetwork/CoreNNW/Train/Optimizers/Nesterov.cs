using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Nesterov
    /// </summary>
    [Serializable]
    public class Nesterov : IOptimizer
    {
        private float Momentum { get; set; }

        private readonly float MomentumInv;

        /// <summary>
        /// Nesterov
        /// </summary>
        public Nesterov()
        {
            Momentum = 0;
            MomentumInv = 1;
        }
        /// <summary>
        /// Nesterov
        /// </summary>
        public Nesterov(float momentum)
        {
            float m = Math.Abs(momentum);
            Momentum = (m > 0.99) ? 0.99f : m;
            MomentumInv = 1 - Momentum;
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
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, kG, gradClip);

                    float delt = MomentumInv * (learningRate * g) + Momentum * m.StepCache[i];
                    m[i] -= delt;
                    m.StepCache[i] = delt;
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
