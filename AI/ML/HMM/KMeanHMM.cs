using AI.DataStructs.Algebraic;
using AI.ML.Clustering;
using System;
using System.Linq;

namespace AI.ML.HMM
{
    /// <summary>
    /// Hidden Markov model with state extraction based on the k-means algorithm
    /// </summary>
    [Serializable]
    public class KMeanHMM
    {
        /// <summary>
        /// Extractor based on the k-means algorithm
        /// </summary>
        public KMeans KMean { get; set; }

        private readonly HMM hmm = new HMM();

        /// <summary>
        /// Hidden Markov model with state extraction based on the k-means algorithm
        /// </summary>
        public KMeanHMM(int numClasters = 3)
        {
            KMean = new KMeans(numClasters);
        }

        /// <summary>
        /// Obtaining a matrix of probabilities of transitions between states
        /// </summary>
        /// <param name="seq">sequence of vectors</param>
        public Matrix GetTransitionMatrix(Vector[] seq)
        {
            int[] states = KMean.Classify(seq).ToArray();
            hmm.Train(states);
            return hmm.stateMatrix;
        }

        /// <summary>
        /// Obtaining a vector of probabilities of transitions between states
        /// </summary>
        /// <param name="seq">Sequence of vectors</param>
        public Vector GetTransitionVector(Vector[] seq)
        {
            return GetTransitionMatrix(seq).Data;
        }

        /// <summary>
        /// Model training
        /// </summary>
        /// <param name="seqInp">Sequence of vectors</param>
        public void Train(Vector[] seqInp)
        {
            KMean.Train(seqInp);
            int[] states = KMean.Classify(seqInp).ToArray();
            hmm.Train(states);
        }

        /// <summary>
        /// Only Markov chein train
        /// </summary>
        /// <param name="seqInp">Sequence of vectors</param>
        public void TrainHMM(Vector[] seqInp)
        {
            int[] states = KMean.Classify(seqInp).ToArray();
            hmm.Train(states);
        }

        /// <summary>
        /// Generating a sequence of states
        /// </summary>
        /// <param name="start">Start vector</param>
        /// <param name="len">Sequence length</param>
        public int[] Generate(Vector start, int len)
        {
            int state = KMean.Classify(start);
            return hmm.Generate(len, state);
        }
    }
}
