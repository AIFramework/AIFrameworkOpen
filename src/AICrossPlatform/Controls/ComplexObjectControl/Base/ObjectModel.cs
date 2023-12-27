using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.Regression;
using System;

namespace AI.Controls.ComplexObjectControl.Base
{
    /// <summary>
    /// Модель для прогнозирование реакции объекта на управляющее воздействие
    /// </summary>
    [Serializable]
    public class ObjectModel
    {
        /// <summary>
        /// Модель
        /// </summary>
        public IMultyRegression<Vector> MultyRegression;

        /// <summary>
        /// Модель для прогнозирование реакции объекта на управляющее воздействие
        /// </summary>
        public ObjectModel() { }

        /// <summary>
        /// Прогнозирование реакции объекта на управляющее воздействие
        /// </summary>
        /// <param name="action">Управляющее воздействие</param>
        /// <returns></returns>
        public virtual Vector GetReaction(Vector action)
        {
            return MultyRegression.Predict(action);
        }

        /// <summary>
        /// Обучение модели
        /// </summary>
        /// <param name="dataset"></param>
        public virtual void Train(ObjModelDataset dataset)
        {
            MultyRegression.Train(dataset.ControlActions.ToArray(), dataset.States.ToArray());
        }


        /// <summary>
        /// Создание прямой модели на базе полносвязной нейронной сети
        /// </summary>
        /// <param name="inputDim">Число входов</param>
        /// <param name="outputDim">Число выходов</param>
        /// <param name="h">Количество нейронов на скрытом слое, если 0 - то сеть не имеет скрытого слоя</param>
        /// <param name="activationH">Активация скрытого слоя</param>
        public static ObjectInversModel GetObjModelWithNNW(int inputDim, int outputDim, int h = 0, IActivation activationH = null)
        {
            ObjectInversModel objectInversModel = new ObjectInversModel();
            objectInversModel.MultyRegression = new NeuralMultyRegression(inputDim, outputDim, h, activationH);
            return objectInversModel;
        }
    }
}

