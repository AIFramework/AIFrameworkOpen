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
    /// Группа объектов одного класса
    /// </summary>
    public class GroupeVidData
    {
        public int GroupeMark;
        public List<Vector> GroupeFeatures = new List<Vector>();

        public GroupeVidData()
        {

        }

        public GroupeVidData(int gMark, Vector features)
        {
            GroupeMark = gMark;
            GroupeFeatures.Add(features);
        }

        public Vector Mean => Statistic.MeanVector(GroupeFeatures);
        public Vector Std => Statistic.EnsembleStd(GroupeFeatures);

        /// <summary>
        /// Возвращет индекс первого вхождения заданной метки класса
        /// </summary>
        /// <param name="lbl">Метка класса</param>
        public static int IndexLbl(IEnumerable<GroupeVidData> data, int lbl)
        {
            int i = 0;
            foreach (var item in data)
            {
                if (lbl == item.GroupeMark) return i;
                i++;
            }
            return -1;
        }
    }

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
        /// Получение векторов признаков
        /// </summary>
        /// <returns></returns>
        public Vector[] GetFeatures() 
        {
            Vector[] vects = new Vector[Count];

            for (int i = 0; i < vects.Length; i++)
            {
                vects[i] = this[i].Features.Clone();
            }

            return vects;
        }

        /// <summary>
        /// Корреляционная матрица признаков
        /// </summary>
        /// <returns>Нормированная кор. матрица</returns>
        public Matrix CorrMatrFeatures()
        {

            var vects = GetFeatures();

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
            Vector[] vects = GetFeatures();
            mean = Statistic.MeanVector(vects);
            disp = Statistic.EnsembleDispersion(vects);
        }


        /// <summary>
        /// Нормализация датасета
        /// </summary>
        /// <returns>Датасет</returns>
        public VectorIntDataset ZNormalise(string pathZData = "z_norm", bool isSave = true)
        {

            DispMeanResult();

            disp = disp.Transform(d => (d == 0) ? 1e-109 : d);

            VectorIntDataset vid = new VectorIntDataset();
            Vector std = FunctionsForEachElements.Sqrt(disp);

            // Сохранение параметров нормализации
            if (isSave)
            {
                if (!Directory.Exists(pathZData))
                    Directory.CreateDirectory(pathZData);

                string stdPath = $"{pathZData}\\std.vect";
                string meanPath = Path.Combine(pathZData, "mean.vect");

                mean.Save(meanPath);
                std.Save(stdPath);
            }

            //Нормализация
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
                string features = this[i].Features.ToString();
                features = features.Replace(' ', separator).Replace("[", "").Replace("]", "");
                stringBuilder.Append(features);
                stringBuilder.Append(separator);
                stringBuilder.Append(this[i].ClassMark);
                stringBuilder.Append("\n");
            }

            File.WriteAllText(path, stringBuilder.ToString());
        }


        /// <summary>
        /// Группирует классы вычисляя средний вектор признаков
        /// </summary>
        public VectorIntDataset GroupMean()
        {
            var data = GetGroupes();
            VectorIntDataset vid = new VectorIntDataset(data.Length);

            foreach (var item in data)
                    vid.Add(new VectorClass(item.Mean, item.GroupeMark)); 
            return vid;
        }

        /// <summary>
        /// Группирует датасет по классам
        /// </summary>
        /// <returns></returns>
        public GroupeVidData[] GetGroupes() 
        {
            List<GroupeVidData> vidG = new List<GroupeVidData>();

            foreach (var item in this)
            {
                var ind = GroupeVidData.IndexLbl(vidG, item.ClassMark);
                if (ind == -1) vidG.Add(new GroupeVidData(item.ClassMark, item.Features));
                else vidG[ind].GroupeFeatures.Add(item.Features);
            }

            return vidG.ToArray();
        }

        /// <summary>
        /// Возвращет индекс первого вхождения заданной метки класса
        /// </summary>
        /// <param name="lbl">Метка класса</param>
        public int IndexLbl(int lbl)
        {
            for (int i = 0; i < Count; i++)
                if (lbl == this[i].ClassMark) return i;

            return -1;
        }
    }
}
