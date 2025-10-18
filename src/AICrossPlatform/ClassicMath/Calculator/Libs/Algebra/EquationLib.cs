using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.ClassicMath.Calculator.Libs.Algebra;

/// <summary>
/// Предоставляет функции для решения алгебраических уравнений.
/// </summary>
[Serializable]
public class EquationLib : IMathLib
{
    /// <summary>
    /// Имя библиотеки
    /// </summary>
    public string Name { get; set; } = "Решение алгебраических уравнений";

    /// <summary>
    /// Описание библиотеки
    /// </summary>
    public string Description { get; set; } = "Библиотека содержит функции для нахождения корней полиномиальных уравнений с 1-й по 4-ю степень с вещественными или комплексными коэффициентами.";

    /// <summary>
    /// Возвращает словарь доступных функций.
    /// </summary>
    public Dictionary<string, FunctionDefinition> GetFunctions()
    {
        var linear = new LinearEquationSolver();
        var quadratic = new QuadraticEquationSolver();
        var cubic = new CubicEquationSolver();
        var quartic = new QuarticEquationSolver();

        return new Dictionary<string, FunctionDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            { linear.Name, linear },
            { quadratic.Name, quadratic },
            { cubic.Name, cubic },
            { quartic.Name, quartic },
        };
    }
}

//======================================================================================
// РЕШАТЕЛИ УРАВНЕНИЙ
//======================================================================================

/// <summary>
/// Решатель линейного уравнения вида ax + b = 0.
/// </summary>
[Serializable]
public class LinearEquationSolver : FunctionDefinition
{
    public LinearEquationSolver()
    {
        ArgumentCount = 2;
        Name = "LinearEquationSolver";
        Delegate = (x) => Solve(CastsVar.CastToComplex(x[0], Name), CastsVar.CastToComplex(x[1], Name));
        Description = new DescriptionFunction()
        {
            AreaList = ["Алгебра", "Физика", "Экономика", "Статистика"],
            Description = "Находит корень линейного уравнения вида ax + b = 0.",
            Signature = "Вход: 2 коэффициента (a, b). Выход: 1 комплексное число (корень x).",
            Exemple = @"// Решение уравнения 5x - 10 = 0
LinearEquationSolver(5, -10)"
        };
    }

    /// <summary>
    /// Находит корень уравнения ax + b = 0.
    /// </summary>
    /// <param name="args">Коэффициенты a и b.</param>
    /// <returns>Корень уравнения x.</returns>
    public static Complex Solve(params Complex[] args)
    {
        Complex a = args[0];
        Complex b = args[1];

        if (a == Complex.Zero)
            throw new ArgumentException("Коэффициент 'a' не может быть равен нулю в линейном уравнении.");

        return -b / a;
    }
}

/// <summary>
/// Решатель квадратного уравнения вида ax² + bx + c = 0.
/// </summary>
[Serializable]
public class QuadraticEquationSolver : FunctionDefinition
{
    public QuadraticEquationSolver()
    {
        ArgumentCount = 3;
        Name = "QuadraticEquationSolver";
        Delegate = (x) => Solve(
            CastsVar.CastToComplex(x[0], Name),
            CastsVar.CastToComplex(x[1], Name),
            CastsVar.CastToComplex(x[2], Name)
        );
        Description = new DescriptionFunction()
        {
            AreaList = ["Алгебра", "Физика", "Геометрия", "Инженерия", "Экономика"],
            Description = "Находит корни квадратного уравнения вида ax² + bx + c = 0.",
            Signature = "Вход: 3 коэффициента (a, b, c). Выход: Вектор из 2 комплексных чисел (корни x₁ и x₂).",
            Exemple = @"// Решение уравнения 2x² - 4x - 6 = 0
QuadraticEquationSolver(2, -4, -6)"
        };
    }

