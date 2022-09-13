using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Обучаемый БИХ-фильтр
    /// </summary>
    [Serializable]
    public class FilterCell : IActivatableLayer, ILearningLayer, IRecurrentLayer, IRandomizableLayer
    {
        public NNValue para, inputs, outputs, bias, outp;
        private readonly int _aLen, _bLen;

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
        public int TrainableParameters => _aLen + _bLen + 1;
        /// <summary>
        /// Добавление для расчета весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
        /// </summary>
        public IActivation ActivationFunction { get; set; } = new TanhUnit();
        /// <summary>
        /// Random initialization for the layer
        /// </summary>
        public Random Random
        {
            set => para = NNValue.Random(_aLen + _bLen, 1, 1.0 / (_aLen * _aLen), value);
        }
        /// <summary>
        /// a - Коэффициенты фильтра
        /// </summary>
        public Vector A
        {
            get
            {
                Vector coef = new Vector(_aLen);

                for (int i = 0; i < _aLen; i++)
                {
                    coef[i] = para[i + _bLen];
                }

                return coef;
            }

            set
            {
                for (int i = 0; i < _aLen; i++)
                {
                    para[i + _bLen] = (float)value[i];
                }
            }
        }
        /// <summary>
        /// b - Коэффициенты фильтра
        /// </summary>
        public Vector B
        {
            get
            {
                Vector coef = new Vector(_bLen);

                for (int i = 0; i < _bLen; i++)
                {
                    coef[i] = para[i];
                }

                return coef;
            }

            set
            {
                for (int i = 0; i < _bLen; i++)
                {
                    para[i] = (float)value[i];
                }
            }
        }

        /// <summary>
        /// Обучаемый нейрофильтр
        /// </summary>
        /// <param name="aL">Коэф. а</param>
        /// <param name="bL">Коэф. б</param>
        public FilterCell(int aL = 12, int bL = 13)
        {
            _aLen = aL;
            _bLen = bL;
            InputShape = new Shape3D(1);
            OutputShape = new Shape3D(1);
            bias = new NNValue(1);
            ResetState();
        }
        /// <summary>
        /// Обучаемый нейрофильтр
        /// </summary>
        /// <param name="aL">Коэф. а</param>
        /// <param name="bL">Коэф. б</param>
        /// <param name="random">Random</param>
        public FilterCell(int aL, int bL, Random random) : this(aL, bL)
        {
            Random = random;
        }

        /// <summary>
        /// Прямой проход (фильтрация)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            inputs = g.AddCicleBuff(inputs, input, _bLen);

            outp = g.Add(
                g.ScalarProduct(para,
                g.ConcatinateVectors(inputs, g.Invers(outputs))),
                bias);

            outputs = g.AddCicleBuff(outputs, outp, _aLen);
            return g.Activate(ActivationFunction, outp);
        }
        /// <summary>
        /// Получение параметров
        /// </summary>
        /// <returns></returns>
        public List<NNValue> GetParameters()
        {
            List<NNValue> param = new List<NNValue>
            {
                para,
                bias
            };
            return param;
        }
        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
        }
        /// <summary>
        /// Resetting the state of the neural network layer
        /// </summary>
        public void ResetState()
        {
            inputs = new NNValue(_bLen);
            outputs = new NNValue(_aLen);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }

        public void OnlyUse()
        {
            para.OnlyUse();
            inputs.OnlyUse();
            outputs.OnlyUse();
            bias.OnlyUse();
        }
    }
}