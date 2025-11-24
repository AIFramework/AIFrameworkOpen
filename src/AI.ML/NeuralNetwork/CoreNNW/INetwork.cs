using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork.CoreNNW.Layers.Base;
using AI.ML.NeuralNetwork.CoreNNW.Models;
using System.Collections.Generic;

namespace AI.ML.NeuralNetwork.CoreNNW
{
    /// <summary>
    /// Интерфейс нейронной сети
    /// </summary>
    public interface INetwork : ISavable
    {
        /// <summary>
        /// Форма входа
        /// </summary>
        Shape3D InputShape { get; set; }
        /// <summary>
        /// Форма выхода
        /// </summary>
        Shape3D OutputShape { get; }
        /// <summary>
        /// Число обучаемых параметров
        /// </summary>
        int TrainableParameters { get; }
        /// <summary>
        /// Список слоев
        /// </summary>
        List<ILayer> Layers { get; set; }

        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        /// <returns></returns>
        NNValue Forward(NNValue input, INNWGraph g);
        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Входные данные</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        /// <returns></returns>
        NNValue Forward(IAlgebraicStructure<double> input, INNWGraph g);
        /// <summary>
        /// Сброс состояния рекуррентной сети
        /// </summary>
        void ResetState();
        /// <summary>
        /// Обучаемые параметры
        /// </summary>
        /// <returns></returns>
        List<NNValue> GetParameters();
        /// <summary>
        /// Добавление нового слоя в НС
        /// </summary>
        /// <param name="layer">Новый слой</param>
        void AddNewLayer(ILayer layer);
        /// <summary>
        /// Добавление нового слоя в НС
        /// </summary>
        /// <param name="inputShape">Форма входа</param>
        /// <param name="layer">Новый слой</param>
        void AddNewLayer(Shape3D inputShape, ILayer layer);

        /// <summary>
        /// Только для использования (удаляет кэши важные для обучения, делает сеть в 4раза меньше)
        /// </summary>
        void OnlyUse();
    }
}