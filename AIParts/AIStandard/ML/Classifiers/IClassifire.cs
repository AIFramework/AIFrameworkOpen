using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;

namespace AI.ML.Classifiers
{
    /// <summary>
    /// Интерфейс для работы классификаторов
    /// </summary>
    public interface IClassifier : ISavable
    {

        void Train(Vector[] features, int[] classes);

        void Train(VectorIntDataset dataset);

        /// <summary>
        /// Распознавание
        /// </summary>
        /// <param name="inp">Вектор который надо распознать</param>
        int Classify(Vector inp);

        /// <summary>
        /// Распознавание
        /// </summary>
        /// <param name="inp">Вектор который надо распознать</param>
        Vector ClassifyProbVector(Vector inp);
    }
}
