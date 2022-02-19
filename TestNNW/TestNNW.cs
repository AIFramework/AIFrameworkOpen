using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Activations;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using System;

namespace TestNNW
{
    public static class TestNNW
    {
        public static void Execute()
        {
            NNW net = new NNW();
            net.AddNewLayer(new Shape3D(2), new FeedForwardLayer(130, new ReLU(0.1)));
            net.AddNewLayer(new FeedForwardLayer(3, new LinearUnit()));
            Console.WriteLine(net.ToString());

            Vector inp1 = new double[] { 0.9, 0.1 };
            Vector inp2 = new double[] { 0.1, 0.9 };

            Vector outp1 = new double[] { 0.23, -0.1, 0.6 };
            Vector outp2 = new double[] { -0.9, 0.8, 0.4 };

            NeuralNetworkManager manager = new NeuralNetworkManager(net)
            {
                EpochesToPass = 10
            };
            manager.TrainNet(new[] { inp1, inp2 }, new[] { outp1, outp2 });
        }
    }
}
