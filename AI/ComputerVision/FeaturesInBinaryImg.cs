/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 24.07.2018
 * Время: 1:47
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.DSP.DSPCore;
using AI.Statistics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AI.ComputerVision
{
    /// <summary>
    /// Description of FeaturesInBinaryImg.
    /// </summary>
    [Serializable]
    public class FeaturesInBinaryImg
    {
        private ComplexVector points;
        private readonly bool _isRot, _isScale, _isMove;
        private readonly int n = 30;

        /// <summary>
        /// Фичи из матрицы изобр
        /// </summary>
        /// <param name="isRot">Сохранить оригенальный поворот</param>
        /// <param name="isScale">Сохранить оригенальный масштаб</param>
        /// <param name="isMove">Сохранить оригенальное смещение</param>
        /// <param name="nGarm">Количество гармоник, кол-во точек в 2 раза больше</param>
        public FeaturesInBinaryImg(bool isRot = false, bool isScale = false, bool isMove = false, int nGarm = 30)
        {
            _isRot = isRot;
            _isScale = isScale;
            _isMove = isMove;
            n = nGarm;
        }

        /// <summary>
        /// Генерация вектора частотных признаков из матрицы изображения
        /// </summary>
        /// <param name="img">Матрица изображения</param>
        /// <returns>Коэф. ряда фурье после преобразований</returns>
        public Vector MatrixFeatures(Matrix img)
        {
            GenVectorPoint(img);
            return GenFeature();
        }

        /// <summary>
        /// Кепстральные коэффициенты
        /// </summary>
        /// <param name="img">Изображение</param>
        public Vector KepstrFeatures(Matrix img)
        {
            GenVectorPoint(img);
            return Kepstral();
        }


        /// <summary>
        /// Выдает точки
        /// </summary>
        /// <param name="img">Матрица серого изображения</param>
        public Vector GetPoints(Matrix img)
        {
            GenVectorPoint(img);
            Vector real = points.MagnitudeVector.CutAndZero(n);
            Vector im = points.MagnitudeVector.CutAndZero(n);

            real /= Statistic.MaximalValue(real);
            im /= Statistic.MaximalValue(im);

            real -= Statistic.ExpectedValue(real);
            im -= Statistic.ExpectedValue(im);

            return Vector.Concat(new Vector[] { real, im });
        }

        // Генерация вектора фич
        private Vector GenFeature()
        {
            ComplexVector cV = FFT.CalcFFT(points);
            double k, cP1, cP2;
            Complex kR;

            if (!_isScale)
            {
                cP1 = cV[0].Magnitude;
                cP2 = cV[cV.Count - 1].Magnitude;
                k = Math.Sqrt(cP1 * cP1 + cP2 * cP2);
                cV /= k;
            }


            if (!_isRot)
            {
                cP1 = cV[0].Phase;
                cP2 = cV[cV.Count - 1].Phase;
                kR = Complex.Exp(new Complex(0, 1) * (cP2 - cP1) / 2.0);
                cV *= kR;
            }

            if (!_isMove)
            {
                cV[0] = 0;
            }

            cV = cV.CutAndZero(n);

            Vector modules = cV.MagnitudeVector;
            Vector phases = cV.PhaseVector;

            return phases;//Vector.Concatinate(new Vector[]{modules, phases});

        }

        // Получение точек в виде комплексных чисел
        private void GenVectorPoint(Matrix img)
        {
            List<Complex> pointList = new List<Complex>();

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {

                    if (img[i, j] < 0.5)
                    {
                        pointList.Add(new Complex(j, i));
                    }
                }
            }

            points = new ComplexVector(pointList.Count);

            for (int i = 0; i < points.Count; i++)
            {
                points[i] = pointList[i];
            }

        }

        private Vector Kepstral()
        {
            return Kepstr.FKT(points).CutAndZero(n);
        }
    }
}
