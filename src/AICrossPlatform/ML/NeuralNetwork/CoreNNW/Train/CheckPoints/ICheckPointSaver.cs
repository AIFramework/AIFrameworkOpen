namespace AI.ML.NeuralNetwork.CoreNNW.Train.CheckPoints
{
    public interface ICheckPointSaver
    {
        void Save(INetwork net, float val);
    }
}