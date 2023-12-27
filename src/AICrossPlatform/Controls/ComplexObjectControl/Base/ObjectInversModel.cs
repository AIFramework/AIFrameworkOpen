using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.Regression;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Controls.ComplexObjectControl.Base
{
    /// <summary>
    /// Модель обратного процесса, к процессу управления
    /// </summary>
    [Serializable]
    public class ObjectInversModel
    {
        /// <summary>
        /// Модель
        /// </summary>
        public IMultyRegression<Vector> MultyRegression;

        /// <summary>
        /// Модель обратного процесса, к процессу управления
        /// </summary>
        public ObjectInversModel() { }

       

        /// <summary>
        /// Получение управляющего воздействия, способного вызвать нужную реакцию
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public virtual Vector GetControl(Vector state) 
        {
            return MultyRegression.Predict(state);
        }

        /// <summary>
        /// Обучение модели
        /// </summary>
        /// <param name="dataset"></param>
        public virtual void Train(ObjModelDataset dataset) 
        {
            MultyRegression.Train(dataset.States.ToArray(), dataset.ControlActions.ToArray());
        }

        /// <summary>
        /// Создание обратной модели на базе полносвязной нейронной сети
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
