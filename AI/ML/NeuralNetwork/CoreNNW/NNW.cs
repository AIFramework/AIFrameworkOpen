using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AI.ML.NeuralNetwork.CoreNNW
{
    /// <summary>
    /// Neural network
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Input shape = {InputShape?.ToString(),nq}, Output shape = {OutputShape?.ToString(),nq}")]
    public class NNW : INetwork
    {
        #region Поля и свойства
        private readonly Random _rnd;

        /// <summary>
        /// List of network layers
        /// </summary>
        public List<ILayer> Layers { get; set; }
        /// <summary>
        /// Input dimension
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters
        {
            get
            {
                int trpar = 0;

                for (int i = 0; i < Layers.Count; i++)
                {
                    trpar += Layers[i].TrainableParameters;
                }

                return trpar;
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Neural network
        /// </summary>
        /// <param name="seed">Random number generator seed</param>
        public NNW(int seed = 10)
        {
            _rnd = new Random(seed);
            Layers = new List<ILayer>();
        }
        /// <summary>
        /// Neural network
        /// </summary>
        /// <param name="layers">List of layers</param>
        public NNW(List<ILayer> layers)
        {
            Layers = layers ?? throw new ArgumentNullException(nameof(layers));
            InputShape = layers[0].InputShape;
            OutputShape = layers[layers.Count - 1].OutputShape;
        }
        #endregion

        /// <summary>
        /// Adding a NEW layer to the neural network
        /// </summary>
        /// <param name="layer">Layer</param>
        public void AddNewLayer(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            Shape3D shape = Layers[Layers.Count - 1].OutputShape;
            layer.InputShape = shape;
            if (layer is IRandomizableLayer randomizableLayer)
            {
                randomizableLayer.Random = _rnd;
            }

            if (layer is ILearningLayer learningLayer)
            {
                learningLayer.InitWeights(_rnd);
            }

            OutputShape = layer.OutputShape;
            Layers.Add(layer);

            if (Layers.Count == 1)
            {
                InputShape = Layers[0].InputShape;
            }

            ILayer oldL = Layers[Layers.Count - 2];

            if (oldL is ConvolutionalLayer convolutionLayer)
            {
                if (layer is IActivatableLayer activatableLayer && activatableLayer.ActivationFunction != null)
                {
                    convolutionLayer.Numerator += activatableLayer.ActivationFunction.Numerator;
                    convolutionLayer.Numerator /= 2.0;
                }
                convolutionLayer.GeneratorW(_rnd, layer.AddDenInSqrt);
                Layers[Layers.Count - 2] = convolutionLayer;
            }
        }
        /// <summary>
        /// Adding a NEW layer to the neural network
        /// </summary>
        /// <param name="inpShape">Input tensor shape</param>
        /// <param name="layer">Layer</param>
        public void AddNewLayer(Shape3D inpShape, ILayer layer)
        {
            layer.InputShape = inpShape ?? throw new ArgumentNullException(nameof(inpShape));

            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            if (layer is ILearningLayer learningLayer)
            {
                learningLayer.InitWeights(_rnd);
            }

            OutputShape = layer.OutputShape;

            Layers.Add(layer);

            if (Layers.Count == 1)
            {
                InputShape = inpShape;
            }
        }
        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="graph">Graph of automatic differentiation</param>
        public NNValue Forward(NNValue input, IGraph graph)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (input.Shape != InputShape)
            {
                throw new ArgumentException("Inpud data shape doesn't match network input shape", nameof(input));
            }

            NNValue prev = input;

            foreach (ILayer layer in Layers)
            {
                prev = layer.Forward(prev, graph);
            }

            return prev;
        }
        /// <summary>
        /// Forward pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="graph"></param>
        /// <returns>Graph of automatic differentiation</returns>
        public NNValue Forward(IAlgebraicStructure input, IGraph graph)
        {
            return Forward(new NNValue(input), graph);
        }
        /// <summary>
        /// Resetting the state (necessary for recurrent neural networks)
        /// </summary>
        public void ResetState()
        {
            foreach (ILayer layer in Layers)
            {
                if (layer is IRecurrentLayer recurrentLayer)
                {
                    recurrentLayer.ResetState();
                }
            }
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>();
            foreach (ILayer layer in Layers)
            {
                if (layer is ILearningLayer)
                {
                    result.AddRange((layer as ILearningLayer).GetParameters());
                }
            }
            return result;
        }
        /// <summary>
        /// Use only mode, all additional parameters are deleted
        /// </summary>
        public void OnlyUse()
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                Layers[i].OnlyUse();
            }
        }

        #region Технические методы
        #pragma warning disable CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Layers.Count; i++)
            {
                sb.AppendLine(Layers[i].ToString());
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("Input shape: {0} | Output shape: {1} | Trainable parameters: {2}", InputShape, OutputShape, TrainableParameters);

            return sb.ToString();
        }

        #pragma warning restore CS1591 // Отсутствует комментарий XML для открытого видимого типа или члена
        #endregion

        #region Сериализация

        #region Сохранение
        /// <summary>
        /// Save network to file
        /// </summary>
        /// <param name="path">File path</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save network to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Load network from file
        /// </summary>
        /// <param name="path">File path</param>
        public static NNW Load(string path)
        {
            return BinarySerializer.Load<NNW>(path);
        }
        /// <summary>
        /// Load network from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static NNW Load(Stream stream)
        {
            return BinarySerializer.Load<NNW>(stream);
        }
        #endregion

        #endregion
    }
}