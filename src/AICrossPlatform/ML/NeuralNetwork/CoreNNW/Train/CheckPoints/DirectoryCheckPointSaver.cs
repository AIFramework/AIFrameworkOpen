using System;
using System.IO;

namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{

    /// <summary>
    /// Директория для сохранения чекпоинтов нейросети
    /// </summary>
    [Serializable]
    public class DirectoryCheckPointSaver : ICheckPointSaver
    {
        /// <summary>
        /// Путь до директории
        /// </summary>
        public string DirectoryPath { get; set; } = "";

        /// <summary>
        /// Директория для сохранения чекпоинтов нейросети
        /// </summary>
        public DirectoryCheckPointSaver(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        /// <summary>
        /// Сохранение чекпоинта в папку
        /// </summary>
        /// <param name="net"></param>
        /// <param name="val"></param>
        public void Save(INetwork net, float val)
        {
            string fPath = Path.Combine(DirectoryPath, $"checkpoint - [{DateTime.Now:dd.MM.yyyy HH'h'mm'm'}], val - {val.ToString(AISettings.GetProvider())}.nn");
            net.Save(fPath);
        }

        /// <summary>
        /// Перевод в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"DirectoryCheckPointSaver[DirectoryPath={DirectoryPath}]";
        }
    }
}