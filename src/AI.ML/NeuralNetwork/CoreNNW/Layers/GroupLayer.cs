using AI.ML.NeuralNetwork.CoreNNW.Models;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Layers
{
    /// <summary>
    /// Слой для работы с группами входов
    /// </summary>
    [Serializable]
    public class GroupLayer : NonSeqBlockNet
    {


        /// <summary>
        /// Слой для работы с группами входов
        /// </summary>
        /// <param name="countGroupe">Число групп</param>
        /// <param name="outpEachGroupe">Число выходов в каждой группе</param>
        public GroupLayer(int countGroupe, int outpEachGroupe)
        {

        }


        /// <summary>
        /// Прямой проход
        /// </summary>
        /// <param name="input">Вход</param>
        /// <param name="g">Граф автоматического дифференцирования</param>
        public override NNValue Forward(NNValue input, INNWGraph g)
        {
            throw new NotImplementedException();
        }
    }
}
