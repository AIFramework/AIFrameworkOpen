using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using System;

namespace AI.ML.Classifiers
{
    [Serializable]
    public class NeuralClassifier : BaseClassifier<NeuralClassifier>
    {
        private readonly NNW _net;
        public IGraph Graph { get; set; } = new GraphCPU(false);
        public float LearningRate { get; set; } = 0.001f;
        public int EpochesToPass { get; set; } = 100;
        public IOptimizer Optimizer { get; set; } = new Adam();
        public float ValSplit { get; set; } = 0f;

        public ILoss Loss { get; set; } = new CrossEntropyWithSoftmax();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="net"></param>
        public NeuralClassifier(NNW net)
        {
            _net = net;
        }

        /// <summary>
        /// Classify
        /// </summary>
        /// <param name="inp">Input vector</param>
        public int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }


        /// <summary>
        /// Classify
        /// </summary>
        /// <param name="inp">Input vector</param>
        public Vector ClassifyProbVector(Vector inp)
        {
            NNValue input = new NNValue(inp);
            return _net.Forward(input, Graph).ToVector();
        }

        public override void Train(Vector[] features, int[] classes)
        {
            NeuralNetworkManager manager = new NeuralNetworkManager(_net)
            {
                EpochesToPass = EpochesToPass,
                Optimizer = Optimizer,
                LearningRate = LearningRate,
                Graph = Graph,
                ValSplit = ValSplit,
                Loss = Loss
            };

            Vector[] outps = new Vector[classes.Length];
            int maxInd = _net.Layers[_net.Layers.Count - 1].OutputShape.Volume - 1;

            for (int i = 0; i < classes.Length; i++)
            {
                outps[i] = Vector.OneHotPol(classes[i], maxInd);
            }

            manager.TrainNet(features, outps);
        }
    }
}
