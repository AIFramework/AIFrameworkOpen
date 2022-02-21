using AI.DataStructs.Algebraic;
using AI.ML.NeuralNetwork.CoreNNW.DataSets;
using System.Collections.Generic;

namespace TestNNW
{
    public static class TestDataSets
    {
        public static void Execute()
        {
            Vector inp1 = new double[] { 0.9, 0.1 };
            Vector inp2 = new double[] { 0.1, 0.9 };

            Vector outp1 = new double[] { 0.23, -0.1, 0.6 };
            Vector outp2 = new double[] { -0.9, 0.8, 0.4 };

            DataSetNoRecurrent dataSetNoReccurent = new DataSetNoRecurrent(new Vector[] { inp1, inp2 }, new Vector[] { outp1, outp2 }, null);

            List<Vector> inp1Req = new List<Vector>();
            inp1Req.Add(inp1);
            List<Vector> inp2Req = new List<Vector>();
            inp2Req.Add(inp2);

            List<Vector> outp1Req = new List<Vector>();
            outp1Req.Add(outp1);
            List<Vector> outp2Req = new List<Vector>();
            outp2Req.Add(outp2);

            DataSetRecurrent dataSetReccurent = new DataSetRecurrent(new[] { inp1Req, inp2Req }, new[] { outp1Req, outp2Req }, null);

        }
    }
}
