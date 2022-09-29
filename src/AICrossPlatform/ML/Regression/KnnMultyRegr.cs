/*
 * Created by SharpDevelop.
 * User: 01
 * Date: 31.03.2016
 * Time: 18:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using AI.DataStructs.Algebraic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AI.ML.Regression
{
    /// <summary>
    /// Регрессия (Метод k-ближ. соседей)
    /// </summary>
    [Serializable]
    public class KnnMultyRegr : IMultyRegression
    {

        /// <summary>
        /// Число соседей
        /// </summary>
        public int K { get; set; }


        /// <summary>
        /// Ширина окна
        /// </summary>
        public double H { get; set; }

        /// <summary>
        /// Фиксирована ли ширина окна
        /// </summary>
        public bool FixedH { get; set; }

        /// <summary>
        /// Используется ли окно Парзена
        /// </summary>
        public bool IsNadrMethod { get; set; }

        /// <summary>
        /// Ядро окна
        /// </summary>
        public Func<double, double> KernelWindow { get; set; }

        /// <summary>
        /// Функция измерения расстояния
        /// </summary>
        public Func<Vector, Vector, double> Dist { get; set; }

        private StructRegresMulty reges;// Данные регрессии

        /// <summary>
        /// Набор данных
        /// </summary>
        public StructRegresMulty Reg
        {
            get => reges;
            set => reges = value;
        }


        /// <summary>
        /// Регрессия (Метод ближайшего соседа)
        /// </summary>
        public KnnMultyRegr()
        {
            reges = new StructRegresMulty();
            KernelWindow = RbfK;
            IsNadrMethod = false;
            FixedH = false;
            Dist = Distances.BaseDist.SquareEucl;
            K = 4;
            H = 1;
        }


        /// <summary>
        /// Радиально-базисное ядро для окна Парзена
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public double RbfK(double r)
        {
            return Math.Exp(-2 * r * r);
        }


        /// <summary>
        /// Регрессия (Метод ближайшего соседа)
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public KnnMultyRegr(string path)
        {
            reges = new StructRegresMulty();
            Open(path);
        }


        /// <summary>
        /// Регрессия (Метод ближайшего соседа)
        /// </summary>
        /// <param name="reg"> Данные для регрессии</param>
        public KnnMultyRegr(StructRegresMulty reg)
        {
            reges = reg;
        }


        /// <summary>
        /// Сохранение
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            try
            {
                StructRegresMulty clases = new StructRegresMulty
                {
                    Classes = reges.Classes
                };

                BinaryFormatter binFormat = new BinaryFormatter();

                using Stream fStream = new FileStream(path,
                  FileMode.Create, FileAccess.Write, FileShare.None);
                binFormat.Serialize(fStream, clases);
            }

            catch
            {
                throw new Exception("Save error (source: KnnMultyRegr)");
            }
        }


        /// <summary>
        /// Загрузка
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Open(string path)
        {

            try
            {

                StructRegresMulty clases;
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path,
                  FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    clases = (StructRegresMulty)binFormat.Deserialize(fStream);
                }

                reges.Classes = clases.Classes;
            }

            catch
            {
                throw new Exception("Load error (source: KnnMultyRegr)");
            }

        }

        /// <summary>
        /// Перевод в double
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <param name="R">Вес окна (возвращаемый параметр)</param>
        private Vector ToData(int i, out double R)
        {
            Vector mark = reges.Classes[i].Targets;
            Vector outp = new Vector(reges.Classes[0].Targets.Count);
            double r;
            if (IsNadrMethod)
            {
                r = reges.Classes[i].R / H;
                r = KernelWindow(r);
                outp = mark * r;
            }
            else
            {
                outp = mark;
                r = 1;
            }

            R = r;
            return outp;
        }


        // Ранжирование
        private void Rang(Vector inp)
        {

            for (int i = 0; i < reges.Classes.Count; i++)
            {
                reges.Classes[i].R = Dist(inp, reges.Classes[i].CentGiperSfer); // Вычисление билжайшего центра
            }

            reges.Classes.Sort((a, b) => a.R.CompareTo(b.R));
        }


        /// <summary>
        /// Добавление класса
        /// </summary>
        /// <param name="tData">Центральный вектор</param>
        /// <param name="targ">Зависимая переменная</param>
        public void Train(Vector tData, Vector targ)
        {
            StructRegrMulty structR = new StructRegrMulty
            {
                CentGiperSfer = tData.Clone(),
                Targets = targ
            };
            reges.Classes.Add(structR);
        }


        /// <summary>
        /// Обучение регрессии
        /// </summary>
        /// <param name="tData">Входные данные</param>
        /// <param name="targs">Данные выходов</param>
        public void Train(Vector[] tData, Vector[] targs)
        {
            for (int i = 0; i < tData.Length; i++)
            {
                Train(tData[i], targs[i]);
            }
        }

        /// <summary>
        /// Обучение регрессии
        /// </summary>
        /// <param name="tData">Входные данные</param>
        /// <param name="targs">Данные выходов</param>
        public void Train(Vector tData, Vector[] targs)
        {
            for (int i = 0; i < tData.Count; i++)
            {
                Train(new Vector(tData[i]), targs[i]);
            }
        }


        /// <summary>
        /// Прогнозируемый вектор
        /// </summary>
        /// <param name="inp">Вектор входных данных</param>
        public Vector Predict(Vector inp)
        {
            if (FixedH && IsNadrMethod)
            {
                double w = 0;
                Rang(inp);

                Vector pred = ToData(0, out double r);
                w += r;

                for (int i = 1; i < reges.Classes.Count; i++)
                {
                    pred += ToData(i, out r);
                    w += r;
                }

                pred /= w + 1e-20;


                return pred;
            }

            else
            {
                double w = 0;
                Rang(inp);
                H = reges.Classes[K - 1].R;

                Vector pred = ToData(0, out double r);
                w += r;

                for (int i = 1; i < K; i++)
                {
                    pred += ToData(i, out r);
                    w += r;
                }

                pred /= w;


                return pred;
            }

        }

        /// <summary>
        /// Предсказание
        /// </summary>
        /// <param name="inp">Вектор входа</param>
        public Vector[] PredictV(Vector[] inp)
        {
            Vector[] vect = new Vector[inp.Length];

            for (int i = 0; i < inp.Length; i++)
            {
                vect[i] = Predict(inp[i]);
            }

            return vect;
        }
    }
}


