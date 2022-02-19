using System;
using System.IO;

namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{
    public class DirectoryCheckPointSaver : ICheckPointSaver
    {
        public string DirectoryPath { get; set; } = "";

        public DirectoryCheckPointSaver(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public void Save(INetwork net, float val)
        {
            string fPath = Path.Combine(DirectoryPath, $"checkpoint - [{DateTime.Now:dd.MM.yyyy HH'h'mm'm'}], val - {val.ToString(AISettings.GetProvider())}.nn");
            net.Save(fPath);
        }

        public override string ToString()
        {
            return $"DirectoryCheckPointSaver[DirectoryPath={DirectoryPath}]";
        }
    }
}