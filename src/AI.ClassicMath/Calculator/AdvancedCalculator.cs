using AI.ClassicMath.Calculator.Libs;
using AI.ClassicMath.Calculator.Libs.Algebra;
using AI.DataStructs.Algebraic;
using AI.DataStructs.WithComplexElements;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading;

namespace AI.ClassicMath.Calculator
{
    /// <summary>
    /// Калькулятор с поддержкой библиотек, векторов и комплексных чисел.
    /// </summary>
    [Serializable]
    public class AdvancedCalculator
    {
        #region Поля и конструктор

        /// <summary>
        /// Зарегистрированные операторы
        /// </summary>
        public Dictionary<string, (int Precedence, string Associativity)> Operators { get; set; }

        /// <summary>
        /// Операции с этими операторами
        /// </summary>
        public Dictionary<(Type, Type, string), Func<object, object, object>> OperationsFunctions { get; set; }

        /// <summary>
        /// Функции (математические)
        /// </summary>
        public Dictionary<string, FunctionDefinition> Functions { get; set; }


        /// <summary>
        /// Калькулятор с поддержкой библиотек, векторов и комплексных чисел.
        /// </summary>
        public AdvancedCalculator()
        {
            var baseMathLib = new BaseMathLib();
            var eq = new EquationLib();

            var baseOperators = new LibOperatorsBase();

            Operators = baseOperators.GetOperators();
            OperationsFunctions = baseOperators.GetOperationsFunctions();

            Functions = baseMathLib.GetFunctions();

            foreach (var func in eq.GetFunctions())
            {
                Functions.Add(func.Key, func.Value);
            }
        }

        #endregion


