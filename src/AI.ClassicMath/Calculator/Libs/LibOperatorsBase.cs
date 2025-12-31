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
        // ИСПРАВЛЕНИЕ: Правильные приоритеты битовых операторов (как в C/C++)
        // От низкого к высокому: || < && < | < & < сравнения < сдвиги < +- < */ < ^ < унарные
        return new Dictionary<string, (int Precedence, string Associativity)>(StringComparer.OrdinalIgnoreCase)
            {
                { "||", (0, "Left") },  // Логическое ИЛИ (самый низкий)
                { "&&", (1, "Left") },  // Логическое И
                { "|", (2, "Left") },   // Bitwise OR
                { "&", (3, "Left") },   // Bitwise AND (выше чем |)
                { "==", (4, "Left") }, { "!=", (4, "Left") },
                { ">",  (4, "Left") }, { "<",  (4, "Left") },
                { ">=", (4, "Left") }, { "<=", (4, "Left") },
                { "<<", (5, "Left") }, { ">>", (5, "Left") }, // Bit shifts
                { "+",  (6, "Left") }, { "-",  (6, "Left") },
                { "*",  (7, "Left") }, { "/",  (7, "Left") }, { "%", (7, "Left") },
                { "^",  (8, "Right") }, // Возведение в степень
                { "~",  (9, "Right") }, // Унарный минус / битовое НЕ
                { "!",  (9, "Right") }  // Логическое НЕ (унарное)
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
            [(typeof(Complex), typeof(Complex), "!=")] = (o1, o2) => (Complex)o1 != (Complex)o2 ? 1.0 : 0.0,
            
            // --- Операции сравнения строк ---
            [(typeof(string), typeof(string), "==")] = (o1, o2) => (string)o1 == (string)o2 ? 1.0 : 0.0,
            [(typeof(string), typeof(string), "!=")] = (o1, o2) => (string)o1 != (string)o2 ? 1.0 : 0.0,
            
            // --- Логические операции ---
            [(typeof(Complex), typeof(Complex), "&&")] = (o1, o2) => 
                (((Complex)o1).Real != 0 && ((Complex)o2).Real != 0) ? 1.0 : 0.0,
            [(typeof(Complex), typeof(Complex), "||")] = (o1, o2) => 
                (((Complex)o1).Real != 0 || ((Complex)o2).Real != 0) ? 1.0 : 0.0,
            
            // --- Bitwise операции (работают только с целыми числами) ---
            [(typeof(Complex), typeof(Complex), "&")] = (o1, o2) => 
                new Complex((int)((Complex)o1).Real & (int)((Complex)o2).Real, 0),
            [(typeof(Complex), typeof(Complex), "|")] = (o1, o2) => 
                new Complex((int)((Complex)o1).Real | (int)((Complex)o2).Real, 0),
            [(typeof(Complex), typeof(Complex), "<<")] = (o1, o2) => 
                new Complex((int)((Complex)o1).Real << (int)((Complex)o2).Real, 0),
            [(typeof(Complex), typeof(Complex), ">>")] = (o1, o2) => 
                new Complex((int)((Complex)o1).Real >> (int)((Complex)o2).Real, 0),
            
            // --- Операции с датами ---
            // Сравнение дат
            [(typeof(DateTime), typeof(DateTime), ">")] = (o1, o2) => (DateTime)o1 > (DateTime)o2 ? 1.0 : 0.0,
            [(typeof(DateTime), typeof(DateTime), "<")] = (o1, o2) => (DateTime)o1 < (DateTime)o2 ? 1.0 : 0.0,
            [(typeof(DateTime), typeof(DateTime), ">=")] = (o1, o2) => (DateTime)o1 >= (DateTime)o2 ? 1.0 : 0.0,
            [(typeof(DateTime), typeof(DateTime), "<=")] = (o1, o2) => (DateTime)o1 <= (DateTime)o2 ? 1.0 : 0.0,
            [(typeof(DateTime), typeof(DateTime), "==")] = (o1, o2) => (DateTime)o1 == (DateTime)o2 ? 1.0 : 0.0,
            [(typeof(DateTime), typeof(DateTime), "!=")] = (o1, o2) => (DateTime)o1 != (DateTime)o2 ? 1.0 : 0.0,
            
            // Добавление/вычитание дней к дате
            [(typeof(DateTime), typeof(Complex), "+")] = (o1, o2) => ((DateTime)o1).AddDays(((Complex)o2).Real),
            [(typeof(DateTime), typeof(Complex), "-")] = (o1, o2) => ((DateTime)o1).AddDays(-((Complex)o2).Real),
            [(typeof(Complex), typeof(DateTime), "+")] = (o1, o2) => ((DateTime)o2).AddDays(((Complex)o1).Real)
        };

        return _operations;
    }
}
