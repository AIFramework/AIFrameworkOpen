using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AI.ClassicMath.Calculator.Libs;

/// <summary>
/// Библиотека базовых операторов
/// </summary>
[Serializable]
public class LibOperatorsBase
{
    public Dictionary<string, (int Precedence, string Associativity)> GetOperators()
    {
        return new Dictionary<string, (int Precedence, string Associativity)>(StringComparer.OrdinalIgnoreCase)
            {
                { "==", (0, "Left") }, { "!=", (0, "Left") },
                { ">",  (0, "Left") }, { "<",  (0, "Left") },
                { ">=", (0, "Left") }, { "<=", (0, "Left") },
                { "+",  (1, "Left") }, { "-",  (1, "Left") },
                { "*",  (2, "Left") }, { "/",  (2, "Left") }, { "%", (2, "Left") },
                { "^",  (3, "Right") }
            };
    }

    public Dictionary<(Type, Type, string), Func<object, object, object>> GetOperationsFunctions()
    {
        // Инициализация словаря со всеми операциями
        Dictionary<(Type, Type, string), Func<object, object, object>> _operations = new()
        {
            // --- Операции Вектор-Вектор ---
            [(typeof(ComplexVector), typeof(ComplexVector), "+")] = (o1, o2) => (ComplexVector)o1 + (ComplexVector)o2,
            [(typeof(ComplexVector), typeof(ComplexVector), "-")] = (o1, o2) => (ComplexVector)o1 - (ComplexVector)o2,
            [(typeof(ComplexVector), typeof(ComplexVector), "*")] = (o1, o2) => (ComplexVector)o1 * (ComplexVector)o2,
            [(typeof(ComplexVector), typeof(ComplexVector), "/")] = (o1, o2) => (ComplexVector)o1 / (ComplexVector)o2,
            [(typeof(ComplexVector), typeof(ComplexVector), "%")] = (o1, o2) => (ComplexVector)o1 % (ComplexVector)o2,
            [(typeof(ComplexVector), typeof(ComplexVector), "^")] = (o1, o2) => ComplexVector.Pow((ComplexVector)o1, (ComplexVector)o2),

            // --- Операции Вектор-Скаляр ---
            [(typeof(ComplexVector), typeof(Complex), "+")] = (o1, o2) => (ComplexVector)o1 + (Complex)o2,
            [(typeof(ComplexVector), typeof(Complex), "-")] = (o1, o2) => (ComplexVector)o1 - (Complex)o2,
            [(typeof(ComplexVector), typeof(Complex), "*")] = (o1, o2) => (ComplexVector)o1 * (Complex)o2,
            [(typeof(ComplexVector), typeof(Complex), "/")] = (o1, o2) => (ComplexVector)o1 / (Complex)o2,
            [(typeof(ComplexVector), typeof(Complex), "%")] = (o1, o2) => (ComplexVector)o1 % (Complex)o2,
            [(typeof(ComplexVector), typeof(Complex), "^")] = (o1, o2) => ComplexVector.Pow((ComplexVector)o1, (Complex)o2),

            // --- Операции Скаляр-Вектор (обратный порядок) ---
            [(typeof(Complex), typeof(ComplexVector), "+")] = (o1, o2) => (ComplexVector)o2 + (Complex)o1,
            [(typeof(Complex), typeof(ComplexVector), "-")] = (o1, o2) => (Complex)o1 - (ComplexVector)o2,
            [(typeof(Complex), typeof(ComplexVector), "*")] = (o1, o2) => (ComplexVector)o2 * (Complex)o1,
            [(typeof(Complex), typeof(ComplexVector), "/")] = (o1, o2) => (Complex)o1 / (ComplexVector)o2,
            [(typeof(Complex), typeof(ComplexVector), "%")] = (o1, o2) => (Complex)o1 % (ComplexVector)o2,
            [(typeof(Complex), typeof(ComplexVector), "^")] = (o1, o2) => ComplexVector.Pow((Complex)o1, (ComplexVector)o2),

            // --- Операции Скаляр-Скаляр ---
            [(typeof(Complex), typeof(Complex), "+")] = (o1, o2) => (Complex)o1 + (Complex)o2,
            [(typeof(Complex), typeof(Complex), "-")] = (o1, o2) => (Complex)o1 - (Complex)o2,
            [(typeof(Complex), typeof(Complex), "*")] = (o1, o2) => (Complex)o1 * (Complex)o2,
            [(typeof(Complex), typeof(Complex), "/")] = (o1, o2) => (Complex)o1 / (Complex)o2,
            [(typeof(Complex), typeof(Complex), "^")] = (o1, o2) => Complex.Pow((Complex)o1, (Complex)o2),
            [(typeof(Complex), typeof(Complex), "%")] = (o1, o2) => new Complex(((Complex)o1).Real % ((Complex)o2).Real, 0),

            // --- Операции сравнения (возвращают double, который будет упакован в object) ---
            [(typeof(Complex), typeof(Complex), ">")] = (o1, o2) => ((Complex)o1).Real > ((Complex)o2).Real ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), "<")] = (o1, o2) => ((Complex)o1).Real < ((Complex)o2).Real ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), ">=")] = (o1, o2) => ((Complex)o1).Real >= ((Complex)o2).Real ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), "<=")] = (o1, o2) => ((Complex)o1).Real <= ((Complex)o2).Real ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), "==")] = (o1, o2) => (Complex)o1 == (Complex)o2 ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), "!=")] = (o1, o2) => (Complex)o1 != (Complex)o2 ? 1.0 : 0.0
        };

        return _operations;
    }
}