    public object Evaluate(string expression, ExecutionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            expression = expression.Trim();
            if (string.IsNullOrEmpty(expression)) return null;

            // Обработка ++ и --
            var incrementMatch = Regex.Match(expression, @"^(\w+)\+\+$");
            var decrementMatch = Regex.Match(expression, @"^(\w+)--$");
            var preIncrementMatch = Regex.Match(expression, @"^\+\+(\w+)$");
            var preDecrementMatch = Regex.Match(expression, @"^--(\w+)$");
            
            if (incrementMatch.Success)
            {
                var varName = incrementMatch.Groups[1].Value;
                expression = $"{varName} = {varName} + 1";
            }
            else if (decrementMatch.Success)
            {
                var varName = decrementMatch.Groups[1].Value;
                expression = $"{varName} = {varName} - 1";
            }
            else if (preIncrementMatch.Success)
            {
                var varName = preIncrementMatch.Groups[1].Value;
                expression = $"{varName} = {varName} + 1";
            }
            else if (preDecrementMatch.Success)
            {
                var varName = preDecrementMatch.Groups[1].Value;
                expression = $"{varName} = {varName} - 1";
            }
            
            // Обработка составных операторов (+=, -=, *=, /=, %=, ^=)
            var compoundMatch = Regex.Match(expression, @"^(\w+)\s*([+\-*/%^])=\s*(.+)$");
            if (compoundMatch.Success)
            {
                var varName = compoundMatch.Groups[1].Value;
                var op = compoundMatch.Groups[2].Value;
                var rightExpr = compoundMatch.Groups[3].Value;
                expression = $"{varName} = {varName} {op} ({rightExpr})";
            }

            if (expression.Count(c => c == '=') == 1 && IsSimpleAssignmentTarget(expression))
            {
                var parts = expression.Split('=');
                var varName = parts[0].Trim();
                if (!IsValidVarName(varName)) throw new ArgumentException($"Недопустимое имя переменной: '{varName}'.");

                var exprToEvaluate = parts[1];
                var result = EvaluateExpression(exprToEvaluate, context, cancellationToken);
                context.Memory[varName] = result;
                return result;
            }
            return EvaluateExpression(expression, context, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

        private object EvaluateExpression(string expression, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // УЛУЧШЕНИЕ 2: Поддержка тернарного оператора (? :)
            // Проверяем есть ли тернарный оператор НА ВЕРХНЕМ УРОВНЕ (не в скобках)
            if (HasTopLevelTernary(expression))
            {
                return EvaluateTernary(expression, context, cancellationToken);
            }
            
            // НОВОЕ: Пре-обработка скобочных выражений с тернарным
            // Если в expression есть (expr ? a : b), заменяем его на результат
            expression = PreprocessTernaryInGroups(expression, context, cancellationToken);
            
            // ИСПРАВЛЕНИЕ: Обработка тернарных в аргументах функций
            expression = PreprocessTernaryInFunctions(expression, context, cancellationToken);
            
            var tokens = Tokenize(expression, cancellationToken);
            var rpn = ConvertToRpn(tokens, context, cancellationToken);
            return EvaluateRpn(rpn, context, cancellationToken);
        }
        
    /// <summary>
    /// Пре-обработка скобочных выражений с тернарным оператором
    /// (1 > 0 ? 5 : 10) * 2 → 5 * 2
    /// </summary>
    private string PreprocessTernaryInGroups(string expression, ExecutionContext context, CancellationToken cancellationToken)
    {
        while (true)
        {
            int parenDepth = 0;
            int startPos = -1;
            int endPos = -1;
            bool hasTernary = false;
            
            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];
                
                if (c == '(')
                {
                    if (parenDepth == 0) startPos = i;
                    parenDepth++;
                }
                else if (c == ')')
                {
                    parenDepth--;
                    if (parenDepth == 0)
                    {
                        endPos = i;
                        // Проверяем есть ли ? внутри этих скобок
                        string innerExpr = expression.Substring(startPos + 1, endPos - startPos - 1);
                        if (innerExpr.Contains("?"))
                        {
                            // ИСПРАВЛЕНИЕ: Проверяем, это функция или просто скобки?
                            // Если перед ( идёт имя (буквы/цифры), то это функция — НЕ обрабатываем здесь!
                            bool isFunctionCall = false;
                            if (startPos > 0)
                            {
                                int j = startPos - 1;
                                // Пропускаем пробелы
                                while (j >= 0 && char.IsWhiteSpace(expression[j])) j--;
                                // Если перед ( есть буквы/цифры — это функция
                                if (j >= 0 && (char.IsLetterOrDigit(expression[j]) || expression[j] == '_'))
                                    isFunctionCall = true;
                            }
                            
                            if (!isFunctionCall)
                            {
                                hasTernary = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            if (!hasTernary || startPos == -1 || endPos == -1)
                break; // Нет тернарных в скобках (или все скобки — это функции)
            
            // Вычисляем выражение внутри скобок
            string innerExpr2 = expression.Substring(startPos + 1, endPos - startPos - 1);
            var result = EvaluateExpression(innerExpr2, context, cancellationToken);
            
            // Форматируем результат как строку для подстановки
            string resultStr;
            if (result is Complex complexVal)
            {
                if (Math.Abs(complexVal.Imaginary) < 1e-10) // Реальное число
                    resultStr = complexVal.Real.ToString(CultureInfo.InvariantCulture);
                else // Комплексное число
                    resultStr = $"({complexVal.Real.ToString(CultureInfo.InvariantCulture)}+{complexVal.Imaginary.ToString(CultureInfo.InvariantCulture)}*i)";
            }
            else if (result is string strVal)
            {
                resultStr = $"\"{strVal}\""; // Строки в кавычках
            }
            else
            {
                resultStr = result.ToString();
            }
            
            // Заменяем (expr) на result
            expression = expression.Substring(0, startPos) + resultStr + expression.Substring(endPos + 1);
        }
        
        return expression;
    }
    
    /// <summary>
    /// Обрабатывает тернарные операторы внутри аргументов функций
    /// abs(1 > 0 ? -5 : 5) → abs(-5)
    /// </summary>
    private string PreprocessTernaryInFunctions(string expression, ExecutionContext context, CancellationToken cancellationToken)
    {
        // Ищем функции вида funcName(...)
        var regex = new Regex(@"(\w+)\s*\(");
        var matches = regex.Matches(expression);
        
        if (matches.Count == 0)
            return expression; // Нет функций
        
        // Обрабатываем каждую функцию
        foreach (Match match in matches)
        {
            var funcName = match.Groups[1].Value;
            var openParenIndex = match.Index + match.Length - 1; // Индекс '('
            
            // Проверяем, это функция или просто переменная со скобкой
            if (!Functions.ContainsKey(funcName))
                continue;
            
            // Находим закрывающую скобку
            int parenDepth = 0;
            int closeParenIndex = -1;
            for (int i = openParenIndex; i < expression.Length; i++)
            {
                if (expression[i] == '(') parenDepth++;
                else if (expression[i] == ')')
                {
                    parenDepth--;
                    if (parenDepth == 0)
                    {
                        closeParenIndex = i;
                        break;
                    }
                }
            }
            
            if (closeParenIndex == -1)
                continue; // Не нашли закрывающую скобку
            
            // Извлекаем аргументы
            string argsString = expression.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1);
            
            // Проверяем есть ли тернарный в аргументах
            if (!argsString.Contains("?"))
                continue;
            
            // Разбиваем на аргументы (учитывая вложенные скобки)
            var args = SplitFunctionArguments(argsString);
            
            // Обрабатываем каждый аргумент
            bool changed = false;
            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].Contains("?"))
                {
                    try
                    {
                        // Вычисляем тернарный в аргументе
                        var result = EvaluateTernary(args[i], context, cancellationToken);
                        args[i] = FormatComplexResult(result);
                        changed = true;
                    }
                    catch
                    {
                        // Если не удалось обработать - оставляем как есть
                    }
                }
            }
            
            if (changed)
            {
                // Собираем функцию обратно
                string newFuncCall = funcName + "(" + string.Join(", ", args) + ")";
                expression = expression.Substring(0, match.Index) + newFuncCall + expression.Substring(closeParenIndex + 1);
                
                // Рекурсивно обрабатываем дальше (могут быть ещё функции)
                return PreprocessTernaryInFunctions(expression, context, cancellationToken);
            }
        }
        
        return expression;
    }
    
    /// <summary>
    /// Разбивает строку аргументов функции на отдельные аргументы (учитывая вложенные скобки)
    /// </summary>
    private List<string> SplitFunctionArguments(string argsString)
    {
        var args = new List<string>();
        int parenDepth = 0;
        int start = 0;
        
        for (int i = 0; i < argsString.Length; i++)
        {
            if (argsString[i] == '(') parenDepth++;
            else if (argsString[i] == ')') parenDepth--;
            else if (argsString[i] == ',' && parenDepth == 0)
            {
                args.Add(argsString.Substring(start, i - start).Trim());
                start = i + 1;
            }
        }
        
        // Добавляем последний аргумент
        if (start < argsString.Length)
            args.Add(argsString.Substring(start).Trim());
        
        return args;
    }
    
    /// <summary>
    /// Форматирует результат вычисления для подстановки в выражение
    /// </summary>
    private string FormatComplexResult(object result)
    {
        if (result is Complex complexVal)
        {
            if (Math.Abs(complexVal.Imaginary) < 1e-10) // Реальное число
                return complexVal.Real.ToString(CultureInfo.InvariantCulture);
            else // Комплексное число
                return $"({complexVal.Real.ToString(CultureInfo.InvariantCulture)}+{complexVal.Imaginary.ToString(CultureInfo.InvariantCulture)}*i)";
        }
        else if (result is string strVal)
        {
            return $"\"{strVal}\""; // Строки в кавычках
        }
        else
        {
            return result.ToString();
        }
    }
        
    /// <summary>
    /// Проверяет есть ли тернарный оператор на верхнем уровне (вне скобок)
    /// ИЗМЕНЕНИЕ: Теперь ищет '?' НА ЛЮБОЙ ГЛУБИНЕ, потому что
    /// выражение (1 > 0 ? 5 : 10) * 2 тоже нужно обработать!
    /// </summary>
    private bool HasTopLevelTernary(string expression)
    {
        int parenDepth = 0;
        int bracketDepth = 0;
        bool inString = false;
        int topLevelQuestionPos = -1;
        
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            
            if (c == '"' && (i == 0 || expression[i - 1] != '\\'))
                inString = !inString;
            
            if (inString) continue;
            
            if (c == '(') parenDepth++;
            else if (c == ')') parenDepth--;
            else if (c == '[') bracketDepth++;
            else if (c == ']') bracketDepth--;
            else if (c == '?')
            {
                // Ищем ПЕРВЫЙ '?' на НУЛЕВОМ уровне скобок
                if (parenDepth == 0 && bracketDepth == 0 && topLevelQuestionPos == -1)
                {
                    topLevelQuestionPos = i;
                    return true; // Есть тернарный оператор на верхнем уровне
                }
            }
        }
        
        return false;
    }
        
