/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 06.06.2017
 * Время: 21:31
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;


namespace AI.DSP.DSPCore
{
    /// <summary>
    /// Дискретно-косинусное преобразование
    /// </summary>
    [Serializable]
    public class DCT
    {


        /// <summary> 
        /// Матрица прямого преобразования
        /// </summary>
        public Matrix MainMatrix { get; set; }
        /// <summary> 
        /// Матрица обратного преобразования
        /// </summary>
        public Matrix InvMatrix { get; set; }


        /// <summary>
        /// Дискретно-косинусное преобразование
        /// </summary>
        public DCT()
        {
        }

        /// <summary>
        /// Дискретно-косинусное преобразование
        /// </summary>
        /// <param name="countInp">Кол-во входов</param>
        /// <param name="countOutp">Кол-во Выходов</param>
        public DCT(int countInp, int countOutp)
        {
            InvMatrix = GetMatrW(countInp, countOutp);
            MainMatrix = InvMatrix.Transpose();
        }





        /// <summary>
        /// Матрица
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="M"></param>
        /// <returns></returns>
        public static Matrix GetMatrW(int Count, int M)
        {
            Matrix W = new Matrix(M, Count);
            double lambda = 1.0 / Math.Sqrt(Count);
            for (int j = 0; j < W.Width; j++)
                W[0, j] = lambda;

            lambda = Math.Sqrt(2.0 / Count);

            for (int i = 1; i < W.Height; i++)
                for (int j = 0; j < W.Width; j++)
                    W[i, j] = lambda * Math.Cos(GetArg(i, j, Count));

            return W;
        }

        private static double GetArg(int i, int j, int count)
        {
            return i * Math.PI * ((2 * j) + 1) / (2 * count);
        }

        /// <summary>
        /// Прямое ДКТ
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public Vector DirectDCTNorm(Vector inp)
        {
            return inp * MainMatrix;
        }



        /// <summary>
        /// Обратное Дкт
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        public Vector InversDCTNorm(Vector inp)
        {
            Matrix inpM = inp.ToMatrix().Transpose();
            return (InvMatrix * inpM).Transpose().LikeVector();
        }

    }
}
