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
        CreateCeilFunction(),
        CreateAbsFunction(),
        CreateSqrtFunction(),
        CreateCbrtFunction(),
        CreatePowFunction(),
        
        // Работа с датами
        CreateDateTimeFunction(),
        CreateDateDiffFunction(),

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
        
        // Теория чисел
        CreateGCDFunction(),
        CreateLCMFunction(),

        // Векторные операции
        CreateMagFunction(),
        CreateSumFunction(),
        CreateDotFunction(),
        CreateCrossFunction(),
        CreateIndexFunction(),

        // Статистика
        CreateMeanFunction(),
        CreateMinFunction(),
        CreateMaxFunction(),
        
        // Bitwise операции
        CreateXorFunction(),
        CreateBitNotFunction(),
        
        // Строковые операции
        CreateLenFunction(),
        CreateConcatFunction(),
        CreateSubstrFunction(),
        CreateJoinFunction()
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
                    Exemple = "floor(3.59) // Результат: 3"
                }
            };
        }

        private static FunctionDefinition CreateCeilFunction()
        {
            const string name = "ceil";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args => (Complex)Math.Ceiling(CastsVar.CastToDouble(args[0], name)),
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Программирование"],
                    Description = "Округляет вещественное число вверх до ближайшего целого.",
                    Signature = "Вход: 1 число. Выход: 1 округлённое число.",
                    Exemple = "ceil(3.2) // Результат: 4"
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

        //================== Работа с датами ==================

        private static FunctionDefinition CreateDateTimeFunction()
        {
            const string name = "DateTime";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args =>
                {
                    var dateString = args[0]?.ToString() ?? throw new ArgumentException($"Функция '{name}' требует строку с датой.");
                    
                    if (!System.DateTime.TryParse(dateString, System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out var result))
                    {
                        throw new ArgumentException($"Не удалось распарсить дату: '{dateString}'. Используйте формат: yyyy-MM-dd или yyyy-MM-dd HH:mm:ss");
                    }
                    
                    return result;
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Календарь"],
                    Description = "Парсит строку и возвращает объект DateTime. Поддерживает форматы: yyyy-MM-dd, yyyy-MM-dd HH:mm:ss",
                    Signature = "Вход: 1 строка (дата). Выход: DateTime объект.",
                    Exemple = "DateTime(\"2025-12-19\") // Парсит дату"
                }
            };
        }

        private static FunctionDefinition CreateDateDiffFunction()
        {
            const string name = "DateDiff";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    if (args[0] is not System.DateTime date1)
                        throw new ArgumentException($"Функция '{name}': первый аргумент должен быть DateTime");
                    if (args[1] is not System.DateTime date2)
                        throw new ArgumentException($"Функция '{name}': второй аргумент должен быть DateTime");
                    
                    var isNegative = date1 < date2;
                    var span = date1 - date2;
                    
                    // Работаем с абсолютными значениями для упрощения логики
                    var start = isNegative ? date1 : date2;
                    var end = isNegative ? date2 : date1;
                    
                    // Вычисляем компоненты разницы
                    int years = end.Year - start.Year;
                    int months = end.Month - start.Month;
                    int days = end.Day - start.Day;
                    int hours = end.Hour - start.Hour;
                    int minutes = end.Minute - start.Minute;
                    int seconds = end.Second - start.Second;
                    
                    // Корректируем отрицательные значения снизу вверх
                    if (seconds < 0) { seconds += 60; minutes--; }
                    if (minutes < 0) { minutes += 60; hours--; }
                    if (hours < 0) { hours += 24; days--; }
                    if (days < 0)
                    {
                        months--;
                        days += System.DateTime.DaysInMonth(start.Year, start.Month);
                    }
                    if (months < 0) { months += 12; years--; }
                    
                    var sign = isNegative ? "-" : "";
                    return $"{sign}{years}y {months}m {days}d {hours}h {minutes}min {seconds}s (total: {span.TotalDays:F2} days)";
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Календарь"],
                    Description = "Вычисляет точную календарную разницу между двумя датами в годах, месяцах, днях, часах, минутах и секундах",
                    Signature = "Вход: 2 DateTime объекта. Выход: строка с детальной разницей.",
                    Exemple = "DateDiff(DateTime(\"2025-12-19\"), DateTime(\"2024-01-01\")) // Детальная разница"
                }
            };
        }

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
                ArgumentCount = -1, // ИСПРАВЛЕНИЕ: Поддержка 1 или 2 аргументов
                Delegate = args =>
                {
                    if (args.Length == 1)
                    {
                        // log(x) = ln(x) - натуральный логарифм
                        return Complex.Log(CastsVar.CastToComplex(args[0], name));
                    }
                    else if (args.Length == 2)
                    {
                        // log(x, base) - логарифм по основанию
                        return Complex.Log(CastsVar.CastToComplex(args[0], name)) / Complex.Log(CastsVar.CastToComplex(args[1], name));
                    }
                    else
                    {
                        throw new ArgumentException($"Функция '{name}' ожидает 1 или 2 аргумента, получила {args.Length}.");
                    }
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Алгебра", "Информатика", "Физика"],
                    Description = "Вычисляет логарифм числа. log(x) = ln(x), log(x, base) = логарифм по основанию.",
                    Signature = "Вход: 1 число (ln) или 2 числа (значение, основание). Выход: 1 число.",
                    Exemple = "log(e) // Результат: 1; log(8, 2) // Результат: 3"
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

        //================== Теория чисел ==================

        private static FunctionDefinition CreateGCDFunction()
        {
            const string name = "gcd";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    var a = CastsVar.CastToDouble(args[0], name);
                    var b = CastsVar.CastToDouble(args[1], name);
                    
                    // Проверяем, являются ли числа целыми
                    bool isAInteger = Math.Abs(a - Math.Round(a)) < 1e-10;
                    bool isBInteger = Math.Abs(b - Math.Round(b)) < 1e-10;
                    
                    if (isAInteger && isBInteger)
                    {
                        // Для целых чисел используем быстрый алгоритм
                        return (Complex)ProcessorLogic.Processor.GCDLong((long)a, (long)b);
                    }
                    else
                    {
                        // Для дробных чисел используем алгоритм с преобразованием в дроби
                        return (Complex)ProcessorLogic.Processor.GCDDouble(a, b);
                    }
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Теория чисел", "Алгебра"],
                    Description = "Вычисляет наибольший общий делитель (НОД) двух чисел. Работает как с целыми, так и с дробными числами.",
                    Signature = "Вход: 2 числа. Выход: 1 число (НОД).",
                    Exemple = "gcd(48, 18) // Результат: 6\ngcd(2.5, 1.5) // Результат: 0.5"
                }
            };
        }

        private static FunctionDefinition CreateLCMFunction()
        {
            const string name = "lcm";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    var a = CastsVar.CastToDouble(args[0], name);
                    var b = CastsVar.CastToDouble(args[1], name);
                    
                    // Проверяем, являются ли числа целыми
                    bool isAInteger = Math.Abs(a - Math.Round(a)) < 1e-10;
                    bool isBInteger = Math.Abs(b - Math.Round(b)) < 1e-10;
                    
                    if (isAInteger && isBInteger)
                    {
                        // Для целых чисел используем быстрый алгоритм
                        return (Complex)ProcessorLogic.Processor.LCM((long)a, (long)b);
                    }
                    else
                    {
                        // Для дробных чисел используем алгоритм с преобразованием в дроби
                        return (Complex)ProcessorLogic.Processor.LCMDouble(a, b);
                    }
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Теория чисел", "Алгебра"],
                    Description = "Вычисляет наименьшее общее кратное (НОК) двух чисел. Работает как с целыми, так и с дробными числами.",
                    Signature = "Вход: 2 числа. Выход: 1 число (НОК).",
                    Exemple = "lcm(12, 18) // Результат: 36\nlcm(2.5, 1.5) // Результат: 7.5"
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
                Delegate = args =>
                {
                    var array = args[0];
                    var indexArg = CastsVar.CastToInt32(args[1], name);
                    
                    // Поддержка массивов строк
                    if (array is string[] stringArray)
                    {
                        if (indexArg < 0 || indexArg >= stringArray.Length)
                            throw new IndexOutOfRangeException($"Индекс {indexArg} выходит за границы массива (длина: {stringArray.Length}).");
                        return stringArray[indexArg];
                    }
                    
                    // Поддержка числовых массивов (ComplexVector)
                    var vector = CastsVar.CastToComplexVector(array, name);
                    if (indexArg < 0 || indexArg >= vector.Count)
                        throw new IndexOutOfRangeException($"Индекс {indexArg} выходит за границы массива (длина: {vector.Count}).");
                    return vector[indexArg];
                },
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
                    
                    // ИСПРАВЛЕНИЕ: Поддержка массивов - если передан ComplexVector, используем его элементы
                    Complex[] complexArgs;
                    if (args.Length == 1 && args[0] is ComplexVector vector)
                    {
                        complexArgs = vector.ToArray();
                    }
                    else
                    {
                        complexArgs = args.Select(a => CastsVar.CastToComplex(a, name)).ToArray();
                    }
                    
                    return new Complex(complexArgs.Average(c => c.Real), complexArgs.Average(c => c.Imaginary));
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Вычисляет среднее арифметическое для набора чисел или массива.",
                    Signature = "Вход: N чисел или 1 массив. Выход: 1 число.",
                    Exemple = "mean(2, 4, 9) // Результат: 5; mean([1, 2, 3, 4, 5]) // Результат: 3"
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
                    
                    // Поддержка массивов - если передан ComplexVector, используем его элементы
                    double[] values;
                    if (args.Length == 1 && args[0] is ComplexVector vector)
                    {
                        values = vector.ToArray().Select(c => c.Real).ToArray();
                    }
                    else
                    {
                        values = args.Select(a => CastsVar.CastToDouble(a, name)).ToArray();
                    }
                    
                    return new Complex(values.Min(), 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Находит минимальное значение среди набора чисел или массива.",
                    Signature = "Вход: N чисел или 1 массив. Выход: 1 число.",
                    Exemple = "min(2, -1, 5) // Результат: -1; min([5, 2, 8, 1]) // Результат: 1"
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
                    
                    // Поддержка массивов - если передан ComplexVector, используем его элементы
                    double[] values;
                    if (args.Length == 1 && args[0] is ComplexVector vector)
                    {
                        values = vector.ToArray().Select(c => c.Real).ToArray();
                    }
                    else
                    {
                        values = args.Select(a => CastsVar.CastToDouble(a, name)).ToArray();
                    }
                    
                    return new Complex(values.Max(), 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Статистика", "Анализ данных"],
                    Description = "Находит максимальное значение среди набора чисел или массива.",
                    Signature = "Вход: N чисел или 1 массив. Выход: 1 число.",
                    Exemple = "max(2, -1, 5) // Результат: 5; max([5, 2, 8, 1]) // Результат: 8"
                }
            };
        }

        //================== Bitwise операции ==================

        private static FunctionDefinition CreateXorFunction()
        {
            const string name = "xor";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    int a = (int)CastsVar.CastToDouble(args[0], name);
                    int b = (int)CastsVar.CastToDouble(args[1], name);
                    return new Complex(a ^ b, 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Битовые операции"],
                    Description = "Выполняет битовую операцию XOR (исключающее ИЛИ) над двумя целыми числами.",
                    Signature = "Вход: 2 целых числа. Выход: 1 число.",
                    Exemple = "xor(5, 3) // Результат: 6 (101 XOR 011 = 110)"
                }
            };
        }

        private static FunctionDefinition CreateBitNotFunction()
        {
            const string name = "bitnot";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args =>
                {
                    int a = (int)CastsVar.CastToDouble(args[0], name);
                    return new Complex(~a, 0);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Битовые операции"],
                    Description = "Выполняет битовую операцию NOT (инверсия всех битов) над целым числом.",
                    Signature = "Вход: 1 целое число. Выход: 1 число.",
                    Exemple = "bitnot(5) // Результат: -6 (инверсия битов 101)"
                }
            };
        }

        //================== Строковые операции ==================

        private static FunctionDefinition CreateLenFunction()
        {
            const string name = "len";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 1,
                Delegate = args =>
                {
                    if (args[0] is string str)
                        return new Complex(str.Length, 0);
                    if (args[0] is string[] strArray)
                        return new Complex(strArray.Length, 0);
                    if (args[0] is ComplexVector vec)
                        return new Complex(vec.Count, 0);
                    throw new ArgumentException($"Функция '{name}' ожидает строку, массив или вектор.");
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Строковые операции"],
                    Description = "Возвращает длину строки или вектора.",
                    Signature = "Вход: 1 строка или вектор. Выход: 1 число.",
                    Exemple = "len(\"Hello\") // Результат: 5"
                }
            };
        }

        private static FunctionDefinition CreateConcatFunction()
        {
            const string name = "concat";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = -1, // Переменное количество аргументов
                Delegate = args =>
                {
                    if (!args.Any()) throw new ArgumentException("Функция 'concat' требует хотя бы один аргумент.");
                    return args.Select(arg => arg?.ToString() ?? "").Aggregate((a, b) => a + b);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Строковые операции"],
                    Description = "Объединяет несколько строк в одну.",
                    Signature = "Вход: N строк. Выход: 1 строка.",
                    Exemple = "concat(\"Hello\", \" \", \"World\") // Результат: \"Hello World\""
                }
            };
        }

        private static FunctionDefinition CreateSubstrFunction()
        {
            const string name = "substr";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 3,
                Delegate = args =>
                {
                    if (!(args[0] is string str))
                        throw new ArgumentException($"Функция '{name}' ожидает строку в качестве первого аргумента.");
                    
                    int start = CastsVar.CastToInt32(args[1], name);
                    int length = CastsVar.CastToInt32(args[2], name);
                    
                    if (start < 0 || start >= str.Length)
                        throw new ArgumentException($"Индекс начала ({start}) выходит за пределы строки.");
                    if (length < 0)
                        throw new ArgumentException($"Длина подстроки ({length}) не может быть отрицательной.");
                    
                    // ИСПРАВЛЕНИЕ: Обрезаем длину если она выходит за пределы
                    if (start + length > str.Length)
                        length = str.Length - start;
                    
                    return str.Substring(start, length);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Строковые операции"],
                    Description = "Извлекает подстроку из строки, начиная с указанного индекса и заданной длины.",
                    Signature = "Вход: 1 строка, 2 целых числа (индекс, длина). Выход: 1 строка.",
                    Exemple = "substr(\"Hello World\", 0, 5) // Результат: \"Hello\""
                }
            };
        }

        private static FunctionDefinition CreateJoinFunction()
        {
            const string name = "join";
            return new FunctionDefinition
            {
                Name = name,
                ArgumentCount = 2,
                Delegate = args =>
                {
                    // Первый аргумент - массив строк
                    if (!(args[0] is string[] strArray))
                        throw new ArgumentException($"Функция '{name}' ожидает массив строк в качестве первого аргумента.");
                    
                    // Второй аргумент - разделитель
                    var separator = args[1]?.ToString() ?? "";
                    
                    return string.Join(separator, strArray);
                },
                Description = new DescriptionFunction
                {
                    AreaList = ["Программирование", "Строковые операции"],
                    Description = "Объединяет элементы массива строк в одну строку с указанным разделителем.",
                    Signature = "Вход: 1 массив строк, 1 строка (разделитель). Выход: 1 строка.",
                    Exemple = "join([\"Hello\", \"World\", \"!\"], \" \") // Результат: \"Hello World !\""
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