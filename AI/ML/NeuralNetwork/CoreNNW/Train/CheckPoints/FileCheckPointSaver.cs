using AI.DataStructs;
using System;

namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{
    [Serializable]
    public class FileCheckPointSaver : ICheckPointSaver
    {
        public string FilePath { get; set; } = "";

        public FileCheckPointSaver(string filePath)
        {
            FilePath = filePath;
        }

        public void Save(INetwork net, float val)
        {
            BinarySerializer.Save(FilePath, net);
        }

        public override string ToString()
        {
            return $"FileCheckPointSaver[FilePath={FilePath}]";
        }
    }
}