using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using System;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Классификатор на базе нейронной сети
    /// </summary>
    [Serializable]
    public class NeuralClassifier : BaseClassifier<NeuralClassifier>
    {
        private readonly NNW _net;
        /// <summary>
        /// Граф автоматического дифференцирования
        /// </summary>
        public INNWGraph Graph { get; set; } = new NNWGraphCPU(false);
        /// <summary>
        /// Скорость обучения
        /// </summary>
        public float LearningRate { get; set; } = 0.001f;
        /// <summary>
        /// Число эпох
        /// </summary>
        public int EpochesToPass { get; set; } = 100;
        /// <summary>
        /// Оптимизатор
        /// </summary>
        public IOptimizer Optimizer { get; set; } = new Adam();
        /// <summary>
        /// Доля валидационной выборки
        /// </summary>
        public float ValSplit { get; set; } = 0f;
        /// <summary>
        /// Метод измерения ошибки
        /// </summary>
        public ILoss Loss { get; set; } = new CrossEntropyWithSoftmax();

        /// <summary>
        /// Классификатор на базе нейронной сети
        /// </summary>
        /// <param name="net">Нейронная сеть</param>
        public NeuralClassifier(NNW net)
        {
            _net = net;
        }

        /// <summary>
        /// Распознать
        /// </summary>
        /// <param name="inp">Вектор входа</param>
        public override int Classify(Vector inp)
        {
            return ClassifyProbVector(inp).MaxElementIndex();
        }


        /// <summary>
        /// Распознать
        /// </summary>
        /// <param name="inp">Вектор входа</param>
        public override Vector ClassifyProbVector(Vector inp)
        {
            NNValue input = new NNValue(inp);
            return _net.Forward(input, Graph).ToVector();
        }

        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="features"></param>
        /// <param name="classes"></param>
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

            _ = manager.TrainNet(features, outps);
        }
    }
}
