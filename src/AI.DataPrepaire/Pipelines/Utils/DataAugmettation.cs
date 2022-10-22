using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.DataPrepaire.Pipelines.Utils
{
    /// <summary>
    /// Аугментация данных
    /// </summary>
    [Serializable]
    public abstract class DataAugmetation<T>
    {

        /// <summary>
        /// Восколько раз расширить данные
        /// </summary>
        public int KAug { get; set; }


        /// <summary>
        /// Аугментация данных
        /// </summary>
        public DataAugmetation(int kAug)
        {
            KAug = kAug;
        }

        /// <summary>
        /// Создание набора из одного примера
        /// </summary>
        /// <param name="sample">Пример</param>
        public abstract T[] Augmetation(T sample);

        /// <summary>
        /// Создание набора для классификации из одного примера
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="classInd"></param>
        /// <returns></returns>
        public virtual Tuple<T[], int[]> Augmetation(T sample, int classInd)
        {
            T[] resultData = Augmetation(sample);
            int[] classes = new int[resultData.Length];

            for (int i = 0; i < classes.Length; i++)
            {
                classes[i] = classInd;
            }

            return new Tuple<T[], int[]>(resultData, classes);
        }


        /// <summary>
        /// Создание набора для классификации из одного примера
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="target">Целевое значение</param>
        /// <returns></returns>
        public virtual Tuple<T[], double[]> Augmetation(T sample, double target)
        {
            T[] resultData = Augmetation(sample);
            double[] targets = new double[resultData.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = target;
            }

            return new Tuple<T[], double[]>(resultData, targets);
        }


        /// <summary>
        /// Аугментация нескольких примеров
        /// </summary>
        public virtual T[] Augmetation(IEnumerable<T> samples)
        {
            T[] values = new T[samples.Count() * KAug];

            int k = 0;

            foreach (T sample in samples)
            {
                T[] d = Augmetation(sample);

                foreach (T value in d)
                {
                    values[k++] = value;
                }
            }

            return values;
        }

        /// <summary>
        /// Аугментация нескольких примеров, для классификатора
        /// </summary>
        public virtual Tuple<T, int>[] Augmetation(IEnumerable<T> samples, IEnumerable<int> cls)
        {
            T[] values = Augmetation(samples);
            int[] newCls = new int[values.Length];

            int k = 0;

            foreach (int cl in cls)
            {
                for (int i = 0; i < KAug; i++)
                {
                    newCls[k++] = cl;
                }
            }

            Tuple<T, int>[] tuples = new Tuple<T, int>[newCls.Length];

            for (int i = 0; i < newCls.Length; i++)
                tuples[i] = new Tuple<T, int>(values[i], newCls[i]);

            return tuples;
        }


        /// <summary>
        /// Аугментация нескольких примеров, для классификатора
        /// </summary>
        public virtual Tuple<T, double>[] Augmetation(IEnumerable<T> samples, IEnumerable<double> tgts)
        {
            T[] values = Augmetation(samples);
            double[] newCls = new double[values.Length];

            int k = 0;

            foreach (int target in tgts)
            {
                for (int i = 0; i < KAug; i++)
                {
                    newCls[k++] = target;
                }
            }

            Tuple<T, double>[] tuples = new Tuple<T, double>[newCls.Length];

            for (int i = 0; i < newCls.Length; i++)
                tuples[i] = new Tuple<T, double>(values[i], newCls[i]);

            return tuples;
        }
    }
}