    /// <summary>
    /// Обрабатывает тернарный оператор: condition ? trueValue : falseValue
    /// ПОДДЕРЖИВАЕТ вложенные тернарные и тернарные в скобках!
    /// </summary>
    private object EvaluateTernary(string expression, ExecutionContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        // Найти самый внешний ? (не внутри скобок)
        int questionPos = -1;
        int colonPos = -1;
        int parenDepth = 0;
        int bracketDepth = 0;
        bool inString = false;
        int nestedTernaryCount = 0; // Счётчик вложенных ? после первого
        
        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];
            
            if (c == '"' && (i == 0 || expression[i - 1] != '\\'))
                inString = !inString;
            
            if (inString) continue;
            
            if (c == '(') parenDepth++;
            else if (c == ')') parenDepth--;
            else if (c == '[') bracketDepth++;
            else if (c == ']') bracketDepth--;
            else if (parenDepth == 0 && bracketDepth == 0)
            {
                if (c == '?')
                {
                    if (questionPos == -1)
                    {
                        questionPos = i; // Первый ? на верхнем уровне
                    }
                    else
                    {
                        nestedTernaryCount++; // Это вложенный тернарный
                    }
                }
                else if (c == ':' && questionPos != -1)
                {
                    if (nestedTernaryCount > 0)
                    {
                        nestedTernaryCount--; // Это : для вложенного тернарного
                    }
                    else
                    {
                        colonPos = i; // Нашли соответствующую :
                        break;
                    }
                }
            }
        }
        
