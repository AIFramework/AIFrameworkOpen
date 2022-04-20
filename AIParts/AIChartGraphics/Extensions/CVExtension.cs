using AI.ComputerVision;
using AI.DataStructs.Algebraic;
using System;
using System.Drawing;
using Matrix = AI.DataStructs.Algebraic.Matrix;

namespace AI.Extensions
{
    /// <summary>
    /// Extensoins for computer vision
    /// </summary>
    [Serializable]
    public static class CVExtension
    {
        /// <summary>
        /// Преобразование картинки в матрицу
        /// </summary>
        /// <param name="bitmap">Картинка</param>
        public static Matrix ToMatrix(this Bitmap bitmap)
        {
            return ImageMatrixConverter.BmpToMatr(bitmap);
        }

        /// <summary>
        /// Преобразование картинки в матрицу
        /// </summary>
        /// <param name="bitmap">Картинка</param>
        /// <param name="newWidth">Новая ширин</param>
        /// <param name="newHeights">Новая высота</param>
        public static Matrix ToMatrix(this Bitmap bitmap, int newWidth, int newHeight)
        {
            Bitmap bmp = new Bitmap(bitmap, newWidth, newHeight);
            return ImageMatrixConverter.BmpToMatr(bmp);
        }

        /// <summary>
        /// Преобразование картинки в тензор
        /// </summary>
        /// <param name="bitmap">Картинка</param>
        public static Tensor ToTensor(this Bitmap bitmap)
        {
            return ImageMatrixConverter.BmpToTensor(bitmap);
        }

        /// <summary>
        /// Преобразование картинки в тензор и изменение размера
        /// </summary>
        /// <param name="bitmap">Картинка</param>
        /// <param name="newW">Новая ширина</param>
        /// <param name="newH">Новая высота</param>
        public static Tensor ToTensor(this Bitmap bitmap, int newW, int newH)
        {
            Bitmap bmp = new Bitmap(bitmap, newW, newH);
            return ImageMatrixConverter.BmpToTensor(bmp);
        }

    }
}
