using AI.DataStructs.Algebraic;
using AI.DataStructs.Shapes;
using AI.ML.NeuralNetwork;
using AI.ML.NeuralNetwork.CoreNNW;
using AI.ML.NeuralNetwork.CoreNNW.Layers;
using System;
using System.Collections.Generic;

namespace AI.ML.SeqPredict
{

    /// <summary>
    /// Предсказание на базе GRU сети
    /// </summary>
    [Serializable]
    public class GRUPredict : ISeqPredict
    {
        private readonly NeuralNetworkManager network;
        private readonly int window;
        private double std, m;


        /// <summary>
        /// Предсказание на базе GRU сети
        /// </summary>
        public GRUPredict(int w)
        {
            window = w;
            std = 1;
            m = 0;
            NNW net = new NNW();
            net.AddNewLayer(new Shape3D(window), new GRURegression(4));
            net.AddNewLayer(new Agregate());
            network = new NeuralNetworkManager(net);
        }

        /// <summary>
        /// Предсказать следующие n значений
        /// </summary>
        /// <param name="data">Начальные данные</param>
        /// <param name="n">Насколько шагов предсказать</param>
        public Vector Predict(Vector data, int n)
        {
            network.Model.ResetState();
            Vector outp = new Vector(data);

            for (int i = 0; i < n; i++)
            {
                int end = outp.Count;
                int start = end - window;
                Vector inp = (outp.GetInterval(start, end) - m) / std;
                outp.Add((network.Forward(inp)[0] * std) + m);
            }

            return outp;
        }

        /// <summary>
        /// Метод не реализован
        /// </summary>
        public double PredictTrain(Vector data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Обучение
        /// </summary>
        public void Train(Vector data)
        {

            std = data.Std();
            m = data.Mean();

            Vector seq = (data - m) / std;
            Vector y = new Vector(seq.Count - window);

            for (int i = window; i < seq.Count; i++)
            {
                y[i - window] = seq[i];
            }

            Vector[] X = Vector.GetWindows(seq, window, 1);
            Vector[] Y = new Vector[X.Length];

            for (int i = 0; i < X.Length; i++)
            {
                Y[i] = new Vector(y[i]);
            }

            int n = X.Length / 100;

            List<Vector>[] xS = new List<Vector>[n], yS = new List<Vector>[n];

            int lJ = X.Length / n;

            for (int i = 0; i < n; i++)
            {
                xS[i] = new List<Vector>();
                yS[i] = new List<Vector>();

                for (int j = 0; j < lJ; j++)
                {
                    xS[i].Add(X[(lJ * i) + j]);
                    yS[i].Add(Y[(lJ * i) + j]);
                }

            }


            network.EpochesToPass = 100;
            network.TrainNet(xS, yS);
        }
    }
}
