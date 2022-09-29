using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork.CoreNNW.Train;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.ML.FeaturesTransforms
{
    /// <summary>
    /// Автокодировщик
    /// </summary>
    [Serializable]
    public class AutoEncoder
    {
        /// <summary>
        /// Слои кодировщика
        /// </summary>
        public List<ILayer> LayersEncoder { get; private set; } = new List<ILayer>();
        /// <summary>
        /// Слои декодера
        /// </summary>
        public List<ILayer> LayersDecoder { get; private set; } = new List<ILayer>();
        /// <summary>
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape => _inputShape;

        /// <summary>
        /// Граф автодиф
        /// </summary>
        public INNWGraph GraphAutoDif { get; set; } = new NNWGraphCPU(false);

        private NNW _net;
        private NNW _netEncoder;
        private Shape3D _inputShape;

        /// <summary>
        /// Автокодировщик
        /// </summary>
        public AutoEncoder(Shape3D inputShape, List<ILayer> layersEnc, List<ILayer> layersDecoder)
        {
            _inputShape = inputShape;
            LayersEncoder = layersEnc;
            LayersDecoder = layersDecoder;
            CreateNetAutoEnc();
        }

        /// <summary>
        /// Линейный автокодировщик
        /// </summary>
        public AutoEncoder(int inpDim, int outpDim=2)
        {
            _inputShape = new Shape3D(inpDim);
            LayersEncoder.Add(new LinearLayer(outpDim));
            LayersDecoder.Add(new LinearLayer(inpDim));
            CreateNetAutoEnc();
        }

        /// <summary>
        /// Преобразование (векторизация)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Vector Transform(IAlgebraicStructure<double> input)
        {
            NNValue nNValue = new NNValue(input);

            if (_netEncoder == null)
                throw new Exception("Обучите автокодировщик");

            return _netEncoder.Forward(nNValue, GraphAutoDif).ToVector();
        }

        /// <summary>
        /// Преобразование (векторизация)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vector[] Transform(IEnumerable<IAlgebraicStructure<double>> input)
        {
            List<Vector> result = new List<Vector>();

            foreach (var item in input)
                result.Add(Transform(item));

            return result.ToArray();
        }

        /// <summary>
        /// Преобразование
        /// </summary>
        public Matrix Transform(Matrix input)
        {
            var vects = Matrix.GetRows(input);
            var outp = Transform(vects);
            return Matrix.FromVectorsAsRows(outp);
        }

        /// <summary>
        /// Обучение автокодировщика
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="epoch">Число эпох</param>
        /// <param name="lr">Скорость обучения</param>
        /// <param name="optimizer">Оптимизатор</param>
        /// <returns></returns>
        public TrainInfo Train(IEnumerable<IAlgebraicStructure<double>> data, int epoch = 10, double lr = 0.001, IOptimizer optimizer = null)
        {
            if (optimizer == null)
                optimizer = new Adam();

            NeuralNetworkManager neuralNetworkManager = new NeuralNetworkManager(_net)
            {
                EpochesToPass = epoch,
                Graph = GraphAutoDif,
                LearningRate = (float)lr,
                Optimizer = optimizer
            };

            var info = neuralNetworkManager.TrainNet(data.ToArray(), data.ToArray());
            CreateNetEncoder();
            return info;
        }

        /// <summary>
        /// Обучение автокодировщика
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="epoch">Число эпох</param>
        /// <param name="lr">Скорость обучения</param>
        /// <param name="optimizer">Оптимизатор</param>
        /// <returns></returns>
        public TrainInfo Train(Matrix data, int epoch = 10, double lr = 0.001, IOptimizer optimizer = null)
        {
            Vector[] vectors = Matrix.GetRows(data);
            return Train(vectors, epoch, lr, optimizer);
        }


        // Создание сети
        private void CreateNetAutoEnc()
        {
            _net = new NNW();
            bool is_inp = true; // Входной ли слой

            foreach (var item in LayersEncoder)
            {
                if (is_inp)
                {
                    is_inp = false;
                    _net.AddNewLayer(_inputShape, item);
                }
                else
                {
                    _net.AddNewLayer(item);
                }
            }

            foreach (var item in LayersDecoder)
                _net.AddNewLayer(item);
        }

        // Создание сети кодировщика
        private void CreateNetEncoder()
        {
            _netEncoder = new NNW();
            _netEncoder.Layers = new List<ILayer>();
            _netEncoder.InputShape = _inputShape;

            for (int i = 0; i < LayersEncoder.Count; i++)
                _netEncoder.Layers.Add(_net.Layers[i]);
           // _netEncoder.OnlyUse();
        }
    }
}