    /// <summary>
    /// Находит корни уравнения ax² + bx + c = 0.
    /// </summary>
    /// <param name="args">Коэффициенты a, b и c.</param>
    /// <returns>Вектор из двух комплексных корней.</returns>
    public static ComplexVector Solve(params Complex[] args)
    {
        Complex a = args[0];
        Complex b = args[1];
        Complex c = args[2];

        if (a == Complex.Zero)
            throw new ArgumentException("Коэффициент 'a' не может быть равен нулю в квадратном уравнении.");

        Complex discriminant = b * b - 4 * a * c;
        Complex sqrtDiscriminant = Complex.Sqrt(discriminant);

        return new Complex[]
        {
            (-b + sqrtDiscriminant) / (2 * a),
            (-b - sqrtDiscriminant) / (2 * a)
        };
    }
}

/// <summary>
/// Решатель кубического уравнения вида ax³ + bx² + cx + d = 0.
/// </summary>
[Serializable]
public class CubicEquationSolver : FunctionDefinition
{
    public CubicEquationSolver()
    {
        ArgumentCount = 4;
        Name = "CubicEquationSolver";
        Delegate = (x) => Solve(
            CastsVar.CastToComplex(x[0], Name),
            CastsVar.CastToComplex(x[1], Name),
            CastsVar.CastToComplex(x[2], Name),
            CastsVar.CastToComplex(x[3], Name)
        );
        Description = new DescriptionFunction()
        {
            AreaList = ["Алгебра", "Физика", "Инженерия", "Гидродинамика"],
            Description = "Находит все три корня кубического уравнения вида ax³ + bx² + cx + d = 0, используя формулу Кардано.",
            Signature = "Вход: 4 коэффициента (a, b, c, d). Выход: Вектор из 3 комплексных чисел (корни x₁, x₂, x₃).",
            Exemple = @"// Решение уравнения x³ - 6x² + 11x - 6 = 0 
CubicEquationSolver(1, -6, 11, -6)"
        };
    }

    /// <summary>
    /// Находит корни уравнения ax³ + bx² + cx + d = 0 по формуле Кардано.
    /// </summary>
    /// <param name="args">Коэффициенты a, b, c и d.</param>
    /// <returns>Вектор из трёх комплексных корней.</returns>
    public static ComplexVector Solve(params Complex[] args)
    {
        // Необходимые using: System.Numerics, AI.DataStructs.WithComplexElements
        Complex a = args[0], b = args[1], c = args[2], d = args[3];

        if (a == Complex.Zero)
            throw new ArgumentException("Коэффициент 'a' не может быть равен нулю в кубическом уравнении.");

        // Код решения остается без изменений, но убедимся, что он возвращает ComplexVector
        Complex[] roots = CubicEquationSolverHelper.SolveCubic(a, b, c, d);
        return new ComplexVector(roots);
    }
}

/// <summary>
/// Решатель уравнения четвёртой степени вида ax⁴ + bx³ + cx² + dx + e = 0.
/// </summary>
[Serializable]
public class QuarticEquationSolver : FunctionDefinition
{
    public QuarticEquationSolver()
    {
        ArgumentCount = 5;
        Name = "QuarticEquationSolver";
        Delegate = (x) => Solve(
            CastsVar.CastToComplex(x[0], Name),
            CastsVar.CastToComplex(x[1], Name),
            CastsVar.CastToComplex(x[2], Name),
            CastsVar.CastToComplex(x[3], Name),
            CastsVar.CastToComplex(x[4], Name)
        );
        Description = new DescriptionFunction()
        {
            AreaList = ["Алгебра", "Физика", "Инженерия", "Теория управления"],
            Description = "Находит все четыре корня уравнения четвёртой степени вида ax⁴ + bx³ + cx² + dx + e = 0, используя метод Феррари.",
            Signature = "Вход: 5 коэффициентов (a, b, c, d, e). Выход: Вектор из 4 комплексных чисел (корни x₁, x₂, x₃, x₄).",
            Exemple = @"// Решение x⁴ - 10x³ + 35x² - 50x + 24 = 0 
QuarticEquationSolver(1, -10, 35, -50, 24)"
        };
    }

