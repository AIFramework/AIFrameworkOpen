/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 16.09.2018
 * Время: 9:33
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.HightLevelFunctions;
using System;
using System.Drawing;

namespace AI.ComputerVision
{
    /// <summary>
    /// Description of BinaryImg.
    /// </summary>
    [Serializable]
    public class BinaryImg
    {
        private bool[,] img;
        /// <summary>
        /// Ширина
        /// </summary>
        public int M { get; set; }
        /// <summary>
        /// Высота
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Вывод индекса
        /// </summary>
        public bool this[int i, int j]
        {
            set => img[i, j] = value;

            get => img[i, j];
        }

        /// <summary>
        /// Бинарное изображение
        /// </summary>
        /// <param name="matr">Матрица серого</param>
        public BinaryImg(Matrix matr)
        {
            ToBools(matr);
            M = matr.Height;
            Count = matr.Width;
        }

        /// <summary>
        /// Бинарное изображение
        /// </summary>
        /// <param name="bm">Изображение</param>
        public BinaryImg(Bitmap bm)
        {
            Matrix matr = ImageMatrixConverter.BmpToMatr(bm);
            matr = ActivationFunctions.Threshold(matr, 0.85);
            ToBools(matr);
            M = matr.Height;
            Count = matr.Width;
        }

        private Matrix ToMatrix()
        {
            Matrix matr = new Matrix(M, Count);

            for (int i = 0; i < matr.Height; i++)
            {
                for (int j = 0; j < matr.Width; j++)
                {
                    matr[i, j] = img[i, j] ? 1 : 0;
                }
            }

            return matr;
        }


        /// <summary>
        /// Бинарное в матрицу
        /// </summary>
        public Matrix ToMatrixInvers()
        {
            Matrix matr = new Matrix(M, Count);

            for (int i = 0; i < matr.Height; i++)
            {
                for (int j = 0; j < matr.Width; j++)
                {
                    matr[i, j] = img[i, j] ? -1 : 0;
                }
            }

            return matr;
        }

        /// <summary>
        /// Бинарное в Bitmap
        /// </summary>
        public Bitmap ToBmp()
        {
            return ImageMatrixConverter.ToBitmap(ToMatrix());
        }

        private void ToBools(Matrix matr)
        {
            img = new bool[matr.Height, matr.Width];

            for (int i = 0; i < matr.Height; i++)
            {
                for (int j = 0; j < matr.Width; j++)
                {
                    img[i, j] = Math.Abs(matr[i, j] - 1) < 0.1;
                }
            }
        }
    }
}
