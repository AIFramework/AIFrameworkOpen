using System;

namespace AI.DataPrepaire.Pipelines.RL
{
    /// <summary>
    /// Конвейер создания критика (Оценщика состояний)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CriticPipeline<T> : ObjectRegressionPipeline<T>
    {

    }
}
