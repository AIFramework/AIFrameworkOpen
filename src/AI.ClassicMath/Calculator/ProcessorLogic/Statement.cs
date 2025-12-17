using System.Collections.Generic;
using System.Threading;

namespace AI.ClassicMath.Calculator.ProcessorLogic;

/// <summary>
/// Базовый класс для любой инструкции в скрипте.
/// </summary>
internal abstract class Statement
{
    public abstract void Execute(Processor processor, ExecutionContext context, List<string> output, CancellationToken cancellationToken = default);
}