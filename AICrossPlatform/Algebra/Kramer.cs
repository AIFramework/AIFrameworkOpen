/*
 * Создано в SharpDevelop.
 * Пользователь: 01
 * Дата: 14.07.2017
 * Время: 14:58
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using AI.DataStructs.Algebraic;
using System;
using System.Threading.Tasks;


namespace AI.Algebra
{
    /// <summary>
    /// Метод Крамера
    /// </summary>
    [Serializable]
    public class Kramer
    {
        private Matrix _a;
        private Vector _b, _x;
        private double _detA;

        private void Loop(int i)
        {
            _x[i] = NewDet(i) / _detA;
        }


        /// <summary>
        /// Решение СЛАУ методом Крамера
        /// </summary>
        /// <param name="A">Матрица коэффициентов</param>
        /// <param name="B">Вектор свободных членов</param>
        public Vector SolvingEquations(Matrix A, Vector B)
        {
            _a = A;
            _detA = _a.Determinant;
            _b = B;
            _x = new Vector(_b.Count);

            Parallel.For(0, _b.Count, Loop);

            return _x;
        }

        private double NewDet(int index)
        {
            Matrix newA = _a.Copy();

            for (int i = 0; i < _b.Count; i++)
            {
                newA[i, index] = _b[i];
            }



            return newA.Determinant;
        }
    }
}
