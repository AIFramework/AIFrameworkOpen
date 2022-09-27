using AI.DataStructs.Algebraic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI.ML.MatrixUtils
{
    /// <summary>
    /// Метод главных компонент
    /// </summary>
    [Serializable]
    public class PCA
    {
        /// <summary>
        /// Число итераций
        /// </summary>
        public int Iterations { get; set; } = 50;

        /// <summary>
        /// Информация об экземпляре
        /// </summary>
        public PCAInfo Info { get; protected set; }

        /// <summary>
        /// Собственные числа ков матрицы
        /// </summary>
        public Vector Eigenvalues { get; protected set; }


        /// <summary>
        /// Значение сходимости (если разница в RQ алгоритме выше, срабатывает исключение)
        /// </summary>
        public double Eps { get; set; } = 0.5;

        // Число компонент
        private int? _k;
        // Матрица преобразования
        private Matrix _pca;

        /// <summary>
        /// Корень из собственных чисел ков матрицы
        /// </summary>
        private Vector _sqrtEigenvalues;



        /// <summary>
        /// Метод главных компонент
        /// </summary>
        /// <param name="k">Число компонент null - все</param>
        public PCA(int? k = null)
        {
            _k = k;
        }

        /// <summary>
        /// Обучение PCA
        /// </summary>
        /// <param name="matrix">Матрица данных</param>
        public PCAInfo Train(Matrix matrix)
        {
            Matrix var_matrix = Matrix.GetCovMatrixFromColumns(matrix);
            Info = new PCAInfo();

            // Определение числа компонент
            int k = _k == null ? var_matrix.Height : _k.Value;
            k = k > var_matrix.Height ? var_matrix.Height : k;

            var eigenvalues = QR.GetEigenvalues(var_matrix, Iterations, Eps);
            
            eigenvalues.Sort((x,y)=>y.CompareTo(x)); // Поиск главных компонент

            if (k == var_matrix.Height)
            {
                Info.SaveVar = eigenvalues.Sum();
                Info.LastVar = 0;
            }
            else 
            {
                Info.SaveVar = 0;
                Info.LastVar = 0;
                int i = 0;
                for (; i < k; i++) Info.SaveVar += eigenvalues[i];
                for (; i < eigenvalues.Count; i++) Info.LastVar += eigenvalues[i];

            }

            Eigenvalues = eigenvalues.CutAndZero(k);
            _sqrtEigenvalues = Eigenvalues.Transform(Math.Sqrt);

            var vectors = QR.GetEigenvectors(var_matrix, Eigenvalues);
            _pca = Matrix.FromVectorsAsColumns(vectors);

            return Info;
        }

        ///// <summary>
        ///// Прямое преобразование
        ///// </summary>
        ///// <param name="data">Данные</param>
        ///// <param name="isNormal">Нормализовывать ли</param>
        //public Vector[] Transform(IEnumerable<Vector> data, bool isNormal = false)
        //{
        //    if (_pca == null)
        //        throw new Exception("Обучите алгоритм PCA!");

        //    Vector[] res = new Vector[data.Count()];
        //    int i = 0;
        //    foreach (var item in data) res[i++] = item * _pca;

        //    // Нужна ли нормализация 
        //    if (isNormal)
        //    {
        //        for (int j = 0; j < res.Length; j++)
        //            res[i] /= Eigenvalues;
        //    }

        //    return res;

        //}

        /// <summary>
        /// Прямое преобразование
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="isNormal">Нормализовывать ли</param>
        public Matrix Transform(Matrix data, bool isNormal = false)
        {

            if (_pca == null)
                throw new Exception("Обучите алгоритм PCA!");

            Matrix res = data * _pca; 

            // Нужна ли нормализация 
            if (isNormal)
            {
                for (int i = 0; i < res.Width; i++)
                    for (int j = 0; j < res.Height; j++)
                        res[j,i] /= _sqrtEigenvalues[i];
            }

            return res;
        }
    }

    /// <summary>
    /// Информация о преобразовании
    /// </summary>
    [Serializable]
    public class PCAInfo 
    {
        /// <summary>
        /// Доля сохраненной энергии
        /// </summary>
        public double InfoSaveEnergy => SaveVar / (SaveVar + LastVar);

        /// <summary>
        /// Остаточная дисперсия
        /// </summary>
        public double LastVar { get; set; }

        /// <summary>
        /// Сохраненная дисперсия
        /// </summary>
        public double SaveVar { get; set; }

    }
}
