namespace AI.Algorithms.TransportTask.PlanBuilders
{
    /// <summary>
    /// Интерфейс для создания начального плана
    /// </summary>
    public interface IInitialPlanBuilder
    {
        /// <summary>
        /// Метод создания начального плана
        /// </summary>
        /// <param name="costs">Матрица стоимостей</param>
        /// <param name="supply">Вектор предложений</param>
        /// <param name="demand">Вектор потребностей</param>
        double[,] BuildInitialPlan(double[,] costs, double[] supply, double[] demand);
    }
}
