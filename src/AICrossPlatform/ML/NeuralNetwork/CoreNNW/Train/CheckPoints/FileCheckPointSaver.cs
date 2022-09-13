using AI.DataStructs;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{
    /// <summary>
    /// Сохранение чекпоинта нейронной сети в файл
    /// </summary>
    [Serializable]
    public class FileCheckPointSaver : ICheckPointSaver
    {
        /// <summary>
        /// Путь до файла
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Сохранение чекпоинта нейронной сети в файл
        /// </summary>
        public FileCheckPointSaver(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Сохранение нейросети
        /// </summary>
        /// <param name="net"></param>
        /// <param name="val"></param>
        public void Save(INetwork net, float val)
        {
            BinarySerializer.Save(FilePath, net);
        }

        /// <summary>
        /// Строковое представление
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"FileCheckPointSaver[FilePath={FilePath}]";
        }
    }
}