using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Linq;
using System.Numerics;

namespace AI.ClassicMath.Calculator;

/// <summary>
/// Работа с различными типами данных (нетипизированными)
/// </summary>
[Serializable]
public static class CastsVar
{
    /// <summary>
    /// Преобразует переменную в комплексное число
    /// </summary>
    public static Complex CastToComplex(object obj, string funcName)
    {
        if (obj is Complex c) return c;
        if (obj is double d) return new Complex(d, 0);
        throw new ArgumentException($"Функция '{funcName}' ожидает скалярный аргумент, но получила {obj.GetType().Name}.");
    }

    /// <summary>
    /// Преобразует переменную в вещественное число
    /// </summary>
    public static double CastToDouble(object obj, string funcName)
    {
        var c = CastToComplex(obj, funcName);
        if (Math.Abs(c.Imaginary) > 1e-12) throw new ArgumentException($"Функция '{funcName}' ожидает действительный аргумент, но получила комплексное число {c}.");
        return c.Real;
    }

    /// <summary>
    /// Преобразует переменную в комплексный вектор
    /// </summary>
    public static ComplexVector CastToComplexVector(object obj, string funcName)
    {
        if (obj is ComplexVector cv) return cv;
        if (obj is Vector rv) return rv.Select(c => new Complex(c, 0)).ToArray();
        throw new ArgumentException($"Функция '{funcName}' ожидает векторный аргумент, но получила {obj.GetType().Name}.");
    }

    /// <summary>
    /// Преобразует переменную в вещественный вектор
    /// </summary>
    public static Vector CastToRealVector(object obj, string funcName)
    {
        if (obj is Vector rv) return rv;
        if (obj is ComplexVector cv)
        {
            if (cv.Any(c => Math.Abs(c.Imaginary) > 1e-12))
                throw new ArgumentException($"Функция '{funcName}' ожидает вектор с действительными компонентами.");
            return cv.Select(c => c.Real).ToArray();
        }
        throw new ArgumentException($"Функция '{funcName}' ожидает векторный аргумент, но получила {obj.GetType().Name}.");
    }
}
