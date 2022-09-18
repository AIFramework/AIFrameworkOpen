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
    /// Нейронная сеть
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
        /// Размерность входа
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        /// <summary>
        /// Число обучаемых параметров
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
        /// Нейронная сеть
        /// </summary>
        /// <param name="seed">seed для генератора псевдо-случайных чисел</param>
        public NNW(int seed = 10)
        {
            _rnd = new Random(seed);
            Layers = new List<ILayer>();
        }
        /// <summary>
        /// Нейронная сеть
        /// </summary>
        /// <param name="layers">Список слоев</param>
        public NNW(List<ILayer> layers)
        {
            Layers = layers ?? throw new ArgumentNullException(nameof(layers));
            InputShape = layers[0].InputShape;
            OutputShape = layers[layers.Count - 1].OutputShape;
        }
        #endregion

        /// <summary>
        /// Добавление НОВОГО слоя в нейронную сеть (веса перезаписываются)
        /// </summary>
        /// <param name="layer">Слой</param>
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
        /// Добавление НОВОГО слоя в нейронную сеть (веса перезаписываются)
        /// </summary>
        /// <param name="inpShape">Форма тензора входа</param>
        /// <param name="layer">Слой</param>
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
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="graph">Граф атодифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph graph)
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
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="graph">Граф атодифференцирования</param>
        public NNValue Forward(IAlgebraicStructure<double> input, INNWGraph graph)
        {
            return Forward(new NNValue(input), graph);
        }
        /// <summary>
        /// Сброс состояния (необходим для рекуррентных нейронных сетей)
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
        /// Получение тренируемых параметров сети
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> result = new List<NNValue>();
            foreach (ILayer layer in Layers)
            {
                if (layer is ILearningLayer)
                {
                    result.AddRange((layer as ILearningLayer)!.GetParameters());
                }
            }
            return result;
        }
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
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
        /// Сохранение сети в файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Сохранение сети в поток
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        #endregion

        #region Загрузка
        /// <summary>
        /// Загрузка сети из файла
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public static NNW Load(string path)
        {
            return BinarySerializer.Load<NNW>(path);
        }
        /// <summary>
        /// Загрузка сети из потока
        /// </summary>
        /// <param name="stream">Поток</param>
        /// <returns></returns>
        public static NNW Load(Stream stream)
        {
            return BinarySerializer.Load<NNW>(stream);
        }
        #endregion

        #endregion
    }
}