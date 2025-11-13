// BaseMathLib.cs

using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using AI.Distances;
using AI.HightLevelFunctions;
using AI.MathUtils.Combinatorics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AI.ClassicMath.Calculator.Libs
{
    /// <summary>
    /// Предоставляет базовые математические и векторные функции.
    /// </summary>
    [Serializable]
    public class BaseMathLib : IMathLib
    {
        /// <summary>
        /// Имя библиотеки.
        /// </summary>
        public string Name
        {
            get;
            set;
        } = "Базовые математические функции";

        /// <summary>
        /// Описание библиотеки.
        /// </summary>
        public string Description
        {
            get;
            set;
        } = "Библиотека содержит тригонометрические, логарифмические, статистические и другие базовые функции, работающие с вещественными и комплексными числами.";

        /// <summary>
        /// Собирает и возвращает словарь доступных функций.
        /// </summary>
        public Dictionary<string, FunctionDefinition> GetFunctions()
        {
            var functions = new List<FunctionDefinition> {
        // Стандартная математика
        CreateRoundFunction(),
        CreateFloorFunction(),
        CreateAbsFunction(),
        CreateSqrtFunction(),
        CreateCbrtFunction(),
        CreatePowFunction(),

        // Тригонометрия
        CreateSinFunction(),
        CreateCosFunction(),
        CreateTanFunction(),
        CreateAsinFunction(),
        CreateAcosFunction(),
        CreateAtanFunction(),
        CreateTanhFunction(),

        // Логарифмы и экспонента
        CreateLnFunction(),
        CreateLog10Function(),
        CreateLogFunction(),
        CreateExpFunction(),

        // Угловые меры
        CreateRadFunction(),
        CreateDegFunction(),

        // Комбинаторика и специальные функции
        CreateFactFunction(),
        CreateGammaFunction(),
        CreateCombFunction(),
        CreateCombPFunction(),

        // Векторные операции
        CreateMagFunction(),
        CreateSumFunction(),
        CreateDotFunction(),
        CreateCrossFunction(),
        CreateIndexFunction(),

        // Статистика
        CreateMeanFunction(),
        CreateMinFunction(),
        CreateMaxFunction()
      };

            return functions.ToDictionary(f => f.Name, f => f, StringComparer.OrdinalIgnoreCase);
        }

        #region Function Factory Methods

        //================== Стандартная математика ==================

        private static FunctionDefinition CreateRoundFunction()
        {
            const string name = "round";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)Math.Round(CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Программирование"],
                    Description = "Округляет вещественное число до ближайшего целого.",
                    Signature = "Вход: 1 число. Выход: 1 округлённое число.",
                    Exemple = "round(3.59) // Результат: 4"
                }
            };
        }

        private static FunctionDefinition CreateFloorFunction()
        {
            const string name = "floor";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)(int)CastsVar.CastToDouble(args[0], name),
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Программирование"],
                    Description = "Округляет вещественное число вниз до ближайшего целого.",
                    Signature = "Вход: 1 число. Выход: 1 округлённое число.",
                    Exemple = "round(3.59) // Результат: 3"
                }
            };
        }

        private static FunctionDefinition CreateAbsFunction() => CreateUnaryComplexFunction("abs", x => (Complex)Complex.Abs(x),
          new DescriptionFunction
          {
              AreaList = ["Алгебра", "Геометрия", "Физика"],
              Description = "Вычисляет абсолютное значение (модуль) числа.",
              Signature = "Вход: 1 число. Выход: 1 вещественное число.",
              Exemple = "abs(3 - 4i) // Результат: 5"
          });

        private static FunctionDefinition CreateSqrtFunction() => CreateUnaryComplexFunction("sqrt", Complex.Sqrt,
          new DescriptionFunction
          {
              AreaList = ["Алгебра", "Геометрия", "Физика"],
              Description = "Вычисляет квадратный корень из числа.",
              Signature = "Вход: 1 число. Выход: 1 комплексное число (корень).",
              Exemple = "sqrt(-4) // Результат: 2i"
          });

        private static FunctionDefinition CreateCbrtFunction()
        {
            const string name = "cbrt";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => Complex.Pow(CastsVar.CastToComplex(args[0], name), 1.0 / 3.0),
                Description = new DescriptionFunction
                {
                    AreaList = ["Алгебра", "Инженерия", "Физика"],
                    Description = "Вычисляет кубический корень из числа.",
                    Signature = "Вход: 1 число. Выход: 1 комплексное число (главное значение корня).",
                    Exemple = "cbrt(-8) // Результат: 1 + 1.732i"
                }
            };
        }

        private static FunctionDefinition CreatePowFunction() => CreateBinaryComplexFunction("pow", Complex.Pow,
          new DescriptionFunction
          {
              AreaList = ["Алгебра", "Финансы", "Физика"],
              Description = "Возводит число в указанную степень.",
              Signature = "Вход: 2 числа (основание, степень). Выход: 1 число.",
              Exemple = "pow(2, 10) // Результат: 1024"
          });

        //================== Тригонометрия ==================

        private static FunctionDefinition CreateSinFunction() => CreateUnaryComplexFunction("sin", Complex.Sin, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет синус числа (аргумент в радианах).",
            Signature = "Вход: 1 число (угол в радианах). Выход: 1 число.",
            Exemple = "sin(pi/2) // Результат: 1"
        });
        private static FunctionDefinition CreateCosFunction() => CreateUnaryComplexFunction("cos", Complex.Cos, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет косинус числа (аргумент в радианах).",
            Signature = "Вход: 1 число (угол в радианах). Выход: 1 число.",
            Exemple = "cos(pi) // Результат: -1"
        });
        private static FunctionDefinition CreateTanFunction() => CreateUnaryComplexFunction("tan", Complex.Tan, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет тангенс числа (аргумент в радианах).",
            Signature = "Вход: 1 число (угол в радианах). Выход: 1 число.",
            Exemple = "tan(pi/4) // Результат: 1"
        });
        private static FunctionDefinition CreateAsinFunction() => CreateUnaryComplexFunction("asin", Complex.Asin, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет арксинус числа. Результат в радианах.",
            Signature = "Вход: 1 число. Выход: 1 число (угол в радианах).",
            Exemple = "asin(1) // Результат: pi/2"
        });
        private static FunctionDefinition CreateAcosFunction() => CreateUnaryComplexFunction("acos", Complex.Acos, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет арккосинус числа. Результат в радианах.",
            Signature = "Вход: 1 число. Выход: 1 число (угол в радианах).",
            Exemple = "acos(-1) // Результат: pi"
        });
        private static FunctionDefinition CreateAtanFunction() => CreateUnaryComplexFunction("atan", Complex.Atan, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Геометрия", "Физика"],
            Description = "Вычисляет арктангенс числа. Результат в радианах.",
            Signature = "Вход: 1 число. Выход: 1 число (угол в радианах).",
            Exemple = "atan(1) // Результат: pi/4"
        });
        private static FunctionDefinition CreateTanhFunction() => CreateUnaryComplexFunction("tanh", Complex.Tanh, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Физика", "Нейронные сети"],
            Description = "Вычисляет гиперболический тангенс числа.",
            Signature = "Вход: 1 число. Выход: 1 число.",
            Exemple = "tanh(1)"
        });

        //================== Логарифмы и экспонента ==================

        private static FunctionDefinition CreateLnFunction() => CreateUnaryComplexFunction("ln", Complex.Log, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Теория информации", "Физика"],
            Description = "Вычисляет натуральный логарифм (по основанию e) числа.",
            Signature = "Вход: 1 число. Выход: 1 число.",
            Exemple = "ln(e) // Результат: 1"
        });
        private static FunctionDefinition CreateLog10Function() => CreateUnaryComplexFunction("log10", Complex.Log10, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Химия", "Инженерия"],
            Description = "Вычисляет десятичный логарифм (по основанию 10) числа.",
            Signature = "Вход: 1 число. Выход: 1 число.",
            Exemple = "log10(100) // Результат: 2"
        });
        private static FunctionDefinition CreateExpFunction() => CreateUnaryComplexFunction("exp", Complex.Exp, new DescriptionFunction
        {
            AreaList = ["Алгебра", "Статистика", "Физика"],
            Description = "Вычисляет экспоненту числа (e в степени x).",
            Signature = "Вход: 1 число. Выход: 1 число.",
            Exemple = "exp(1) // Результат: 2.718..."
        });

        private static FunctionDefinition CreateLogFunction()
        {
            const string name = "log";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args => Complex.Log(CastsVar.CastToComplex(args[0], name)) / Complex.Log(CastsVar.CastToComplex(args[1], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Алгебра", "Информатика", "Физика"],
                    Description = "Вычисляет логарифм числа по заданному основанию.",
                    Signature = "Вход: 2 числа (значение, основание). Выход: 1 число.",
                    Exemple = "log(8, 2) // Результат: 3"
                }
            };
        }

        //================== Угловые меры ==================

        private static FunctionDefinition CreateRadFunction()
        {
            const string name = "rad";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)FunctionsForEachElements.GradToRad(CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Геометрия", "Физика", "Инженерия"],
                    Description = "Конвертирует градусы в радианы.",
                    Signature = "Вход: 1 число (градусы). Выход: 1 число (радианы).",
                    Exemple = "rad(180) // Результат: 3.14159..."
                }
            };
        }

        private static FunctionDefinition CreateDegFunction()
        {
            const string name = "deg";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)FunctionsForEachElements.RadToGrad(CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Геометрия", "Физика", "Инженерия"],
                    Description = "Конвертирует радианы в градусы.",
                    Signature = "Вход: 1 число (радианы). Выход: 1 число (градусы).",
                    Exemple = "deg(pi) // Результат: 180"
                }
            };
        }

        //================== Комбинаторика и спец. функции ==================

        private static FunctionDefinition CreateFactFunction()
        {
            const string name = "fact";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)FunctionsForEachElements.Factorial((int)CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Комбинаторика", "Статистика"],
                    Description = "Вычисляет факториал целого неотрицательного числа n (n!).",
                    Signature = "Вход: 1 целое число n. Выход: 1 число.",
                    Exemple = "fact(5) // Результат: 120"
                }
            };
        }

        private static FunctionDefinition CreateGammaFunction()
        {
            const string name = "gamma";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)FunctionsForEachElements.Gamma(CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Математический анализ", "Статистика"],
                    Description = "Вычисляет гамма-функцию, обобщение факториала. Gamma(n) = (n-1)!",
                    Signature = "Вход: 1 вещественное число. Выход: 1 число.",
                    Exemple = "gamma(6) // Результат: 120"
                }
            };
        }

        private static FunctionDefinition CreateCombFunction()
        {
            const string name = "Comb";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args => (Complex)CombinatoricsBaseFunction.Combinations((int)CastsVar.CastToDouble(args[0], name), (int)CastsVar.CastToDouble(args[1], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Комбинаторика", "Теория вероятностей"],
                    Description = "Вычисляет число сочетаний из n по k (C n k).",
                    Signature = "Вход: 2 целых числа (n, k). Выход: 1 число.",
                    Exemple = "Comb(5, 2) // Результат: 10"
                }
            };
        }

        private static FunctionDefinition CreateCombPFunction()
        {
            const string name = "CombP";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    var n = (int)CastsVar.CastToDouble(args[0], name);
                    var k = (int)CastsVar.CastToDouble(args[1], name);
                    return (Complex)(FunctionsForEachElements.Factorial(n) / FunctionsForEachElements.Factorial(n - k));
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Комбинаторика", "Теория вероятностей"],
                    Description = "Вычисляет число размещений из n по k (A n k).",
                    Signature = "Вход: 2 целых числа (n, k). Выход: 1 число.",
                    Exemple = "CombP(5, 2) // Результат: 20"
                }
            };
        }

        //================== Векторные операции ==================

        private static FunctionDefinition CreateMagFunction()
        {
            const string name = "mag";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)BaseDist.L2(CastsVar.CastToComplexVector(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Линейная алгебра", "Геометрия", "Физика"],
                    Description = "Вычисляет длину (евклидову норму или L2-норму) вектора.",
                    Signature = "Вход: 1 вектор. Выход: 1 вещественное число.",
                    Exemple = "mag([3, 4]) // Результат: 5"
                }
            };
        }

        private static FunctionDefinition CreateSumFunction()
        {
            const string name = "sum";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => CastsVar.CastToComplexVector(args[0], name).Sum(),
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Линейная алгебра"],
                    Description = "Вычисляет сумму всех элементов вектора.",
                    Signature = "Вход: 1 вектор. Выход: 1 число.",
                    Exemple = "sum([1, 2, 3, 4]) // Результат: 10"
                }
            };
        }

        private static FunctionDefinition CreateDotFunction()
        {
            const string name = "dot";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args => ComplexVector.Dot(CastsVar.CastToComplexVector(args[0], name), CastsVar.CastToComplexVector(args[1], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Линейная алгебра", "Геометрия", "Физика"],
                    Description = "Вычисляет скалярное произведение двух векторов.",
                    Signature = "Вход: 2 вектора. Выход: 1 число.",
                    Exemple = "dot([1, 2, 3], [4, 5, 6]) // Результат: 32"
                }
            };
        }

        private static FunctionDefinition CreateCrossFunction()
        {
            const string name = "cross";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    var v1 = CastsVar.CastToRealVector(args[0], name);
                    var v2 = CastsVar.CastToRealVector(args[1], name);
                    return Vector.Cross(v1, v2);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Линейная алгебра", "Геометрия", "Физика"],
                    Description = "Вычисляет векторное (косое) произведение двух 3D векторов.",
                    Signature = "Вход: 2 двумерных или 2 трехмерных вектора. Выход: 1 вектор.",
                    Exemple = "cross([1, 0, 0], [0, 1, 0]) // Результат: [0, 0, 1]"
                }
            };
        }

        private static FunctionDefinition CreateIndexFunction()
        {
            const string name = "index";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args => CastsVar.CastToComplexVector(args[0], name)[CastsVar.CastToInt32(args[1], name)],
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Линейная алгебра"],
                    Description = "Возвращает элемент вектора по заданному индексу (нумерация с 0).",
                    Signature = "Вход: 1 вектор, 1 целое число (индекс). Выход: элемент вектора.",
                    Exemple = "index([10, 20, 30], 1) // Результат: 20"
                }
            };
        }

        //================== Статистика ==================

        private static FunctionDefinition CreateMeanFunction()
        {
            const string name = "mean";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = -1,
                Delegate = args =>
                {
                    if (!args.Any()) return Complex.Zero;
                    var complexArgs = args.Select(a => CastsVar.CastToComplex(a, name)).ToArray();
                    return new Complex(complexArgs.Average(c => c.Real), complexArgs.Average(c => c.Imaginary));
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Вычисляет среднее арифметическое для набора чисел.",
                    Signature = "Вход: N чисел. Выход: 1 число.",
                    Exemple = "mean(2, 4, 9) // Результат: 5"
                }
            };
        }

        private static FunctionDefinition CreateMinFunction()
        {
            const string name = "min";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = -1,
                Delegate = args =>
                {
                    if (!args.Any()) throw new ArgumentException("Функция 'min' требует хотя бы один аргумент.");
                    return new Complex(args.Select(a => CastsVar.CastToDouble(a, name)).Min(), 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Находит минимальное значение среди набора чисел.",
                    Signature = "Вход: N чисел. Выход: 1 число.",
                    Exemple = "min(2, -1, 5) // Результат: -1"
                }
            };
        }

        private static FunctionDefinition CreateMaxFunction()
        {
            const string name = "max";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = -1,
                Delegate = args =>
                {
                    if (!args.Any()) throw new ArgumentException("Функция 'max' требует хотя бы один аргумент.");
                    return new Complex(args.Select(a => CastsVar.CastToDouble(a, name)).Max(), 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Находит максимальное значение среди набора чисел.",
                    Signature = "Вход: N чисел. Выход: 1 число.",
                    Exemple = "max(2, -1, 5) // Результат: 5"
                }
            };
        }

        #endregion

        #region Helper Factory Methods

        /// <summary>Вспомогательный метод для создания функций вида F(x), работающих с комплексными числами.</summary>
        private static FunctionDefinition CreateUnaryComplexFunction(string name, Func<Complex, Complex> function, DescriptionFunction description)
        {
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => function(CastsVar.CastToComplex(args[0], name)),
                Description = description
            };
        }

        /// <summary>Вспомогательный метод для создания функций вида F(x, y), работающих с комплексными числами.</summary>
        private static FunctionDefinition CreateBinaryComplexFunction(string name, Func<Complex, Complex, Complex> function, DescriptionFunction description)
        {
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args => function(CastsVar.CastToComplex(args[0], name), CastsVar.CastToComplex(args[1], name)),
                Description = description
            };
        }

        #endregion
    }
}