        if (questionPos == -1 || colonPos == -1)
            throw new ArgumentException("Некорректный тернарный оператор: отсутствует '?' или ':'");
        
        var conditionExpr = expression.Substring(0, questionPos).Trim();
        var trueExpr = expression.Substring(questionPos + 1, colonPos - questionPos - 1).Trim();
        var falseExpr = expression.Substring(colonPos + 1).Trim();
        
        var conditionResult = EvaluateExpression(conditionExpr, context, cancellationToken);
        var conditionValue = CastsVar.CastToDouble(conditionResult, "ternary");
        
        if (Math.Abs(conditionValue) > 1e-10) // true
            return EvaluateExpression(trueExpr, context, cancellationToken);
        else // false
            return EvaluateExpression(falseExpr, context, cancellationToken);
    }


        #region Токенизация -> ОПН -> Вычисление

        private List<string> Tokenize(string expression, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Заменяем логические операторы на символы перед токенизацией
            expression = Regex.Replace(expression, @"\band\b", "&&", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bor\b", "||", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"\bnot\b", "!", RegexOptions.IgnoreCase);
            
            // Сначала извлекаем строковые литералы (в двойных кавычках)
            // Поддержка научной нотации: 1e-10, 2.5e+3, 3E10
            var pattern = @"(""[^""]*"")|([0-9]+\.?[0-9]*(?:[eE][+-]?[0-9]+)?|[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)|([a-zA-Z_][a-zA-Z0-9_]*)|(<<|>>|>=|<=|==|!=|&&|\|\|)|(.)";
            var tokens = Regex.Matches(expression, pattern).Cast<Match>()
                .Where(m => !string.IsNullOrWhiteSpace(m.Value))
                .Select(m => m.Value).ToList();

            for (int i = 0; i < tokens.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (tokens[i] == "-")
                {
                    bool isUnary = (i == 0) || Operators.ContainsKey(tokens[i - 1]) || "([,".Contains(tokens[i - 1]);
                    if (isUnary)
                    {
                        // УЛУЧШЕНИЕ 6: Объединяем унарный минус с числом
                        // Вместо: ["-", "5"] → ["~", "5"]
                        // Делаем: ["-", "5"] → ["-5"]
                        if (i + 1 < tokens.Count && double.TryParse(tokens[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                        {
                            tokens[i] = "-" + tokens[i + 1]; // Объединяем
                            tokens.RemoveAt(i + 1); // Удаляем следующий токен
                        }
                        else
                        {
                            tokens[i] = "~"; // Оставляем ~ для других случаев (например ~x)
                        }
                    }
                }
                else if (tokens[i] == "!")
                {
                    // Проверяем, это логическое НЕ или != (не равно)
                    if (i + 1 < tokens.Count && tokens[i + 1] == "=")
                    {
                        // Это !=, объединяем
                        tokens[i] = "!=";
                        tokens.RemoveAt(i + 1);
                    }
                    else
                    {
                        // Это логическое НЕ (унарный оператор)
                        // Оставляем как есть
                    }
                }
            }
            return tokens;
        }

        private Queue<string> ConvertToRpn(List<string> tokens, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            var outputQueue = new Queue<string>();
            var operatorStack = new Stack<string>();
            var argCountStack = new Stack<int>();
            
            // Для тернарного оператора нужно отслеживать ?
            var ternaryStack = new Stack<int>(); // Позиции в outputQueue где нужно вставить результат
            
            for (int i = 0; i < tokens.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var token = tokens[i];
                if (IsValue(token, context))
                {
                    outputQueue.Enqueue(token);
                    
                    // УЛУЧШЕНИЕ 5 (исправлено): Отслеживаем первый элемент в массиве
                    // Увеличиваем счётчик только если это первый элемент (count == 0)
                    if (operatorStack.Count > 0 && operatorStack.Peek() == "[" && argCountStack.Count > 0)
                    {
                        var currentCount = argCountStack.Peek(); // Peek вместо Pop!
                        if (currentCount == 0) // Первый элемент - устанавливаем count = 1
                        {
                            argCountStack.Pop();
                            argCountStack.Push(1);
                        }
                    }
                }
                else if (Functions.ContainsKey(token))
                {
                    operatorStack.Push(token);
                    argCountStack.Push(1);
                    
                    // ИСПРАВЛЕНИЕ: Если функция - это первый элемент массива, увеличиваем счетчик массива
                    // Проверяем стек на уровень выше (под функцией может быть массив "[")
                    if (operatorStack.Count > 1)
                    {
                        // Копируем стек чтобы заглянуть ниже
                        var stackArray = operatorStack.ToArray();
                        // stackArray[0] - это только что добавленная функция
                        // stackArray[1] - это то что было до функции
                        if (stackArray.Length > 1 && stackArray[1] == "[" && argCountStack.Count > 1)
                        {
                            // Под функцией - массив, проверяем его счетчик
                            var argCountArray = argCountStack.ToArray();
                            // argCountArray[0] - счетчик аргументов функции (=1)
                            // argCountArray[1] - счетчик элементов массива
                            if (argCountArray.Length > 1 && argCountArray[1] == 0)
                            {
                                // Первый элемент массива - функция, увеличиваем счетчик
                                var funcArgCount = argCountStack.Pop();
                                var arrayCount = argCountStack.Pop();
                                argCountStack.Push(1); // Массив: первый элемент
                                argCountStack.Push(funcArgCount); // Функция: 1 аргумент
                            }
                        }
                    }
                }
                else if (token == ",")
                {
                    while (operatorStack.Count > 0 && !"([".Contains(operatorStack.Peek()))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && "([".Contains(operatorStack.Peek()))
                    {
                        // УЛУЧШЕНИЕ 4: Правильный подсчет элементов в массиве/функции
                        if (argCountStack.Count > 0)
                        {
                            var currentCount = argCountStack.Pop();
                            argCountStack.Push(currentCount + 1);
                        }
                    }
                    else throw new ArgumentException("Лишняя запятая или запятая вне вызова функции/вектора.");
                }
                else if (token == "~")
                {
                    operatorStack.Push(token);
                }
                else if (token == "!")
                {
                    // Логическое НЕ (унарное)
                    operatorStack.Push(token);
                }
                else if (Operators.ContainsKey(token))
                {
                    while (operatorStack.Count > 0 && Operators.TryGetValue(operatorStack.Peek(), out
                        var op2) && (op2.Precedence > Operators[token].Precedence || (op2.Precedence == Operators[token].Precedence && op2.Associativity == "Left")))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var op = operatorStack.Pop();
                        if (op != "?:" && op != "?") // Не выталкиваем тернарный оператор
                            outputQueue.Enqueue(op);
                    }

                    operatorStack.Push(token);
                }
                else if ("([".Contains(token))
                {
                    if (i > 0 && token == "(" && context.Memory.ContainsKey(tokens[i - 1]) && !Functions.ContainsKey(tokens[i - 1])) throw new ArgumentException($"Переменная '{tokens[i - 1]}' не является функцией.");
                    operatorStack.Push(token);
                    if (token == "[")
                        argCountStack.Push(0); // УЛУЧШЕНИЕ 3: Начинаем с 0 для поддержки пустых массивов
                }
                else if (")]".Contains(token))
                {
                    string openBracket = token == ")" ? "(" : "[";
                    
                    // Проверка на пустой массив []
                    bool isEmptyArray = false;
                    if (token == "]" && i > 0 && tokens[i - 1] == "[")
                    {
                        isEmptyArray = true;
                    }
                    
                    while (operatorStack.Count > 0 && operatorStack.Peek() != openBracket)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        outputQueue.Enqueue(operatorStack.Pop());
                    }

                    if (operatorStack.Count == 0)
                        throw new ArgumentException($"Отсутствует парная открывающая скобка '{openBracket}'.");
                    operatorStack.Pop();

                    if (token == ")" && operatorStack.Count > 0 && Functions.ContainsKey(operatorStack.Peek()))
                    {
                        outputQueue.Enqueue($"{operatorStack.Pop()}_{(argCountStack.Any() ? argCountStack.Pop() : 1)}");
                    }
                    else if (token == "]")
                    {
                        int count = 0;
                        if (isEmptyArray)
                        {
                            count = 0; // Пустой массив
                            if (argCountStack.Any()) argCountStack.Pop(); // Убираем счетчик
                        }
                        else if (argCountStack.Any())
                        {
                            count = argCountStack.Pop();
                            // Если был хотя бы один элемент, count должен быть минимум 1
                            if (count == 0) count = 1;
                        }
                        else
                        {
                            count = 1; // По умолчанию 1 элемент если нет стека
                        }
                        
                        // ОТЛАДКА: считаем сколько элементов реально в outputQueue после последнего [
                        // Проблема может быть в том, что count не соответствует реальному количеству элементов
                        
                        outputQueue.Enqueue($"vector_{count}");
                    }
                }
                else
                {
                    throw new ArgumentException($"Неизвестный токен или синтаксическая ошибка: '{token}'");
                }
            }
            while (operatorStack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var op = operatorStack.Pop();
                if ("([".Contains(op)) throw new ArgumentException($"Отсутствует парная закрывающая скобка.");
                outputQueue.Enqueue(op);
            }
            return outputQueue;
        }

        private object EvaluateRpn(Queue<string> rpnTokens, ExecutionContext context, CancellationToken cancellationToken = default)
        {
            var evalStack = new Stack<object>();
            
            // ОТЛАДКА: выводим RPN для диагностики
            var rpnList = rpnTokens.ToList();
            // Console.WriteLine($"RPN: {string.Join(" ", rpnList)}");
            rpnTokens = new Queue<string>(rpnList);
            
            foreach (var token in rpnTokens)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out
                    var num))
                {
                    evalStack.Push(new Complex(num, 0));
                }
                else if (token == "i")
                {
                    // Проверяем контекст: если 'i' определена как переменная, используем её значение
                    // Иначе - это мнимая единица
                    if (context.Memory.ContainsKey("i"))
                    {
                        evalStack.Push(context.Memory["i"]);
                    }
                    else
                    {
                        evalStack.Push(Complex.ImaginaryOne);
                    }
                }
                else if (token.StartsWith("\"") && token.EndsWith("\""))
                {
                    // Строковый литерал - убираем кавычки и пушим как строку
                    evalStack.Push(token.Substring(1, token.Length - 2));
                }
                else if (context.Memory.TryGetValue(token, out
                    var value))
                {
                    evalStack.Push(value);
                }
                else if (token == "~")
                {
                    if (evalStack.Count < 1) throw new InvalidOperationException("Недостаточно операндов для унарного минуса.");
                    object operand = evalStack.Pop();
                    if (operand is Complex c) evalStack.Push(Complex.Negate(c));
                    else if (operand is ComplexVector cv) evalStack.Push(cv * -1);
                    else if (operand is Vector rv) evalStack.Push(rv * -1);
                    else throw new InvalidOperationException($"Унарный минус не применим к типу {operand.GetType().Name}.");
                }
                else if (token == "!")
                {
                    // Логическое НЕ
                    if (evalStack.Count < 1) throw new InvalidOperationException("Недостаточно операндов для логического НЕ.");
                    object operand = evalStack.Pop();
                    if (operand is Complex c)
                    {
                        evalStack.Push(new Complex(c.Real == 0 ? 1.0 : 0.0, 0));
                    }
                    else if (operand is double d)
                    {
                        evalStack.Push(new Complex(d == 0 ? 1.0 : 0.0, 0));
                    }
                    else throw new InvalidOperationException($"Логическое НЕ не применимо к типу {operand.GetType().Name}.");
                }
                else if (Operators.ContainsKey(token))
                {
                    if (evalStack.Count < 2) throw new InvalidOperationException($"Недостаточно операндов для оператора '{token}'.");
                    ApplyOperator(token, evalStack);
                }
                else if (token == "?:")
                {
                    // Тернарный оператор: condition ? true_val : false_val
                    if (evalStack.Count < 3) throw new InvalidOperationException("Недостаточно операндов для тернарного оператора.");
                    
                    var falseVal = evalStack.Pop();
                    var trueVal = evalStack.Pop();
                    var condition = evalStack.Pop();
                    
                    // Проверяем условие
                    bool isTrue = false;
                    if (condition is Complex c)
                        isTrue = c.Real != 0;
                    else if (condition is double d)
                        isTrue = d != 0;
                    
                    evalStack.Push(isTrue ? trueVal : falseVal);
                }
                else if (token.StartsWith("vector_") || token.Contains('_'))
                {
                    var parts = token.Split(new[] { '_' }, 2);
                    var funcName = parts[0];
                    var argCountStr = parts[1];
                    if (!int.TryParse(argCountStr, out
                        var argCount)) throw new InvalidOperationException("Поврежденный токен функции/вектора.");
                    if (argCount == 0 && funcName == "vector")
                    {
                        evalStack.Push(new ComplexVector(0));
                        continue;
                    }
                    if (evalStack.Count < argCount) throw new InvalidOperationException($"Недостаточно операндов в стеке для '{funcName}' (нужно {argCount}, найдено {evalStack.Count}).");
                    var args = new object[argCount];
                    for (int i = argCount - 1; i >= 0; i--)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        args[i] = evalStack.Pop();
                    }
                    if (funcName == "vector")
                    {
                        evalStack.Push(new ComplexVector(args.Select(a => CastsVar.CastToComplex(a, "vector component"))));
                    }
                    else
                    {
                        if (!Functions.ContainsKey(funcName)) throw new NotSupportedException($"Функция '{funcName}' не найдена.");
                        var funcDef = Functions[funcName];
                        if (funcDef.ArgumentCount != -1 && funcDef.ArgumentCount != argCount) throw new ArgumentException($"Функция '{funcName}' ожидает {funcDef.ArgumentCount} аргументов, но получила {argCount}.");
                        evalStack.Push(funcDef.Delegate(args));
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Неизвестный токен в RPN: {token}");
                }
            }

            if (evalStack.Count > 1) throw new InvalidOperationException("Ошибка в синтаксисе выражения: в стеке осталось больше одного элемента.");
            return evalStack.Count == 0 ? null : evalStack.Pop();
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Применяет оператор к двум верхним элементам стека.
        /// </summary>
        private void ApplyOperator(string op, Stack<object> stack)
        {
            if (stack.Count < 2)
            {
                throw new InvalidOperationException($"Недостаточно операндов для оператора '{op}'.");
            }

            var op2 = stack.Pop();
            var op1 = stack.Pop();

            var normalizedOp1 = Normalize(op1);
            var normalizedOp2 = Normalize(op2);

            var key = (normalizedOp1.GetType(), normalizedOp2.GetType(), op);

            if (OperationsFunctions.TryGetValue(key, out var operationFunc))
            {
                object result = operationFunc(normalizedOp1, normalizedOp2);
                stack.Push(result);
            }
            else
            {
                // Если в словаре не нашлось подходящей операции, генерируем исключение
                throw new InvalidOperationException($"Оператор '{op}' не применим к типам {op1.GetType().Name} и {op2.GetType().Name}.");
            }
        }

        private object Normalize(object obj) => obj
        switch
        {
            double d => new Complex(d, 0),
            int i => new Complex(i, 0),
            Vector rv => new ComplexVector(rv.Select(c => new Complex(c, 0)).ToArray()),
            _ => obj
        };
        private bool IsValue(string token, ExecutionContext context) =>
            token == "i" || 
            (token.StartsWith("\"") && token.EndsWith("\"")) ||  // Строковый литерал
            double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _) || 
            context.Memory.ContainsKey(token);
        private bool IsValidVarName(string name) =>
            !string.IsNullOrWhiteSpace(name) && (char.IsLetter(name[0]) || name[0] == '_') && name.All(c => char.IsLetterOrDigit(c) || c == '_') && !Functions.ContainsKey(name);
        private bool IsSimpleAssignmentTarget(string expression) =>
            !expression.Split('=')[0].Any(c => "()[]<>!".Contains(c));

        #endregion
    }
}