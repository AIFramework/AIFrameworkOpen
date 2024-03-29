﻿using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Models;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers.Base
{
    /// <summary>
    /// Интерфейс слоя нейронной сети
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// Размерность входа
        /// </summary>
        Shape3D InputShape { get; set; }
        /// <summary>
        /// Размерность выхода
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        int TrainableParameters { get; }
        /// <summary>
        /// Добавление значения в знаменатель под корень при инициализации весов
        /// </summary>
        double AddDenInSqrt { get; }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        NNValue Forward(NNValue input, INNWGraph g);
        /// <summary>
        /// Только использование, удаляются все кэши и производные, сеть становится, примерно, в 4 раза легче
        /// </summary>
        void OnlyUse();
    }
}