using AI.DataStructs.Algebraic;
using AI.ML.MatrixUtils;
using System;
using System.Collections.Generic;

namespace AI.ML.FeaturesTransforms
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
        private readonly int? _k;
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

            Matrix var_matrix = Matrix.GetCovMatrixFromColumns(matrix); // Получение кор. матрицы
            EigenValuesVectors eigen = new EigenValuesVectors(var_matrix, Iterations, Eps); // Вычисление собственных значений и векторов
            Info = new PCAInfo();

            // Определение числа компонент
            int k = _k == null ? var_matrix.Height : _k.Value;
            k = k > var_matrix.Height ? var_matrix.Height : k;

            var eigenvalues = eigen.Eigenvalues;

            eigenvalues.Sort((x, y) => y.CompareTo(x)); // Поиск главных компонент

            // Оценка качества
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
                for (; i < k; i++) Info.SaveVar += eigenvalues[i]; // Объясненная дисперсия
                for (; i < eigenvalues.Count; i++) Info.LastVar += eigenvalues[i]; // Остаточная дисперсия
            }

            Info.EpsEigenvalues = eigen.Eps;
            Info.IsConvergence = eigen.IsConvergence;

            Eigenvalues = eigenvalues.CutAndZero(k); // Топ k собственных чисел
            _sqrtEigenvalues = Eigenvalues.Transform(Math.Sqrt); // Корнм из собственных чисел (для нормализации)

            var vectors = EigenValuesVectors.GetEigenvectorsStatic(var_matrix, Eigenvalues); // Получение первых k векторов
            _pca = Matrix.FromVectorsAsColumns(vectors); // Создание матрицы преобразования

            return Info;
        }


        /// <summary>
        /// Обучение PCA
        /// </summary>
        /// <param name="vectors">Матрица данных</param>
        public PCAInfo Train(Vector[] vectors)
        {
            Matrix matrix = Matrix.FromVectorsAsRows(vectors);
            return Train(matrix);
        }

        /// <summary>
        /// Прямое преобразование
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="isNormal">Нормализовывать ли</param>
        public Vector[] Transform(IEnumerable<Vector> data, bool isNormal = false)
        {
            Matrix dMatr = Matrix.FromVectorsAsRows(data);
            return Matrix.GetRows(Transform(dMatr, isNormal));
        }

        /// <summary>
        /// Прямое преобразование одного вектора
        /// </summary>
        /// <param name="vector">Данные</param>
        /// <param name="isNormal">Нормализовывать ли</param>
        public Vector Transform(Vector vector, bool isNormal = false)
        {
            return Transform(new[] { vector }, isNormal)[0];
        }

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
                        res[j, i] /= _sqrtEigenvalues[i];
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

        /// <summary>
        /// Ошибка при вычисление собственных чисел
        /// </summary>
        public double EpsEigenvalues { get; set; }

        /// <summary>
        /// Сошелся ли алгоритм
        /// </summary>
        public bool IsConvergence { get; set; }

    }
}
