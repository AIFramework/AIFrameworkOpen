/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 31.03.2016
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using AI.ML.Distances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AI.ML.Regression
{
    /// <summary>
    /// Regression(k-nearest neighbors method) (correlation metric)
    /// </summary>
    [Serializable]
    public class KNNCorR : IRegression
    {
        /// <summary>
        /// Number of neighbors
        /// </summary>
        public int K { get; set; }
        /// <summary>
        /// Is the width fixed
        /// </summary>
        public bool FixedH { get; set; }
        /// <summary>
        /// Whether to trigger mutation of neighbors
        /// </summary>
        public bool isMutation = true;
        /// <summary>
        /// Number of mutated neighbors
        /// </summary>
        public int mutCount = 0;
        /// <summary>
        /// Distance function
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }

        private StructRegres reges;// Данные регрессии

        /// <summary>
        /// Regression data
        /// </summary>
        public StructRegres Reg
        {
            get => reges;
            set => reges = value;
        }
        /// <summary>
        /// Regression (Nearest Neighbor Method)
        /// </summary>
        public KNNCorR()
        {
            reges = new StructRegres();
            FixedH = false;
            Dist = BaseDist.SquareEucl;
            K = 4;
        }
        /// <summary>
        /// Regression (Nearest Neighbor Method)
        /// </summary>
        /// <param name="path">File path</param>
        public KNNCorR(string path)
        {
            reges = new StructRegres();
            Open(path);
        }
        /// <summary>
        /// Regression (Nearest Neighbor Method)
        /// </summary>
        /// <param name="reg"> Данные для регрессии</param>
        public KNNCorR(StructRegres reg)
        {
            reges = reg;
        }
        /// <summary>
        /// Saving the regression model
        /// </summary>
        /// <param name="path">File path</param>
        public void Save(string path)
        {
            try
            {
                StructRegres clases = new StructRegres
                {
                    Classes = reges.Classes
                };

                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path,
                  FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    binFormat.Serialize(fStream, clases);
                }
            }

            catch
            {
                throw new ArgumentException("Ошибка сохранения", "Сохранение");
            }
        }
        /// <summary>
        /// Loading a regression model
        /// </summary>
        /// <param name="path">File path</param>
        public void Open(string path)
        {

            try
            {

                StructRegres clases;
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path,
                  FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    clases = (StructRegres)binFormat.Deserialize(fStream);
                }

                reges.Classes = clases.Classes;
            }

            catch
            {
                throw new ArgumentException("Ошибка загрузки", "Загрузка");
            }

        }
        // Перевод в double
        private double ToData(int i, out double R)
        {
            double mark = reges.Classes[i].Target;
            double r = reges.Classes[i].Params[0];

            double outp = mark * r;
            R = r;
            return outp;
        }
        // Ранжирование
        private void Rang(Vector inp)
        {

            for (int i = 0; i < reges.Classes.Count; i++)
            {
                reges.Classes[i].R = Dist(inp, reges.Classes[i].Features); // Вычисление билжайшего центра
            }

            reges.Classes.Sort((a, b) => a.R.CompareTo(b.R));
        }
        /// <summary>
        /// Adding data
        /// </summary>
        /// <param name="tData">Central vector</param>
        /// <param name="targ">Dependent (target) variable</param>
        public void Train(Vector tData, double targ)
        {
            if (isMutation)
            {
                AddDataMut(tData, targ);
            }
            else
            {
                AddData(tData, targ);
            }
        }
        /// <summary>
        /// Добавление данных для обучения
        /// </summary>
        /// <param name="tData">Input data vector</param>
        /// <param name="targ">Выход</param>
        private void AddData(Vector tData, double targ)
        {
            StructRegr structR = new StructRegr
            {
                Features = tData.Clone(),
                Target = targ
            };
            reges.Classes.Add(structR);
            reges.Classes[reges.Classes.Count - 1].Params = new double[2];
        }
        private void AddDataMut(Vector tData, double targ)
        {
            if (reges.Classes.Count == 0)
            {
                AddData(tData, targ);
            }
            else
            {
                Rang(tData);
                int count = reges.Classes.Count - 1;
                double d = reges.Classes[0].R,
                    h = reges.Classes[count].R;

                d = Math.Exp(-5 * d / h);

                if (d > 0.97)
                {
                    reges.Classes[0].Features += tData;
                    reges.Classes[0].Features /= 2.0;
                    reges.Classes[0].Target += targ;
                    reges.Classes[0].Target /= 2.0;
                    mutCount++;
                }
                else
                {
                    AddData(tData, targ);
                }
            }
        }
        /// <summary>
        /// Regression training
        /// </summary>
        /// <param name="tData">Training data inputs</param>
        /// <param name="targs">Ideal outputs</param>
        public void Train(Vector[] tData, Vector targs)
        {
            for (int i = 0; i < tData.Length; i++)
            {
                Train(tData[i], targs[i]);
            }
        }
        /// <summary>
        /// Regression training
        /// </summary>
        /// <param name="tData">Training data inputs</param>
        /// <param name="targs">Ideal outputs</param>
        public void Train(Vector tData, Vector targs)
        {
            for (int i = 0; i < tData.Count; i++)
            {
                Train(new Vector(tData[i]), targs[i]);
            }
        }
        /// <summary>
        /// Model prediction
        /// </summary>
        /// <param name="inp">Input data vector</param>
        public double Predict(Vector inp)
        {
            double pred = 0;
            double w = 0;
            Rang(inp);

            double d, h = reges.Classes[K - 1].R;


            reges.Classes[0].Params[1]++;

            for (int i = 0; i < K; i++)
            {
                d = reges.Classes[i].R;

                reges.Classes[i].Params[0] = Math.Exp(-2 * d * d / h);



                pred += ToData(i, out double r);
                w += r;
            }

            pred /= w;
            return pred;

        }
        /// <summary>
        /// Calculation of the distribution of the importance of objects for the forecast
        /// </summary>
        /// <returns></returns>
        public Vector ImpObj()
        {
            Vector vs = new Vector(0);

            for (int i = 0; i < reges.Classes.Count; i++)
            {
                vs.Add(reges.Classes[i].Params[1]);
            }

            return vs;
        }
        /// <summary>
        /// Leaves only objects important for forecasting
        /// </summary>
        /// <param name="n">Number of objects to keep</param>
        public void OnlyImp(int n = 60)
        {
            List<StructRegr> regs = new List<StructRegr>();
            reges.Classes.Sort((a, b) => a.Params[1].CompareTo(b.Params[1]) * -1);

            for (int i = 0; i < n; i++)
            {
                regs.Add(reges.Classes[i]);
            }

            reges.Classes.Clear();

            reges.Classes.AddRange(regs);
        }
        /// <summary>
        /// Vector prediction
        /// </summary>
        /// <param name="inp">Input data vector</param>
        public Vector PredictV(Vector inp)
        {
            Vector vect = new Vector(inp.Count);

            for (int i = 0; i < inp.Count; i++)
            {
                vect[i] = Predict(new Vector(inp[i]));
            }

            return vect;
        }
    }
}


