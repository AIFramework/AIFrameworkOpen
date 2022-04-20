using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Датасет
    /// </summary>
    [Serializable]
    public class VectorIntDataset : List<VectorClass>
    {
        private readonly Random rnd = new Random(12);
        /// <summary>
        /// Средний вектор
        /// </summary>
        public Vector mean;
        /// <summary>
        /// Дисперсия по выборке
        /// </summary>
        public Vector disp;

        /// <summary>
        /// Загрузка датасета из файла
        /// </summary>
        /// <param name="path">File path</param>
        public VectorIntDataset(string path)
        {
            string[] content = File.ReadAllLines(path);
            VectorClass[] vC = new VectorClass[content.Length];

            for (int i = 0; i < content.Length; i++)
            {
                string[] strs = content[i].Split(';');

                vC[i] = new VectorClass(
                    Vector.FromStrings(strs[0].Split(' ')),
                    Convert.ToInt32(strs[1]));
            }

            AddRange(vC);
        }


        /// <summary>
        /// Датасет
        /// </summary>
        public VectorIntDataset() { }



        /// <summary>
        /// Датасет
        /// </summary>
        public VectorIntDataset(int capas) : base(capas)
        { }

        /// <summary>
        /// Случайный представитель датасета
        /// </summary>
        public VectorClass GetRandomData()
        {
            return this[rnd.Next(Count)];
        }


        /// <summary>
        /// Корреляционная матрица признаков
        /// </summary>
        /// <returns>Нормированная кор. матрица</returns>
        public Matrix CorrMatrFeatures()
        {
            Vector[] vects = new Vector[Count];

            for (int i = 0; i < vects.Length; i++)
            {
                vects[i] = this[i].Features.Clone();
            }


            Vector[] vects2 = new Vector[vects[0].Count];

            for (int i = 0; i < vects2.Length; i++)
            {
                vects2[i] = new Vector(vects.Length);


                for (int j = 0; j < vects.Length; j++)
                {
                    vects2[i][j] = vects[j][i];
                }
            }


            return Matrix.GetCorrelationMatrixNorm(vects2);
        }

        /// <summary>
        /// Получение вектора дисперсии и среднего вектора
        /// </summary>
        public void DispMeanResult()
        {
            Vector[] vects = new Vector[Count];

            for (int i = 0; i < vects.Length; i++)
            {
                vects[i] = this[i].Features;
            }

            mean = Statistic.MeanVector(vects);
            disp = Statistic.EnsembleDispersion(vects);
        }

        /// <summary>
        /// Нормализация датасета
        /// </summary>
        /// <returns>Датасет</returns>
        public VectorIntDataset Normalise()
        {

            DispMeanResult();

            disp = disp.Transform(d => (d == 0) ? 1e-109 : d);

            VectorIntDataset vid = new VectorIntDataset();
            Vector std = FunctionsForEachElements.Sqrt(disp);

            for (int i = 0; i < Count; i++)
            {
                vid.Add(new VectorClass
                        (
                            (this[i].Features - mean) / std,
                            this[i].ClassMark
                        )
                       );
            }

            return vid;
        }

        /// <summary>
        /// Удаление похожих векторов из разных классов
        /// </summary>
        /// <param name="simCoef">Коэффициент схожести</param>
        public VectorIntDataset GetDatasetDelSim(double simCoef = 0.9)
        {
            VectorIntDataset vid = new VectorIntDataset();
            List<int> simIndex = new List<int>();
            VectorClass[] vc;

            for (int i = 0; i < Count - 1; i++)
            {
                for (int j = i + 1; j < Count; j++)
                {
                    if (this[i].ClassMark != this[j].ClassMark)
                    {
                        if (Statistic.CorrelationCoefficient(this[i].Features, this[j].Features) > simCoef)
                        {
                            if (IsNotSerch(simIndex, j))
                            {
                                simIndex.Add(j);
                            }
                        }
                    }
                }

            }


            vc = new VectorClass[Count - simIndex.Count];

            for (int i = 0, k = 0; i < Count; i++)
            {
                if (IsNotSerch(simIndex, i))
                {
                    vc[k++] = this[i];
                }
            }

            vid.AddRange(vc);

            return vid;
        }

        private static bool IsNotSerch(List<int> simIndex, int i)
        {
            for (int j = 0; j < simIndex.Count; j++)
            {
                if (i == simIndex[j])
                {
                    return false;
                }
            }

            return true;
        }

        //TODO: Ускорить
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static VectorIntDataset CsvToVid(string path, char separator = ',')
        {
            string[] content = File.ReadAllLines(path);
            VectorClass[] vC = new VectorClass[content.Length];
            string[] strs;

            for (int i = 0; i < content.Length; i++)
            {
                strs = content[i].Split(separator);

                vC[i] = new VectorClass(
                    Vector.FromStrings(strs[0].Split(' ')),
                    Convert.ToInt32(strs[1]));
            }

            VectorIntDataset vid = new VectorIntDataset(content.Length);
            vid.AddRange(vC);
            return vid;
        }

        /// <summary>
        /// 
        /// </summary>
        public static VectorIntDataset CsvToVid(string path, int len, char separator = ',')
        {
            string[] content = File.ReadAllLines(path);

            len = content.Length > len ? len : content.Length;

            VectorClass[] vC = new VectorClass[len];
            string[] strs;

            for (int i = 0; i < len; i++)
            {
                strs = content[i].Split(separator);

                vC[i] = new VectorClass(
                    Vector.FromStrings(strs[0].Split(' ')),
                    Convert.ToInt32(strs[1]));
            }

            VectorIntDataset vid = new VectorIntDataset(len);
            vid.AddRange(vC);
            return vid;
        }

        /// <summary>
        /// Сохранение датасета
        /// </summary>
        public void Save(string path, char separator = ';')
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                stringBuilder.Append(this[i].Features.ToString());
                stringBuilder.Append(separator);
                stringBuilder.Append(this[i].ClassMark);
                stringBuilder.Append("\n");
            }

            File.WriteAllText(path, stringBuilder.ToString());
        }

    }
}
