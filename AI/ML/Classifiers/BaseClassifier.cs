using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.IO;

namespace AI.ML.Classifiers
{
    public class BaseClassifier<T> : IClassifier
    {

        public virtual int Classify(Vector inp)
        {
            throw new NotImplementedException();
        }

        public virtual Vector ClassifyProbVector(Vector inp)
        {
            throw new NotImplementedException();
        }

        public virtual void Train(Vector[] features, int[] classes)
        {
            throw new NotImplementedException();
        }

        public virtual void Train(VectorIntDataset dataset)
        {
            Vector[] features = new Vector[dataset.Count];
            int[] classes = new int[dataset.Count];

            for (int i = 0; i < features.Length; i++)
            {
                classes[i] = dataset[i].ClassMark;
                features[i] = dataset[i].Features;
            }

            Train(features, classes);
        }

        public virtual void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }

        public virtual void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }

        public static T Load(string path)
        {
            return BinarySerializer.Load<T> (path);
        }

        public static T Load(Stream stream)
        {
            return BinarySerializer.Load<T>(stream);
        }
    }
}
