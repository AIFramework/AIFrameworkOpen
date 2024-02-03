namespace AI.Faiss.Enums;

/// <summary>
/// Метрики расстояния из faiss. Данный тип индекса может не поддерживать все типы метрик.
/// </summary>
public enum MetricType
{
    METRIC_INNER_PRODUCT = 0, //< поиск максимального скалярного произведения
    METRIC_L2 = 1,            //< поиск квадрата L2
    METRIC_L1,                //< L1 (также известный как манхэттенское расстояние)
    METRIC_Linf,              //< расстояние по бесконечной норме
    METRIC_Lp,                //< расстояние L_p, p задается через faiss::Index
                              // metric_arg

    /// некоторые дополнительные метрики, определенные в scipy.spatial.distance
    METRIC_Canberra = 20,
    METRIC_BrayCurtis,
    METRIC_JensenShannon,
    METRIC_Jaccard, //< определяется как: sum_i(min(a_i, b_i)) / sum_i(max(a_i, b_i))
                    //< где a_i, b_i > 0
};
