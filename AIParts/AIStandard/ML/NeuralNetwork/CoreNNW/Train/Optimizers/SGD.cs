using System;
using System.Threading.Tasks;

namespace AI.ML.NeuralNetwork.CoreNNW.Optimizers
{
    /// <summary>
    /// Stochastic gradient descent
    /// </summary>
    [Serializable]
    public class SGD : IOptimizer
    {
        /// <summary>
        /// Momentum
        /// </summary>
        public float Momentum { get; set; }

        /// <summary>
        /// Stochastic gradient descent, moment = 0
        /// </summary>
        public SGD()
        {
            Momentum = 0;
        }

        /// <summary>
        /// Stochastic gradient descent
        /// </summary>
        /// <param name="momentum">Moment</param>
        public SGD(float momentum)
        {
            float m = Math.Abs(momentum);
            Momentum = (m > 0.999) ? 0.999f : m;
        }

        /// <summary>
        /// Resetting Teaching Parameters
        /// </summary>
        public void Reset()
        {

        }

        /// <summary>
        /// Updating parameters 
        /// </summary>
        /// <param name="network"> Neural network</param>
        /// <param name="learningRate"> Learning rate</param>
        /// <param name="gradClip"> Maximum gradient value</param>
        /// <param name="gradGain">Gradient enhancement factor</param>
        /// <param name="L1">L1 regularization</param>
        /// <param name="L2">L2 regularization</param>
        public void UpdateModelParams(INetwork network, float learningRate, float gradClip, float L1, float L2, float gradGain)
        {
            System.Collections.Generic.List<NNValue> paramss = network.GetParameters();

            Parallel.ForEach(paramss, m =>
            {
                for (int i = 0; i < m.Shape.Count; i++)
                {

                    float g = BaseMethods.GradCulc(m, i, L1, L2, gradGain, gradClip);


                    float delt = learningRate * g + Momentum * m.StepCache[i];
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
