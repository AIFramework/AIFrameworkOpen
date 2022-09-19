using AI.DataStructs;
using AI.DataStructs.Algebraic;
using AI.ML.DataSets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AI.ML.Classifiers
{

    /// <summary>
    /// Классификатор, работающий по принципу метода эталонов
    /// </summary>
    [Serializable]
    public class NN : IClassifier
    {
        private StructClasses _classes;// Классификатор
        /// <summary>
        /// Classes
        /// </summary>
        public StructClasses Classes
        {
            get => _classes;
            set => _classes = value;
        }


        /// <summary>
        ///  Классификатор, работающий по принципу метода эталонов
        /// </summary>
        public NN()
        {
            _classes = new StructClasses();
        }
        /// <summary>
        ///  Классификатор, работающий по принципу метода эталонов
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public NN(string path)
        {
            _classes = new StructClasses();
            Load(path);
        }
        /// <summary>
        ///  Классификатор, работающий по принципу метода эталонов
        /// </summary>
        /// <param name="classifikator"> Classes</param>
        public NN(StructClasses classifikator)
        {
            _classes = classifikator;
        }

        // Поиск центра класса
        private Vector GetCentr(Vector[] vectors)
        {
            int Count = vectors.Length;
            Vector output = vectors[0];

            for (int i = 1; i < Count; i++)
            {
                output += vectors[i];
            }

            return output / Count;
        }
        /// <summary>
        /// Adding a standard to the classifier
        /// </summary>
        /// <param name="tDataset">Dataset</param>
        /// <param name="numClass">Индекс класса</param>
		public void AddClass(Vector[] tDataset, int numClass)
        {
            Vector a = GetCentr(tDataset);
            _classes.Add(new VectorClass(a, numClass));
        }
        /// <summary>
        /// Recognizing a vector
        /// </summary>
        /// <param name="inp">Вход</param>
        public int Classify(Vector inp)
        {

            double _stMin = 1e+300, _st;
            int output = -1;

            for (int i = 0; i < _classes.Count; i++)
            {
                _st = Distances.BaseDist.SquareEucl(inp, _classes[i].Features); // Вычисление билжайшего центра
                if (_st < _stMin)
                {
                    _stMin = _st;
                }
            }


            for (int i = 0; i < _classes.Count; i++)
            {
                _st = Distances.BaseDist.SquareEucl(inp, _classes[i].Features); // Вычисление билжайшего центра

                if (_st == _stMin)
                {
                    output = _classes[i].ClassMark;
                    break;
                }

            }


            return output;
        }
        /// <summary>
        /// Adding a class
        /// </summary>
        /// <param name="features">Feature vector</param>
        /// <param name="num">Label </param>
        public void AddClass(Vector features, int num)
        {
            VectorClass structClass = new VectorClass
            {
                Features = features.Clone(),
                ClassMark = num
            };
            _classes.Add(structClass);
        }
        /// <summary>
        /// Returns a vector, the length of which is equal to the number of classes, the value at the index of the required class is set to one, and at other indices to zero.
        /// </summary>
        /// <param name="inp">Вектор входных данных</param>
        public Vector ClassifyProbVector(Vector inp)
        {
            List<int> indexis = new List<int>();

            for (int i = 0; i < _classes.Count; i++)
            {
                indexis.Add(_classes[i].ClassMark);
            }

            int Max = indexis.Max();

            Vector classes = new Vector(Max + 1)
            {
                [Classify(inp)] = 1
            };

            return classes;
        }
        /// <summary>
        /// Обучение классификатора
        /// </summary>
        /// <param name="features">Признаки</param>
        /// <param name="classes">Метки классов</param>
        public void Train(Vector[] features, int[] classes)
        {
            if (features.Length != classes.Length)
            {
                throw new InvalidOperationException("The number of feature vectors and the class method do not match");
            }

            for (int i = 0; i < features.Length; i++)
            {
                AddClass(features[i], classes[i]);
            }
        }
        /// <summary>
        /// Обучение классификатора
        /// </summary>
        /// <param name="dataset">Набор данных признаки-метка</param>
        public void Train(VectorIntDataset dataset)
        {
            for (int i = 0; i < dataset.Count; i++)
            {
                AddClass(dataset[i].Features, dataset[i].ClassMark);
            }
        }

        /// <summary>
        /// Обучение классификатора (группировка данных)
        /// </summary>
        /// <param name="dataset">Набор данных признаки-метка</param>
        public void TrainGroup(VectorIntDataset dataset)
        {
            VectorIntDataset data = dataset.GroupMean();

            for (int i = 0; i < data.Count; i++)
            {
                AddClass(data[i].Features, data[i].ClassMark);
            }
        }
        /// <summary>
        /// Save to file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            BinarySerializer.Save(path, this);
        }
        /// <summary>
        /// Save to stream
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream)
        {
            BinarySerializer.Save(stream, this);
        }
        /// <summary>
        /// Load from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NN Load(string path)
        {
            return BinarySerializer.Load<NN>(path);
        }
        /// <summary>
        /// Load from stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static NN Load(Stream stream)
        {
            return BinarySerializer.Load<NN>(stream);
        }
    }
}
