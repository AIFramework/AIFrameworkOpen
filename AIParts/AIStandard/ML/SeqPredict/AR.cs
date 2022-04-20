using AI.DataStructs.Algebraic;
using AI.ML.Regression;
using System;

namespace AI.ML.SeqPredict
{
    /// <summary>
    /// Autoregression
    /// </summary>
    [Serializable]
    public class AR : ISeqPredict
    {
        private readonly SeqPrediction prediction;

        public AR(int windowLenght)
        {
            MultipleRegression multipleRegression = new MultipleRegression(true);
            prediction = new SeqPrediction(multipleRegression, windowLenght);
        }


        public Vector Predict(Vector data, int n)
        {
            return prediction.Predict(data, n);
        }

        public double PredictTrain(Vector data)
        {
            throw new NotImplementedException();
        }

        public void Train(Vector data)
        {
            prediction.Train(data);
        }
    }
}
