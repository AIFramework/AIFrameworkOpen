using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Сверточный слой
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
        /// Число обучаемых параметров
        /// </summary>
        public int TrainableParameters => FilterStrucuture.FilterCount * FilterStrucuture.FilterH * FilterStrucuture.FilterW * InputShape.Depth;
        /// <summary>
        /// Числитель генератора случайных чисел
        /// </summary>
        public double Numerator { get; set; }
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        public double AddDenInSqrt { get; set; }
        /// <summary>
        /// Активационная функция
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
        /// Размерность и форма входного тензора
        /// </summary>
        public Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        public Shape3D OutputShape { get; private set; }
        #endregion

        /// <summary>
        /// Сверточный слой
        /// </summary>
        /// <param name="inputShape"> Размерность и форма входного тензора </param>
        /// <param name="filterStruct">Структура фильтров</param>
        /// <param name="func">Активационная функция</param>
        /// <param name="rnd">Генератор псевдо-случайных чисел</param>
        public ConvolutionalLayer(Shape3D inputShape, FilterStruct filterStruct, IActivation func, Random rnd)
        {
            InputShape = inputShape;
            Init(filterStruct, func, rnd);
        }
        /// <summary>
        /// Сверточный слой
        /// </summary>
        /// <param name="filterStruct">Структура фильтров</param>
        /// <param name="func">Активационная функция</param>
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
        /// Сверточный слой
        /// </summary>
        /// <param name="func">Активационная функция</param>
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
        /// Генерация весовых коэффициентов
        /// </summary>
        /// <param name="random">Генератор псевдо-случайных чисел</param>
        /// <param name="addDenumInSqrt">Добавление значения под корень знаменателя</param>
        public void GeneratorW(Random random, double addDenumInSqrt = 0)
        {
            double initParamsStdDev = Numerator / Math.Sqrt((FilterStrucuture.FilterH * FilterStrucuture.FilterW * InputShape.Depth) + addDenumInSqrt);

            for (int i = 0; i < FilterStrucuture.FilterCount; i++)
            {
                Filters[i] = NNValue.RandomR(FilterStrucuture.FilterH, FilterStrucuture.FilterW, InputShape.Depth, initParamsStdDev, random);
            }
        }
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public NNValue Forward(NNValue input, INNWGraph g)
        {
            NNValue output = g.Convolution(input, Filters, Bias, _padX, _padY, _strideX, _strideY);
            output = g.Activate(ActivationFunction, output);
            return output;
        }
        /// <summary>
        /// Инициализация слоя
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
            int outpH = ((InputShape.Height - Filters[0].Shape.Height + _padY) / _strideY) + 1;
            int outpW = ((InputShape.Width - Filters[0].Shape.Width + _padX) / _strideX) + 1;
            OutputShape = new Shape3D(outpH, outpW, OutputShape.Depth);
        }
        /// <summary>
        /// Описание слоя
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return LayerHelper.GetLayerDescription(GetType().Name, InputShape, OutputShape, ActivationFunction, TrainableParameters);
        }
        /// <summary>
        /// Возвращает обучаемые параметры
        /// </summary>
        /// <returns></returns>
        public List<NNValue> GetParameters()
        {
            List<NNValue> values = new List<NNValue>();
            values.AddRange(Filters);
            values.Add(Bias);
            return values;
        }

        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
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