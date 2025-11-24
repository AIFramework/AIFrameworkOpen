using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.HightLevelFunctions;

/// <summary>
/// Аналитическая геометрия.
/// Содержит функции для работы с векторами: вычисление скалярного произведения, нормы, угла между векторами, проекции, расстояния и поворота.
/// </summary>
public static class AnalyticGeometryFunctions
{
    /// <summary>
    /// Вычисляет косинус угла между двумя векторами.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности векторов должны совпадать и быть ненулевыми.</exception>
    public static double Cos(Vector vector1, Vector vector2)
    {
        EnsureSameDimensions(vector1, vector2);
        double norm1 = NormVect(vector1);
        double norm2 = NormVect(vector2);
        if (norm1 == 0 || norm2 == 0)
            throw new ArgumentException("Невозможно вычислить угол для нулевого вектора.");
        return Dot(vector1, vector2) / (norm1 * norm2);
    }

    /// <summary>
    /// Вычисляет Евклидову норму вектора.
    /// </summary>
    public static double NormVect(Vector vector)
    {
        return Math.Sqrt(Dot(vector, vector));
    }

    /// <summary>
    /// Вычисляет скалярное произведение двух векторов.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности векторов должны совпадать.</exception>
    public static double Dot(Vector vector1, Vector vector2)
    {
        EnsureSameDimensions(vector1, vector2);
        double dot = 0;
        for (int i = 0; i < vector1.Count; i++)
        {
            dot += vector1[i] * vector2[i];
        }
        return dot;
    }

    /// <summary>
    /// Вычисляет скалярное произведение двух векторов.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности векторов должны совпадать.</exception>
    public static Complex Dot(ComplexVector vector1, ComplexVector vector2)
    {
        EnsureSameDimensions(vector1, vector2);
        Complex dot = 0;
        for (int i = 0; i < vector1.Count; i++)
            dot += vector1[i] * vector2[i];

        return dot;
    }

    /// <summary>
    /// Вычисляет проекцию вектора A на вектор B.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности векторов должны совпадать и B не должен быть нулевым.</exception>
    public static Vector ProjectionAtoB(Vector A, Vector B)
    {
        EnsureSameDimensions(A, B);
        double denominator = Dot(B, B);
        if (denominator == 0)
            throw new ArgumentException("Вектор B не должен быть нулевым для вычисления проекции.");
        double k = Dot(A, B) / denominator;
        return k * B;
    }

    /// <summary>
    /// Вычисляет угол между двумя векторами в радианах.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности векторов должны совпадать и векторы не должны быть нулевыми.</exception>
    public static double AngleVect(Vector vector1, Vector vector2)
    {
        EnsureSameDimensions(vector1, vector2);
        double norm1 = NormVect(vector1);
        double norm2 = NormVect(vector2);
        if (norm1 == 0 || norm2 == 0)
            throw new ArgumentException("Невозможно вычислить угол для нулевого вектора.");
        double cosine = Dot(vector1, vector2) / (norm1 * norm2);
        // Ограничиваем значение cosine в диапазоне [-1, 1] для корректной работы Math.Acos
        cosine = Math.Max(-1.0, Math.Min(1.0, cosine));
        return Math.Acos(cosine);
    }

    /// <summary>
    /// Вычисляет вектор, представляющий разность координат между точками A и B.
    /// </summary>
    /// <exception cref="ArgumentException">Размерности точек должны совпадать.</exception>
    public static Vector VectorFromAToB(Vector pointA, Vector pointB)
    {
        EnsureSameDimensions(pointA, pointB);
        return pointB - pointA;
    }

    /// <summary>
    /// Вычисляет расстояние между точками A и B.
    /// </summary>
    public static double DistanceFromAToB(Vector pointA, Vector pointB)
    {
        return NormVect(VectorFromAToB(pointA, pointB));
    }

