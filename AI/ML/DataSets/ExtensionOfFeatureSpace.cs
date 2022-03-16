using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;

namespace AI.ML.DataSets
{
    /// <summary>
    /// Расширение пространства признаков
    /// </summary>
    [Serializable]
    public static class ExtensionOfFeatureSpace
    {
        /// <summary>
        /// Раширение пространства признаков полиномиальной ф-ей
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="n">степень полинома</param>
        /// <returns>Новый вектор</returns>
        public static Vector Polinomial(double x, int n = 2)
        {
            Vector outp = new Vector(n);

            if (n >= 1)
            {
                outp[0] = x;

                for (int i = 1; i < n; i++)
                {
                    outp[i] = Math.Pow(x, i + 1);
                }
            }

            return outp;
        }

        /// <summary>
        /// Раширение пространства признаков полиномиальной ф-ей
        /// </summary>
        /// <param name="inp">Input</param>
        /// <param name="n">степень полинома</param>
        /// <returns>Новый вектор</returns>
        public static Vector Polinomial(Vector inp, int n = 2)
        {
            Vector[] vectors = new Vector[n];


            for (int i = 0; i < n; i++)
            {
                vectors[i] = inp.Transform(x => Math.Pow(x, i + 1));
            }


            return Vector.Concat(vectors);
        }

        /// <summary>
        /// Раширение пространства признаков косинусами
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="n">Число гармоник</param>
        /// <returns>Новый вектор</returns>
        public static Vector Cos(double x, int n = 2)
        {
            Vector outp = new Vector(n + 1);

            for (int i = 0; i <= n; i++)
            {
                outp[i] = Math.Cos(x * i);
            }

            return outp;
        }

        /// <summary>
        /// Раширение пространства признаков синусами 
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="n">Число гармоник</param>
        /// <returns>Новый вектор</returns>
        public static Vector Sin(double x, int n = 2)
        {
            Vector outp = new Vector(n);

            for (int i = 0; i <= n; i++)
            {
                outp[i] = Math.Sin(x * i);
            }

            return outp;
        }

        /// <summary>
        /// Раширение пространства признаков синусами и косинусами
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="n">Число гармоник</param>
        /// <returns>Новый вектор</returns>
        public static Vector SinCos(double x, int n = 2)
        {
            return Vector.Concat(new Vector[] { Sin(x, n), Cos(x, n), new Vector(new double[] { x }) });
        }

        /// <summary>
        /// Раширение пространства признаков косинусами
        /// </summary>
        /// <param name="inp">Input</param>
        /// <param name="n">Число гармоник</param>
        /// <returns>Новый вектор</returns>
        public static Vector Cos(Vector inp, int n = 2)
        {
            Vector[] vectors = new Vector[n + 1];

            vectors[0] = new Vector(1.0);

            for (int i = 1; i <= n; i++)
            {
                vectors[i] = FunctionsForEachElements.Cos(inp * i);
            }


            return Vector.Concat(vectors);
        }




        /// <summary>
        /// Расширение пространства с помощью полиномиальных ф-й и потом косинусов
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="nPolinom">Степень полинома</param>
        /// <param name="nCos">Кол-во косинусов</param>
        public static Vector PoliCos(Vector x, int nPolinom = 3, int nCos = 3)
        {
            Vector[] vects = new Vector[2];
            vects[0] = Polinomial(x, nPolinom);
            vects[1] = Cos(x, nCos);

            return Vector.Concat(vects);
        }

        /// <summary>
        /// Расширение пространства с помощью полиномиальных ф-й и потом косинусов
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="nPolinom">Степень полинома</param>
        /// <param name="nCos">Кол-во косинусов</param>
        public static Vector PoliCos(double x, int nPolinom = 3, int nCos = 3)
        {
            Vector[] vects = new Vector[2];
            vects[0] = Polinomial(x, nPolinom);
            vects[1] = Cos(x, nCos);

            return Vector.Concat(vects);
        }

        /// <summary>
        /// Радиально-базисная ф-я Гаусса
        /// </summary>
        /// <param name="x">Input</param>
        /// <param name="centers">Массив центров</param>
        /// <param name="std">СКО</param>
        /// <returns>Вектор значений от 0 до 1</returns>
        public static Vector GaussRBF(double x, Vector centers, double std = 1)
        {
            Vector outp = new Vector(centers.Count);
            for (int i = 0; i < centers.Count; i++)
            {
                double r = Math.Pow((centers[i] - x), 2) / (2 * std * std);
                outp[i] = Math.Exp(-r);
            }

            return outp;
        }



        /// <summary>
        /// Синус Котельникова sin(x)/x
        /// </summary>
        /// <param name="x"></param>
        /// <param name="centers"></param>
        /// <returns></returns>
        public static Vector Sinc(double x, Vector centers)
        {
            Vector outp = new Vector(centers.Count);
            for (int i = 0; i < centers.Count; i++)
            {
                double r = (x - centers[i]);
                outp[i] = Math.Sin(r) / r;
                outp[i] = r < 1e-13 ? 1 : outp[i];
            }

            return outp;
        }




    }

}
