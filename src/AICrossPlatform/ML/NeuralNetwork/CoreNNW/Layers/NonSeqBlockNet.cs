using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Непоследовательный блок
    /// </summary>
    [Serializable]
    public abstract class NonSeqBlockNet : IActivatableLayer, ILearningLayer, IRecurrentLayer
    {
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
                int sum = 0;
                foreach (var layer in _layers)
                    if (layer is ILearningLayer)
                        sum += (layer as ILearningLayer)!.TrainableParameters;

                return sum;
            }
        }
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; }

        private readonly List<ILayer> _layers;

        /// <summary>
        /// Непоследовательный блок
        /// </summary>
        public NonSeqBlockNet() { }

        /// <summary>
        /// Непоследовательный блок
        /// </summary>
        public NonSeqBlockNet(Shape3D inputDimension, Shape3D outputDimension, List<ILayer> layers)
        {
            InputShape = inputDimension;
            OutputShape = outputDimension;
            _layers = layers;
            ResetState();
        }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public abstract NNValue Forward(NNValue input, INNWGraph g);

        /// <summary>
        /// Сброс состояния слоя нейронной сети
        /// </summary>
        public void ResetState()
        {
            foreach (var layer in _layers)
            {
                if (layer is IRecurrentLayer)
                    (layer as IRecurrentLayer)!.ResetState();
            }
        }
        /// <summary>
        /// Получение обучаемых параметров
        /// </summary>
        public List<NNValue> GetParameters()
        {
            List<NNValue> block_params = new List<NNValue>();

            foreach (var layer in _layers)
                if (layer is ILearningLayer)
                    block_params.AddRange(
                        (layer as ILearningLayer)!.GetParameters());

            return block_params;
        }
        /// <summary>
        /// Генерация весовых коэфициентов
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        public void InitWeights(Random random)
        {
            foreach (var layer in _layers)
                if (layer is ILearningLayer)
                    (layer as ILearningLayer)!.InitWeights(random);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }

        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        public void OnlyUse()
        {
            foreach (var layer in _layers)
            {
                layer.OnlyUse();
            }
        }
    }
}
