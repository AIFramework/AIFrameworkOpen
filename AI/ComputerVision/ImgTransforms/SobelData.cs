using AI.DataStructs.Algebraic;
using System;

namespace AI.ComputerVision.ImgTransforms
{
    /// <summary>
    /// Данные преобразования Собеля
    /// </summary>
    public class SobelData
    {
        /// <summary>
        /// Градиент вдоль оси X
        /// </summary>
        public Matrix GradX { get; set; }
        /// <summary>
        /// Градиент вдоль оси Y
        /// </summary>
        public Matrix GradY { get; set; }
        /// <summary>
        /// Модуль градиента
        /// </summary>
        public Matrix GradImg
        {
            get
            {
                return (GradX.AdamarProduct(GradX) + GradY.AdamarProduct(GradY)).Transform(Math.Sqrt);
            }
        }
        /// <summary>
        /// Фаза градиента
        /// </summary>
        public Matrix PhGrad
        {
            get
            {
                Matrix k = 1.0 / (GradImg + AISettings.GlobalEps);
                return GradX.AdamarProduct(k).Transform(Math.Acos);
            }
        }

        /// <summary>
        /// Создание изображения с данными преобразования Собеля
        /// </summary>
        /// <param name="gradX">Градиент X</param>
        /// <param name="gradY">Градиент Y</param>
        public SobelData(Matrix gradX, Matrix gradY)
        {
            GradX = gradX;
            GradY = gradY;
        }
    }

}
