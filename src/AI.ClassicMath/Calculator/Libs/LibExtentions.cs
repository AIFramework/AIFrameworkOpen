using System;
using System.Collections.Generic;
using System.Text;

namespace AI.ClassicMath.Calculator.Libs;

public static class LibExtentions
{
    public static string GetDescription(this IMathLib mathLib) 
    {
        StringBuilder sb = new StringBuilder();
        var functionDict = mathLib.GetFunctions();

        sb.AppendLine($"Имя библиотеки: {mathLib.Name}");
        sb.AppendLine($"Описание библиотеки: {mathLib.Description}\n");
        sb.AppendLine($"\n## Функции в библиотеки\n\n");
        
        foreach (var item in functionDict)
        {
            var func = item.Value;
            var description = func.Description.ToString();
            var count = func.ArgumentCount == -1 ? "не фиксированное число" : $"{func.ArgumentCount}"; 

            sb.AppendLine($"\n**Имя функции**: {func.Name}");
            sb.AppendLine($"Число аргументов: {count}");
            sb.AppendLine(description);
            sb.AppendLine("---");
        }


        return sb.ToString();
    }
}