    /// <summary>
    /// Находит корни уравнения ax⁴ + bx³ + cx² + dx + e = 0 методом Феррари.
    /// </summary>
    /// <param name="args">Коэффициенты a, b, c, d и e.</param>
    /// <returns>Вектор из четырёх комплексных корней.</returns>
    public static ComplexVector Solve(params Complex[] args)
    {
        Complex a = args[0], b = args[1], c = args[2], d = args[3], e = args[4];

        if (a == Complex.Zero)
            throw new ArgumentException("Коэффициент 'a' не может быть равен нулю в уравнении четвёртой степени.");

        // Нормализация и приведение к depressed quartic y^4 + py^2 + qy + r = 0
        Complex B = b / a, C = c / a, D = d / a, E = e / a;
        Complex p = C - 3 * B * B / 8;
        Complex q = D + B * B * B / 8 - B * C / 2;
        Complex r = E - 3 * B * B * B * B / 256 + B * B * C / 16 - B * D / 4;

        Complex[] rootsY; // Корни для приведённого уравнения

        if (Complex.Abs(q) < 1e-12) // Биквадратное уравнение
        {
            ComplexVector quadraticRoots = QuadraticEquationSolver.Solve(1, p, r);
            Complex sqrt1 = Complex.Sqrt(quadraticRoots[0]);
            Complex sqrt2 = Complex.Sqrt(quadraticRoots[1]);
            rootsY = new[] { sqrt1, -sqrt1, sqrt2, -sqrt2 };
        }
        else // Общий случай
        {
            // Решение резольвентного кубического уравнения: z^3 + 2pz^2 + (p^2 - 4r)z - q^2 = 0
            Complex[] cubicRoots = CubicEquationSolverHelper.SolveCubic(1, 2 * p, p * p - 4 * r, -q * q);
            Complex z = cubicRoots.First(root => root != Complex.Zero);

            Complex sqrtZ = Complex.Sqrt(z);
            Complex commonTerm = (p + z) / 2; // Вычисляем (p/2 + z/2)
            Complex qTerm = q / (2 * sqrtZ);   // Вычисляем q / (2*sqrt(z))

            Complex c1 = commonTerm - qTerm;
            Complex c2 = commonTerm + qTerm;

            ComplexVector quadRoots1 = QuadraticEquationSolver.Solve(1, sqrtZ, c1);
            ComplexVector quadRoots2 = QuadraticEquationSolver.Solve(1, -sqrtZ, c2);

            rootsY = [quadRoots1[0], quadRoots1[1], quadRoots2[0], quadRoots2[1] ];
        }

        // Обратная замена x = y - B/4
        Complex offset = B / 4;
        for (int i = 0; i < 4; i++)
        {
            rootsY[i] -= offset;
        }

        return rootsY;
    }
}

/// <summary>
/// Вспомогательный класс с реализацией алгоритма решения кубического уравнения.
/// </summary>
internal static class CubicEquationSolverHelper
{
    public static Complex[] SolveCubic(Complex a, Complex b, Complex c, Complex d)
    {
        Complex p = (3 * a * c - b * b) / (3 * a * a);
        Complex q = (2 * b * b * b - 9 * a * b * c + 27 * a * a * d) / (27 * a * a * a);

        Complex sqrtDiscriminant = Complex.Sqrt(q * q / 4 + p * p * p / 27);
        Complex u = Complex.Pow(-q / 2 + sqrtDiscriminant, 1.0 / 3.0);

        Complex omega1 = new Complex(-0.5, Math.Sqrt(3) / 2.0);
        Complex omega2 = new Complex(-0.5, -Math.Sqrt(3) / 2.0);
        Complex offset = b / (3 * a);

        Complex[] roots = new Complex[3];

        if (u == Complex.Zero)
        {
            roots[0] = -offset;
            roots[1] = -offset;
            roots[2] = -offset;
        }
        else
        {
            Complex v1 = -p / (3 * u);
            Complex v2 = -p / (3 * u * omega1);
            Complex v3 = -p / (3 * u * omega2);

            roots[0] = (u + v1) - offset;
            roots[1] = (u * omega1 + v2) - offset;
            roots[2] = (u * omega2 + v3) - offset;
        }
        return roots;
    }
}