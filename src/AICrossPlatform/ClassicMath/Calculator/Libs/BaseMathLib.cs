using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.HightLevelFunctions;
using AI.ML.Distances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AI.ClassicMath.Calculator.Libs;

/// <summary>
/// Базовая библиотека функций
/// </summary>
[Serializable]
public class BaseMathLib : IMathLib
{
    /// <summary>
    /// Имя библиотеки
    /// </summary>
    public string Name { get; set; } = "Библиотека для поддержки базовых операций";

    /// <summary>
    /// Описание библиотеки
    /// </summary>
    public string Description { get; set; } = "Библиотека для поддержки базовых операций, для чисел в т.ч. с комплексным аргументом sin, cos, tan, sqrt, ln и т.п.";

    /// <summary>
    /// Отдает базовые функции
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Dictionary<string, FunctionDefinition> GetFunctions()
    {
        Dictionary<string, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            { "round", new(1, args => (Complex)Math.Round(CastsVar.CastToDouble(args[0], "round"))) },

            { "sin", new(1, args => (Complex)Complex.Sin(CastsVar.CastToComplex(args[0], "sin"))) },

            { "asin", new(1, args => (Complex)Complex.Asin(CastsVar.CastToComplex(args[0], "asin"))) },

            { "cos", new(1, args => (Complex)Complex.Acos(CastsVar.CastToComplex(args[0], "cos"))) },

            { "acos", new(1, args => (Complex)Complex.Cos(CastsVar.CastToComplex(args[0], "acos"))) },

            { "tan", new(1, args => (Complex)Complex.Tan(CastsVar.CastToComplex(args[0], "tan"))) },

            { "tanh", new(1, args => (Complex)Complex.Tanh(CastsVar.CastToComplex(args[0], "tanh"))) },

            { "atan", new(1, args => (Complex)Complex.Atan(CastsVar.CastToComplex(args[0], "atan"))) },

            { "sqrt", new(1, args => (Complex)Complex.Sqrt(CastsVar.CastToComplex(args[0], "sqrt"))) },

            { "ln", new(1, args => (Complex)Complex.Log(CastsVar.CastToComplex(args[0], "ln"))) },

            { "log10", new(1, args => (Complex)Complex.Log10(CastsVar.CastToComplex(args[0], "log10"))) },

            { "exp", new(1, args => (Complex)Complex.Exp(CastsVar.CastToComplex(args[0], "exp"))) },

            { "abs", new(1, args => (Complex)Complex.Abs(CastsVar.CastToComplex(args[0], "abs"))) },

            { "rad", new(1, args => (Complex)FunctionsForEachElements.GradToRad(CastsVar.CastToDouble(args[0], "rad"))) },

            { "deg", new(1, args => (Complex)FunctionsForEachElements.RadToGrad(CastsVar.CastToDouble(args[0], "deg"))) },

            { "gamma", new(1, args => (Complex)FunctionsForEachElements.Gamma(CastsVar.CastToDouble(args[0], "gamma"))) },

            { "fact", new(1, args => (Complex)FunctionsForEachElements.Factorial((int)CastsVar.CastToDouble(args[0], "fact"))) },

            { "mag", new(1, args => (Complex)BaseDist.L2(CastsVar.CastToComplexVector(args[0], "mag"))) },

            { "sum", new(1, args => (Complex)Sum(CastsVar.CastToComplexVector(args[0], "sum"))) },

            { "pow", new(2, args => Complex.Pow(CastsVar.CastToComplex(args[0], "pow"), CastsVar.CastToComplex(args[1], "pow"))) },

            { "log", new(2, args => (Complex)Complex.Log(CastsVar.CastToComplex(args[0], "log value")) / Complex.Log(CastsVar.CastToComplex(args[1], "log base"))) },

            { "C", new(2, args =>  (Complex)MathUtils.Combinatorics.CombinatoricsBaseFunction.Combinations((int)CastsVar.CastToDouble(args[0], "C"), (int)CastsVar.CastToDouble(args[1], "C"))) },

            { "P", new(2, args => {
                                        // k-перестановки из n по формуле n! / (n-k)!
                                        var n = (int)CastsVar.CastToDouble(args[0], "P");
                                        var k = (int)CastsVar.CastToDouble(args[1], "P");
                                        return (Complex)(FunctionsForEachElements.Factorial(n) / FunctionsForEachElements.Factorial(n - k));
                                    })},

            { "dot", new(2, args => (Complex)AnalyticGeometryFunctions.Dot(CastsVar.CastToComplexVector(args[0], "dot"), (CastsVar.CastToComplexVector(args[1], "dot")))) },

            { "cross", new(2, args =>
                {
                    var v1 = CastsVar.CastToRealVector(args[0], "cross"); var v2 = CastsVar.CastToRealVector(args[1], "cross");
                    if (v1.Count != 3 || v2.Count != 3) throw new ArgumentException("Функция 'cross' определена только для 3D-векторов.");
                    var r = new Vector(3);
                    r[0] = v1[1] * v2[2] - v1[2] * v2[1]; r[1] = v1[2] * v2[0] - v1[0] * v2[2]; r[2] = v1[0] * v2[1] - v1[1] * v2[0];
                    return r;
                })
            },

            { "mean", new(-1, args => {
                var complexArgs = args.Select(a => CastsVar.CastToComplex(a, "mean")).ToArray();
                if (!complexArgs.Any()) return Complex.Zero;
                return new Complex(complexArgs.Average(c => c.Real), complexArgs.Average(c => c.Imaginary));
              })
            },

            { "min", new(-1, args => new Complex(args.Select(a => CastsVar.CastToDouble(a, "min")).Min(), 0)) },

            { "max", new(-1, args => new Complex(args.Select(a => CastsVar.CastToDouble(a, "max")).Max(), 0)) },
        };

        return functions;
    }

    // ToDo: добавить в вектор
    private static Complex Sum(ComplexVector complexes)
    {
        Complex sum = new Complex(0, 0);
        foreach (var complex in complexes)
            sum += complex;
        return sum;
    }
}
