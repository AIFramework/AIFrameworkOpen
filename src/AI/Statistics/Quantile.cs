using AI.DataStructs.Algebraic;
using AI.Extensions;
using System;
using System.Runtime.CompilerServices;

namespace AI.Statistics;

/// <summary>
/// Квантили
/// </summary>
[Serializable]
public class Quantile
{
    private readonly int _max;

    /// <summary>
    /// Сортированный вектор
    /// </summary>
    public Vector SortVec { get; private set; }

    /// <summary>
    /// Квантили
    /// </summary>
    /// <param name="structureDouble">Алгебраическая структура</param>
    public Quantile(IAlgebraicStructure<double> structureDouble)
    {
        SortVec = structureDouble.Data;
        SortVec.Sort();
        _max = SortVec.Count - 1;
    }

    /// <summary>
    /// Расчет нужного квантиля (0-1)
    /// </summary>
    /// <param name="q">Квантиль</param>
    public double GetQuantile(double q)
    {
        int index = (int)(q * _max);

        if (index > _max)
        {
            throw new InvalidOperationException("Квантиль задан некорректно");
        }

        return SortVec[index];
    }

    /// <summary>
    /// Быстрый расчет квантиля, не требует сортировки всего массива, работает через QSearch
    /// </summary>
    /// <param name="structure">Входные данные</param>
    /// <param name="q">Квантиль</param>
    /// <returns></returns>
    public static double FastQuantile(IAlgebraicStructure<double> structure, double q)
    {
        double[] data = new double[structure.Shape.Count];
        Array.Copy(structure.Data, data, data.Length);

        if (q > 1) q = 1;
        else if (q < 0) q = 0;

        int ordinal = (int)(q * data.Length - 1);
        return QuickSelection<double>.Selection(data, ordinal);
    }


}


/// <summary>
/// Методы быстрого поиска, за линейное время
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class QuickSelection<T>
    where T : IComparable<T>
{

    /// <summary>
    /// Выбор на основе быстрой сортировки(находит порядковую статистику за линейное время)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Selection(T[] data, int orderStatistic)
    {
        data.Shuffle();

        int e = data.Length - 1, s = 0;

        while (e > s)
        {
            int j = Partitions(data, s, e);
            if (j < orderStatistic) s = j + 1;
            else if (j > orderStatistic) e = j - 1;
            else return data[orderStatistic];
        }
        return data[orderStatistic];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Partitions(T[] data, int s, int e)
    {
        int s1 = s, e1 = e + 1;

        while (true)
        {
            while (Less(data[++s1], data[s]))
                if (s1 == e) break;

            while (Less(data[s], data[--e1]))
                if (e1 == s) break;

            if (s1 >= e1)
                break;

            ExCh(data, s1, e1);
        }

        ExCh(data, s, e1);

        return e1;
    }

    // Функция сравнения
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Less(T t1, T t2)
    {
        return t1.CompareTo(t2) <= 0;
    }

    // Функция замены позиций, k и z меняются местами
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExCh(T[] data, int k, int z)
    {
        T mid = data[k];
        data[k] = data[z];
        data[z] = mid;
    }
}