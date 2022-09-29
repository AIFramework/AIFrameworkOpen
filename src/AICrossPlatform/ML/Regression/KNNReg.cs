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
    public class KNNReg : IRegression
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

        /// <summary>
        /// Данные для регрессии
        /// </summary>
        public StructRegres Reg { get; set; }


        /// <summary>
        /// Регрессия (Метод k-ближ. соседей)
        /// </summary>
        public KNNReg()
        {
            Reg = new StructRegres();
            KernelWindow = RbfK;
            IsNadrMethod = false;
            FixedH = false;
            Dist = Distances.BaseDist.SquareEucl;
            K = 4;
            H = 1;
        }


        /// <summary>
        /// Радиально-базисное ядро
        /// </summary>
        public double RbfK(double r)
        {
            return Math.Exp(-2 * r * r);
        }


        /// <summary>
        /// Регрессия (Метод k-ближ. соседей)
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public KNNReg(string path)
        {
            Reg = new StructRegres();
            Open(path);
        }


        /// <summary>
        /// Регрессия (Метод k-ближ. соседей)
        /// </summary>
        /// <param name="reg">Набор данных</param>
        public KNNReg(StructRegres reg)
        {
            Reg = reg;
        }


        /// <summary>
        /// Сохранение
        /// </summary>
        /// <param name="path">Путь до файла</param>
        public void Save(string path)
        {
            try
            {
                StructRegres clases = new StructRegres
                {
                    Classes = Reg.Classes
                };

                BinaryFormatter binFormat = new BinaryFormatter();

                using Stream fStream = new FileStream(path,
                  FileMode.Create, FileAccess.Write, FileShare.None);
                binFormat.Serialize(fStream, clases);
            }

            catch
            {
                throw new Exception("Save error (source: kNNRegr)");
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

                StructRegres clases;
                BinaryFormatter binFormat = new BinaryFormatter();

                using (Stream fStream = new FileStream(path,
                  FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    clases = (StructRegres)binFormat.Deserialize(fStream);
                }

                Reg.Classes = clases.Classes;
                Dist = Distances.BaseDist.SquareEucl;
                KernelWindow = RbfK;
                K = 3;
                H = 1;
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
        /// <param name="R">Вес окна</param>
        private double ToData(int i, out double R)
        {
            double mark = Reg.Classes[i].Target;
            double outp = 0;
            double r;
            if (IsNadrMethod)
            {
                r = Reg.Classes[i].R / H;
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

        /// <summary>
        /// Ранжирование
        /// </summary>
        private void Rang(Vector inp)
        {

            for (int i = 0; i < Reg.Classes.Count; i++)
            {
                Reg.Classes[i].R = Dist(inp, Reg.Classes[i].Features); // Вычисление билжайшего центра
            }

            Reg.Classes.Sort((a, b) => a.R.CompareTo(b.R));
        }


        /// <summary>
        /// Добавление класса
        /// </summary>
        /// <param name="tData">Центральный вектор</param>
        /// <param name="targ">Зависимая переменная</param>
        public void Train(Vector tData, double targ)
        {
            StructRegr structR = new StructRegr
            {
                Features = tData.Clone(),
                Target = targ
            };
            Reg.Classes.Add(structR);
        }

        /// <summary>
        /// Обучение регрессии
        /// </summary>
        /// <param name="tData">Входные данные</param>
        /// <param name="targs">Данные выходов</param>
        public void Train(Vector[] tData, Vector targs)
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
        public void Train(Vector tData, Vector targs)
        {
            for (int i = 0; i < tData.Count; i++)
            {
                Train(new Vector(tData[i]), targs[i]);
            }
        }


        /// <summary>
        /// Возращает вектор, его длинна - Number of classes, на номере нужного класса стоит 1
        /// </summary>
        /// <param name="inp">Вектор входных данных</param>
        /// <returns></returns>
        public double Predict(Vector inp)
        {
            if (FixedH && IsNadrMethod)
            {
                double pred = 0;
                double w = 0;
                Rang(inp);

                for (int i = 0; i < Reg.Classes.Count; i++)
                {
                    pred += ToData(i, out double r);
                    w += r;
                }

                pred /= w;


                return pred;
            }

            else
            {

                double pred = 0;
                double w = 0;
                Rang(inp);
                H = Reg.Classes[K - 1].R;

                for (int i = 0; i < K; i++)
                {
                    pred += ToData(i, out double r);
                    w += r;
                }

                pred /= w;


                return pred;
            }

        }

        /// <summary>
        /// Прогноз вектора значений
        /// </summary>
        /// <param name="inp">Вектор входных данных</param>
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