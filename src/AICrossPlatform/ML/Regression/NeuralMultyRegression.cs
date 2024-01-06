using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW.Loss;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using AI.ML.NeuralNetwork.CoreNNW.Optimizers;
using AI.ML.NeuralNetwork.CoreNNW;
using System;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork;
using AI.DataStructs.Shapes;
using AI.Statistics;
using AI.DataStructs;

namespace AI.ML.Regression
{
    /// <summary>
    /// Нейросетевая множественная регрессия
    /// </summary>
    [Serializable]
    public class NeuralMultyRegression : IMultyRegression<Vector>
    {
        private Random _rnd = new Random();
        private readonly NNW _net;

        /// <summary>
        /// Средний вектор входа
        /// </summary>
        public Vector MeanInp { get; set; }

        /// <summary>
        /// Разброс значений на входе
        /// </summary>
        public Vector StdInp { get; set; }

        /// <summary>
        /// Средний вектор выхода
        /// </summary>
        public Vector MeanOutp { get; set; }

        /// <summary>
        /// Разброс значений на выходе
        /// </summary>
        public Vector StdOutp { get; set; }


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
        public int EpochesToPass { get; set; } = 20;
        /// <summary>
        /// Оптимизатор
        /// </summary>
        public IOptimizer Optimizer { get; set; } = new RMSProp();
        /// <summary>
        /// Доля валидационной выборки
        /// </summary>
        public float ValSplit { get; set; } = 0f;
        /// <summary>
        /// Метод измерения ошибки
        /// </summary>
        public ILoss Loss { get; set; } = new LossMSE();

        /// <summary>
        /// Регрессия для многих переменных на базе нейронной сети
        /// </summary>
        /// <param name="net">Нейронная сеть</param>
        public NeuralMultyRegression(NNW net)
        {
            _net = net;
        }

        /// <summary>
        /// Регрессия для многих переменных на базе нейронной сети
        /// </summary>
        /// <param name="inputDim">Число входов</param>
        /// <param name="outputDim">Число выходов</param>
        /// <param name="h">Количество нейронов на скрытом слое, если 0 - то сеть не имеет скрытого слоя</param>
        /// <param name="activationH">Активация скрытого слоя</param>
        public NeuralMultyRegression(int inputDim, int outputDim, int h = 0, IActivation activationH = null) 
        {
            _net = new NNW();

            if (h == 0)
                _net.AddNewLayer(new Shape3D(inputDim), new FeedForwardLayer(outputDim, new LinearUnit()));
            else
            {
                if(activationH == null)
                    _net.AddNewLayer(new Shape3D(inputDim), new FeedForwardLayer(h, new ReLU(0.1)));
                else
                    _net.AddNewLayer(new Shape3D(inputDim), new FeedForwardLayer(h, activationH));
                _net.AddNewLayer(new FeedForwardLayer(outputDim, new LinearUnit()));
            }

            MeanInp = new Vector(inputDim);
            MeanOutp = new Vector(outputDim);
            StdInp = new Vector(inputDim) + 1;
            StdOutp = new Vector(outputDim) + 1;
        }

        /// <summary>
        /// Прогнозирование
        /// </summary>
        /// <param name="data">Входные данные</param>
        /// <returns></returns>
        public Vector Predict(Vector data)
        {
            NNValue input = new NNValue(Scale(data, MeanInp, StdInp));
            Vector outp = _net.Forward(input, Graph).ToVector();
            return DeScale(outp, MeanOutp, StdOutp);
        }

        private Vector Scale(Vector data, Vector m, Vector std)
        {
            return (data - m) / std ;
        }

        private Vector DeScale(Vector data, Vector m, Vector std)
        {
            return data * std + m;
        }


        /// <summary>
        /// Обучение регрессионной модели
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targets"></param>
        public void Train(Vector[] data, Vector[] targets)
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

            MeanInp = Statistic.MeanVector(data);
            MeanOutp = Statistic.MeanVector(targets);
            StdInp = Statistic.EnsembleStd(data) + double.Epsilon;
            StdOutp = Statistic.EnsembleStd(targets) + double.Epsilon;

            Vector[] inp = new Vector[data.Length];
            Vector[] outp = new Vector[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                inp[i] = Scale(data[i], MeanInp, StdInp);
                outp[i] = Scale(targets[i], MeanOutp, StdOutp);
            }


            manager.TrainNet(inp, outp);
        }

        /// <summary>
        /// Сохранение в бинарный файл
        /// </summary>
        public void Save(string path) 
        {
            BinarySerializer.Save(path, this);
        }

        /// <summary>
        /// Загрузка из бинарного файла
        /// </summary>
        public static NeuralMultyRegression LoadFromFile(string path) 
        {
            return BinarySerializer.Load<NeuralMultyRegression>(path);
        }
    }
}
