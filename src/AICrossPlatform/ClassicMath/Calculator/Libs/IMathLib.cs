using System.Collections.Generic;

namespace AI.ClassicMath.Calculator.Libs;

/// <summary>
/// Интерфейс библиотеки математических функций
/// </summary>
public interface IMathLib
{
    /// <summary>
    /// Отдает функции из библиотеки
    /// </summary>
    public Dictionary<string, FunctionDefinition> GetFunctions();
}