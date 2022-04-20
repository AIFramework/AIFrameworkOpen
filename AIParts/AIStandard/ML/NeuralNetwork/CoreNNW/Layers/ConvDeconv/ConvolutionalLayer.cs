using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Convolutional layer of neural network
    /// </summary>
    [Serializable]
    public class ConvolutionalLayer : IActivatableLayer, ILearningLayer
    {
        #region Поля и свойства
        private int _strideX = 1, _strideY = 1;
        private int _padX = 0, _padY = 0;

        /// <summary>
        /// Filter structure
        /// </summary>
        public FilterStruct FilterStrucuture;
        /// <summary>
        /// X-axis stride
        /// </summary>
        public int StrideX
        {
            get => _strideX;
            set
            {
                _strideX = value;
                RestOutShape();
            }
        }
        /// <summary>
        /// Y-axis stride
        /// </summary>
        public int StrideY
        {
            get => _strideY;
            set
            {
                _strideY = value;
                RestOutShape();
            }
        }
        /// <summary>
        /// Padding X-axis
        /// </summary>
        public int PaddingX
        {
            get => _padX;
            set
            {
                _padX = (value > Filters[0].Shape.Width - 1) ? Filters[0].Shape.Width - 1 : value;
                RestOutShape();
            }
        }
        /// <summary>
        /// Padding Y-axis
        /// </summary>
        public int PaddingY
        {
            get => _padY;
            set
            {
                _padY = (value > Filters[0].Shape.Height - 1) ? Filters[0].Shape.Height - 1 : value;
                RestOutShape();
            }
        }
        /// <summary>
        /// Whether the dimension of the input is preserved
        /// </summary>
        public bool IsSame
        {
            get => (_padY == Filters[0].Shape.Height - 1) && (_padX == Filters[0].Shape.Width - 1);
            set
            {
                if (value)
                {
                    _padY = Filters[0].Shape.Height - 1;
                    _padX = Filters[0].Shape.Width - 1;
                }
                else
                {
                    _padX = 0; _padY = 0;
                }
            }
        }
        /// <summary>
        /// Number of learning parameters
        /// </summary>
        public int TrainableParameters => FilterStrucuture.FilterCount * FilterStrucuture.FilterH * FilterStrucuture.FilterW * InputShape.Depth;
        /// <summary>
        /// Initializer numerator
        /// </summary>
        public double Numerator { get; set; }
        /// <summary>
        /// Adding a value to the denominator under the root when initializing the weights
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Activation function
        /// </summary>
        public IActivation ActivationFunction { get; set; }
        /// <summary>
        /// Offset weights
        /// </summary>
        public NNValue Bias { get; private set; }
        /// <summary>
        /// Filter tensors
        /// </summary>
        public NNValue[] Filters { get; /*private TODO*/set; }
        /// <summary>
        /// Dimension and shape of the input tensor
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Output dimension
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        #endregion

        /// <summary>
        /// Convolutional layer
        /// </summary>
        /// <param name="inputShape">Dimension and shape of the input tensor</param>
        /// <param name="filterStruct">Структура фильтров</param>
        /// <param name="func">Activation function</param>
        /// <param name="rnd">Pseudo-random number generator</param>
        public ConvolutionalLayer(Shape3D inputShape, FilterStruct filterStruct, IActivation func, Random rnd)
        {
            InputShape = inputShape;
            Init(filterStruct, func, rnd);
        }
        /// <summary>
        /// Convolutional layer
        /// </summary>
        /// <param name="filterStruct">Структура фильтров</param>
        /// <param name="func">Activation function</param>
        public ConvolutionalLayer(FilterStruct filterStruct, IActivation func)
        {
            ActivationFunction = func;
            FilterStrucuture = filterStruct;
            Filters = new NNValue[FilterStrucuture.FilterCount];

            for (int i = 0; i < FilterStrucuture.FilterCount; i++)
            {
                Filters[i] = new NNValue(FilterStrucuture.FilterH, FilterStrucuture.FilterW);
            }
        }
        /// <summary>
        /// Convolutional layer
        /// </summary>
        /// <param name="func">Activation function</param>
        /// <param name="filterCount">Число фильтров</param>
        /// <param name="height">Высота фильтра</param>
        /// <param name="width">Ширина фильтра</param>
        public ConvolutionalLayer(IActivation func = null, int filterCount = 3, int height = 3, int width = 3)
        {
            Filters = new NNValue[filterCount];

            for (int i = 0; i < filterCount; i++)
            {
                Filters[i] = new NNValue(height, width);
            }

            ActivationFunction = func ?? new LinearUnit();

            FilterStrucuture = new FilterStruct() { FilterW = width, FilterCount = filterCount, FilterH = height };
        }

        internal void Init(FilterStruct filterStruct, IActivation func, Random rnd)
        {
            FilterStrucuture = filterStruct;
            int outputH = InputShape.Height - filterStruct.FilterH + 1 + _padY,
               outputW = InputShape.Width - filterStruct.FilterW + 1 + _padX;

            Bias = new NNValue(filterStruct.FilterCount);

            OutputShape = new Shape3D(outputH, outputW, filterStruct.FilterCount);

            if (OutputShape.Height < 0 || OutputShape.Width < 0)
            {
                throw new InvalidOperationException($"Negative dimention: (H = {OutputShape.Height},\tW = {OutputShape.Width})");
            }

            ActivationFunction = func;
            Filters = new NNValue[filterStruct.FilterCount];

            Numerator = (func is ReLU) ? 2 : 1; // ReLu / Lin

            GeneratorW(rnd);

            AddDenInSqrt = Filters[0].Shape.Count;
        }
        /// <summary>
        /// Setting up the random number generator initializer
        /// </summary>
        /// <param name="random">Pseudo-random number generator</param>
        /// <param name="addDenumInSqrt">Adding a value to a radical expression in the denominator</param>
        public void GeneratorW(Random random, double addDenumInSqrt = 0)
        {
            double initParamsStdDev = Numerator / Math.Sqrt(FilterStrucuture.FilterH * FilterStrucuture.FilterW * InputShape.Depth + addDenumInSqrt);

            for (int i = 0; i < FilterStrucuture.FilterCount; i++)
            {
                Filters[i] = NNValue.RandomR(FilterStrucuture.FilterH, FilterStrucuture.FilterW, InputShape.Depth, initParamsStdDev, random);
            }
        }
        /// <summary>
        /// Direct network pass
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="g">Automatic differentiation graph</param>
        public NNValue Forward(NNValue input, IGraph g)
        {
            NNValue output = g.Convolution(input, Filters, Bias, _padX, _padY, _strideX, _strideY);
            output = g.Activate(ActivationFunction, output);
            return output;
        }
        /// <summary>
        /// Initialize layer weights
        /// </summary>
        /// <param name="random"></param>
        public void InitWeights(Random random)
        {
            Init(FilterStrucuture, ActivationFunction, random);
        }
        /// <summary>
        /// Обновление размера выхода
        /// </summary>
        private void RestOutShape()
        {
            int outpH = (InputShape.Height - Filters[0].Shape.Height + _padY) / _strideY + 1;
            int outpW = (InputShape.Width - Filters[0].Shape.Width + _padX) / _strideX + 1;
            OutputShape = new Shape3D(outpH, outpW, OutputShape.Depth);
        }
        /// <summary>
        /// Layer description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Getting trained parameters
        /// </summary>
        /// <returns></returns>
        public List<NNValue> GetParameters()
        {
            List<NNValue> values = new List<NNValue>();
            values.AddRange(Filters);
            values.Add(Bias);
            return values;
        }

        public void OnlyUse()
        {
            foreach (NNValue item in Filters)
            {
                item.OnlyUse();
            }

            Bias.OnlyUse();
        }
    }
}