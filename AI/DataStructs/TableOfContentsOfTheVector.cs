using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;

namespace AI.DataStructs
{
    /// <summary>
    /// Оглавление сортированного вектора
    /// </summary>
    [Serializable]
    public class TableOfContentsOfTheSortedVector
    {
        private readonly int numberOfСhapters;
        private readonly int sizeOfVector;
        private readonly List<Vector> chapters = new List<Vector>();
        private readonly List<Diap> diaps = new List<Diap>();

        // данные диапазона
        private class Diap
        {
            public double Down { get; set; }
            public double Up { get; set; }
            public int DownIndex { get; set; }
        }

        /// <summary>
        /// Оглавление сортированного вектора
        /// </summary>
        /// <param name="vect">Сортированный вектор</param>
        /// <param name="numCh">Число глав</param>
        public TableOfContentsOfTheSortedVector(Vector vect, int numCh = 50)
        {
            try
            {
                int numCh2 = Math.Min(numCh, (vect.Count / 2000) + 1);

                numberOfСhapters = numCh2;
                sizeOfVector = vect.Count / numCh2;

                for (int i = 0; i < numCh2; i++)
                {

                    Diap diap = new Diap
                    {
                        DownIndex = i * sizeOfVector
                    };
                    diap.Down = vect[diap.DownIndex];
                    diap.Up = vect[((i + 1) * sizeOfVector) - 1];
                    diaps.Add(diap);

                    Vector data = new Vector(sizeOfVector);
                    for (int j = 0; j < sizeOfVector; j++)
                    {
                        data[j] = vect[diap.DownIndex + j];
                    }

                    chapters.Add(data);
                }
            }
            catch { }

        }


        /// <summary>
        /// Ближайший минимальный индекс
        /// </summary>
        /// <param name="value">Значение</param>
        public int IndexValueNeighborhoodMin(double value)
        {
            int i = 0;

            for (; i < numberOfСhapters; i++)
            {
                if (diaps[i].Down < diaps[i].Up)
                {
                    if ((diaps[i].Down <= value) && (diaps[i].Up >= value))
                    {
                        break;
                    }
                }
                else
                {
                    if (i + 1 < numberOfСhapters)
                    {
                        if ((diaps[i].Down <= value) && (diaps[i + 1].Up >= value))
                        {
                            i++;
                            break;
                        }
                    }
                }
            }

            i = (i >= numberOfСhapters) ? numberOfСhapters - 1 : i;

            int startInd = diaps[i].DownIndex;
            int index = chapters[i].IndexValueNeighborhoodMin(value);

            return startInd + index;
        }


    }
}
