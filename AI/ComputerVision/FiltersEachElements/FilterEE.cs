using AI.DataStructs.Algebraic;
using System;
using System.Drawing;

namespace AI.ComputerVision.FiltersEachElements
{
    /// <summary>
    /// Базовый класс гамма фильтра
    /// </summary>
    public class FilterEE : IFilterEE
    {
        private Func<double, double> _elFunc;
        private bool _prepNorm;
        private bool _postNorm;


        /// <summary>
        /// Гамма-фильтр
        /// </summary>
        /// <param name="elem">Функция фильтра</param>
        /// <param name="prepNorm">Пред. обработка (минимакс нормализация)</param>
        /// <param name="postNorm">Пост. обработка (минимакс нормализация)</param>
        public FilterEE(Func<double, double> elem, bool prepNorm = false, bool postNorm = false)
        {
            Init(elem, prepNorm, postNorm);
        }


        /// <summary>
        /// Гамма-фильтр
        /// </summary>
        public FilterEE()
        {
            _elFunc = (x) => x;
            _prepNorm = false;
            _postNorm = true;
        }

        /// <summary>
        /// Фильтрация
        /// </summary>
        /// <param name="input">Вход</param>
        public Matrix Filtration(Matrix input)
        {
            Matrix matrix = input.Copy();

            if (_prepNorm)
                matrix = matrix.Minimax();

            matrix = matrix.Transform(_elFunc);

            if (_postNorm)
                matrix = 255 * matrix.Minimax();

            Normal(matrix);

            return matrix;
        }

        /// <summary>
        /// Фильтрация
        /// </summary>
        /// <param name="input">Вход</param>
        public Bitmap Filtration(Bitmap input)
        {
            Matrix matrix = ImageMatrixConverter.BmpToMatr(input);
            Matrix filtred = Filtration(matrix);
            return new Bitmap(ImageMatrixConverter.ToBitmap(filtred), input.Width, input.Height);
        }

        /// <summary>
        /// Фильтрация
        /// </summary>
        /// <param name="input">Вход</param>
        public Bitmap Filtration(string path)
        {
            Matrix matrix = ImageMatrixConverter.LoadAsMatrix(path);
            Matrix filtred = Filtration(matrix);
            return new Bitmap(ImageMatrixConverter.ToBitmap(filtred), matrix.Width, matrix.Height);
        }

        public void Init(Func<double, double> elem, bool prepNorm = false, bool postNorm = false)
        {
            _elFunc = elem;
            _prepNorm = prepNorm;
            _postNorm = postNorm;
        }



        // Нормализация 0-255
        private void Normal(Matrix img)
        {
            for (int i = 0; i < img.Data.Length; i++)
            {
                if (img.Data[i] < 0) img.Data[i] = 0;
                if (img.Data[i] > 255) img.Data[i] = 255;
            }
        }
    }
}
