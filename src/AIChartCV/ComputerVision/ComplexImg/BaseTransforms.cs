using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI.ComputerVision.ComplexImg
{
    /// <summary>
    /// Базовые преобразования изображений с использованием комплексныхчисел
    /// </summary>
    [Serializable]
    public class BaseTransformsColorImg
    {
        Vector[] _pixels = null;
        Tensor _colorImg = null;

        /// <summary>
        /// Базовые преобразования изображений с использованием комплексныхчисел
        /// </summary>
        protected BaseTransformsColorImg() { }

        /// <summary>
        /// Преобразование изображения
        /// </summary>
        /// <param name="func"></param>
        /// <param name="h"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public Bitmap Transform(Func<Complex, Complex> func, int h, int w) 
        {
            var cv = Img2Complex();
            cv = Norm(cv);
            cv = cv.Transform(func);
            cv = Norm(cv, h - 1, w - 1);
            return Compl2Img(cv, h, w);
        }


        /// <summary>
        /// Базовые преобразования изображений с использованием комплексныхчисел
        /// </summary>
        /// <param name="bitmap"></param>
        public BaseTransformsColorImg(Bitmap bitmap)
        {
            _colorImg = ImageMatrixConverter.BmpToTensor(bitmap);
            _pixels = new Vector[_colorImg.Width * _colorImg.Height];
        }

        /// <summary>
        /// Базовые преобразования изображений с использованием комплексныхчисел
        /// </summary>
        /// <param name="path"></param>
        public BaseTransformsColorImg(string path)
        {
            _colorImg = ImageMatrixConverter.LoadAsTensor(path);
            _pixels = new Vector[_colorImg.Width*_colorImg.Height];
        }

        /// <summary>
        /// Получить пиксель
        /// </summary>
        /// <param name="x">Коорд. x</param>
        /// <param name="y">Коорд. y</param>
        public static Vector GetPixel(int x, int y, Tensor tensor)
        {
            Vector vect = new Vector(3);

            for (int i = 0; i < 3; i++)
                vect[i] = tensor[y, x, i];

            return vect;
        }

        /// <summary>
        /// Установить пиксель
        /// </summary>
        /// <param name="x">Коорд. x</param>
        /// <param name="y">Коорд. y</param>
        /// <param name="pixel">RGB вектор</param>
        public static void SetPixel(int x, int y, Tensor tensor, Vector pixel)
        {
            for (int i = 0; i < 3; i++)
                tensor[y, x, i] = pixel[i];
        }


        // Нормализация
        ComplexVector Norm(ComplexVector complexes, int h = 1, int w = 1)
        {
            Vector hv = h * complexes.ImaginaryVector.Minimax();
            Vector wv = w * complexes.RealVector.Minimax();
            ComplexVector complexeVect = new ComplexVector(hv.Count);

            for (int i = 0; i < complexeVect.Count; i++)
                complexeVect[i] = new Complex(wv[i], hv[i]);

            return complexeVect;
        }


        ComplexVector Img2Complex()
        {
            int len = _colorImg.Width * _colorImg.Height;

            ComplexVector complexes = new ComplexVector(len);


            for (int i = 0, k = 0; i < _colorImg.Width; i++)
                for (int j = 0; j < _colorImg.Height; j++)
                {
                    complexes[k] = new Complex(i, j);
                    _pixels[k] = GetPixel(i, j, _colorImg);
                    k++;
                }

            return complexes;
        }

        Bitmap Compl2Img(ComplexVector complexes, int h, int w)
        {
            Tensor tensor = new Tensor(h, w, 3);

            for (int i = 0; i < complexes.Count; i++)
                SetPixel((int)complexes[i].Real, (int)complexes[i].Imaginary, tensor, _pixels[i]);

            return ImageMatrixConverter.ToBitmap(tensor);
        }
    }
}
