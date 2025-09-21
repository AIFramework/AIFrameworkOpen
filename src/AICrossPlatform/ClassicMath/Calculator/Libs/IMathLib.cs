using System.Collections.Generic;

namespace AI.ClassicMath.Calculator.Libs;

/// <summary>
/// Интерфейс библиотеки математических функций
/// </summary>
public interface IMathLib
{
    /// <summary>
    /// Имя библиотеки
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Описание библиотеки
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Отдает функции из библиотеки
    /// </summary>
    public Dictionary<string, FunctionDefinition> GetFunctions();
}