    /// <summary>
    /// Поворачивает вектор вокруг начала координат в плоскости, определяемой индексами осей indAx1 и indAx2.
    /// Предполагается, что Vector — это вектор-строка.
    /// </summary>
    /// <param name="inp">Входной вектор (вектор-строка).</param>
    /// <param name="angl">Угол поворота в радианах.</param>
    /// <param name="indAx1">Индекс первой оси плоскости поворота.</param>
    /// <param name="indAx2">Индекс второй оси плоскости поворота.</param>
    /// <returns>Новый вектор после поворота.</returns>
    /// <exception cref="ArgumentException">Если индексы осей выходят за пределы размерности вектора.</exception>
    public static Vector VectorRotate(Vector inp, double angl, int indAx1, int indAx2)
    {
        if (indAx1 < 0 || indAx1 >= inp.Count || indAx2 < 0 || indAx2 >= inp.Count)
            throw new ArgumentException("Индексы осей находятся вне диапазона размерности вектора.");

        // Создаем единичную матрицу размерности inp.Count x inp.Count.
        Matrix rotateMatr = IdentityMatrix(inp.Count);

        // Заполняем элементы матрицы для поворота в плоскости (indAx1, indAx2)
        rotateMatr[indAx1, indAx1] = Math.Cos(angl);
        rotateMatr[indAx2, indAx2] = Math.Cos(angl);
        rotateMatr[indAx1, indAx2] = -Math.Sin(angl);
        rotateMatr[indAx2, indAx1] = Math.Sin(angl);

        // Так как Vector — это вектор-строка, преобразуем его в матрицу-строку (1 x N)
        Matrix vectorMatrix = inp.ToMatrix();
        // Выполняем умножение: (1 x N) * (N x N) = (1 x N)
        Matrix rotatedMatrix = vectorMatrix * rotateMatr;
        return rotatedMatrix.LikeVector();
    }


    /// <summary>
    /// Вектор L2 расстояний от точек до центра фигуры 
    /// (нужен для проверки/построения правильных многомерных фигур,
    /// рассчета точек опор наибольшей площади, проверки на гиперсферу и многомерный шар)
    /// </summary>
    /// <param name="points">Точки</param>
    /// <param name="centr">Центр</param>
    public static Vector L2DistancesPoints2Center(IEnumerable<Vector> points, Vector centr)
    {
        Vector dists = points.Select(point => DistanceFromAToB(point, centr)).ToArray();
        return dists;
    }

    /// <summary>
    /// Вектор L2 расстояний от точек до центра фигуры 
    /// (нужен для проверки/построения правильных многомерных фигур,
    /// рассчета точек опор наибольшей площади, проверки на гиперсферу и многомерный шар)
    /// </summary>
    /// <param name="points">Точки</param>
    public static Vector L2DistancesPoints2Center(IEnumerable<Vector> points)
    {
        Vector centr = Vector.Mean(points.ToArray());
        Vector dists = points.Select(point => DistanceFromAToB(point, centr)).ToArray();
        return dists;
    }



    /// <summary>
    /// Создает единичную матрицу заданного размера.
    /// </summary>
    /// <param name="size">Размер матрицы (количество строк и столбцов).</param>
    /// <returns>Единичная матрица размера size x size.</returns>
    private static Matrix IdentityMatrix(int size)
    {
        Matrix identity = new Matrix(size, size);
        for (int i = 0; i < size; i++)
            identity[i, i] = 1.0;

        return identity;
    }

    /// <summary>
    /// Проверяет, что два вектора имеют одинаковую размерность.
    /// </summary>
    /// <exception cref="ArgumentException">Если размерности не совпадают.</exception>
    private static void EnsureSameDimensions(Vector v1, Vector v2)
    {
        if (v1.Count != v2.Count)
            throw new ArgumentException("Размерности векторов не совпадают.");
    }

    /// <summary>
    /// Проверяет, что два вектора имеют одинаковую размерность.
    /// </summary>
    /// <exception cref="ArgumentException">Если размерности не совпадают.</exception>
    private static void EnsureSameDimensions(ComplexVector v1, ComplexVector v2)
    {
        if (v1.Count != v2.Count)
            throw new ArgumentException("Размерности векторов не совпадают.");
    }

    /// <summary>
    /// Проверяет, что два вектора имеют одинаковую размерность.
    /// </summary>
    /// <exception cref="ArgumentException">Если размерности не совпадают.</exception>
    private static void EnsureSameDimensions(ComplexVector v1, Vector v2)
    {
        if (v1.Count != v2.Count)
            throw new ArgumentException("Размерности векторов не совпадают.");
    }
}