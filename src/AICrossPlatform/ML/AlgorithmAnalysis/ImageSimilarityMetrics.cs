using AI.DataStructs.Algebraic;
using System;

namespace AI.ML.AlgorithmAnalysis
{
    /// <summary>
    /// Метрики схожести изображений
    /// </summary>
    [Serializable]
    public class ImageSimilarityMetrics
    {
        // Коэф. для расчета SSIM
        private const double k1_ssim = 0.01, k2_ssim = 0.03;
        private static readonly double c1_ssim = (k1_ssim * 255) * (k1_ssim * 255), c2_ssim = (k2_ssim * 255) * (k2_ssim * 255);

        // Fausto Milletari, Nassir Navab, Seyed-Ahmad Ahmadi.// V-Net: Fully Convolutional Neural Networks for Volumetric Medical Image Segmentation.// 2016 Fourth International Conference on 3D Vision
        /// <summary>
        /// Приблизительный алгоритм вычисления значения метрики Дайса (метрика симметричная) 
        /// </summary>
        /// <param name="alg_str_1">Алгебраическая структура 1</param>
        /// <param name="alg_str_2">Алгебраическая структура 2</param>
        public static double DiceApproximate(IAlgebraicStructure<double> alg_str_1, IAlgebraicStructure<double> alg_str_2)
        {
            Vector v1 = alg_str_1.Data;
            Vector v2 = alg_str_2.Data;
            return 2 * (v1 * v2).Mean() / (v1.Mean() + v2.Mean());
        }

        // https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient 
        /// <summary>
        /// Значение метрики Дайса (метрика симметричная) 
        /// </summary>
        /// <param name="alg_str_1">Алгебраическая структура 1</param>
        /// <param name="alg_str_2">Алгебраическая структура 2</param>
        /// <param name="trashold">Порог</param>
        public static double Dice(IAlgebraicStructure<double> alg_str_1, IAlgebraicStructure<double> alg_str_2, double trashold = 0.5)
        {
            int alg_str_1_count_el = alg_str_1.Shape.Count;
            double[] data1 = alg_str_1.Data;
            double[] data2 = alg_str_2.Data;
            double s1 = 0, s2 = 0, s_cross = 0;

            for (int i = 0; i < alg_str_1_count_el; i++)
            {
                bool struct_1 = data1[i] >= trashold;
                bool struct_2 = data2[i] >= trashold;

                if (struct_1) s1++;
                if (struct_2) s2++;
                if (struct_2 && struct_1) s_cross++;
            }

            return 2 * s_cross / (s1 + s2);
        }

        /// <summary>
        /// Структурное сходство 2х изображений, метрика SSIM
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        public double SSIM(IAlgebraicStructure<double> img1, IAlgebraicStructure<double> img2)
        {
            double mx = Statistics.Statistic.ExpectedValue(img1);
            double sig_x = Statistics.Statistic.CalcStd(img1);

            double my = Statistics.Statistic.ExpectedValue(img2);
            double sig_y = Statistics.Statistic.CalcStd(img2);
            double im_cov = Statistics.Statistic.Cov(img1, img2);

            return (2 * mx * my + c1_ssim) * (2 * im_cov + c2_ssim) / ((mx * mx + my * my + c1_ssim) * (sig_y * sig_y + sig_x * sig_x + c2_ssim));
        }

    }
}
