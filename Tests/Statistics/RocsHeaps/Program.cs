using AI.Statistics.MonteCarlo;

Random random = new Random();


int count_rocks = 1800;
int count_heaps = 6;

int[] rocs = new int[count_rocks];
List<int>[] heaps = new List<int>[count_heaps];

// Инициализация камней
for (int i = 0; i < count_rocks; i++)
    rocs[i] = random.Next(100,1000);

// Инициализация куч
InitHeaps();
// Метод оптимизации
SimulatedAnnealing simulatedAnnealing = new SimulatedAnnealing(Cost(heaps))
{
    T = 20,
    Kt = 1.02
};


int iter = 0;
for (int i = 0; i < 1500000000; i++)
{
    double cost = Cost(heaps);
    if (cost <= 6) break; // Критерий останова
    if(i%1000 == 0)
        Console.WriteLine(cost);
    iter++;

    var new_heaps = Step(); // Делаем шаг оптимизации

    if (simulatedAnnealing.IsAccept(Cost(new_heaps)))
        heaps = new_heaps;
}

double cost1 = Cost(heaps);
Console.WriteLine($"iter: {iter}\ncost: {cost1}");



// Инициализация куч
void InitHeaps() 
{
    for (int i = 0; i < count_heaps; i++)
        heaps[i] = new List<int>();

    for(int i = 0; i < count_rocks; i++) 
    {
        int num_heap = random.Next(count_heaps);
        heaps[num_heap].Add(rocs[i]);
    }
}


List<int>[] Copy(List<int>[] heaps)
{
    List<int>[] heaps_new = new List<int>[heaps.Length];
    for (int i = 0; i < heaps.Length;i++)
        heaps_new[i] = new List<int>(heaps[i].Count);

    for (int i = 0; i < heaps.Length; i++)
            heaps_new[i].AddRange(heaps[i]);

    return heaps_new;
}

// Поменять камни в кучах
List<int>[] ExCh() 
{

    List<int>[] heaps_new = Copy(heaps);

    int heap1 = random.Next(count_heaps);
    int heap2 = random.Next(count_heaps);
    int position_in_heap1 = random.Next(heaps_new[heap1].Count);
    int position_in_heap2 = random.Next(heaps_new[heap2].Count);

    try
    {
        (heaps_new[heap2][position_in_heap2], heaps_new[heap1][position_in_heap1]) 
                                            = 
        (heaps_new[heap1][position_in_heap1], heaps_new[heap2][position_in_heap2]);
    }
    catch { }
    return heaps_new;
}

// Перекладывание
List<int>[] Translation() 
{
    List<int>[] heaps_new = Copy(heaps);
    int heap1 = random.Next(count_heaps);
    int heap2 = random.Next(count_heaps);
    int position_in_heap1 = random.Next(heaps_new[heap1].Count);
    try
    {
        int a = heaps_new[heap1][position_in_heap1];
        heaps_new[heap1].RemoveAt(position_in_heap1); // Берем из кучи 1
        heaps_new[heap2].Add(a); // Кладем в кучу 2
    }
    catch { }

    return heaps_new;
}

// 1 шаг
List<int>[] Step() 
{
    bool exch = random.NextDouble() > 0.5; // Выбор действия

    return exch ? ExCh() : Translation();
}

double Cost(List<int>[] heaps) 
{
    int[] costs = new int[count_heaps];

    for (int i = 0; i < count_heaps; i++)
        costs[i] = heaps[i].Sum();

    return costs.Max() - costs.Min();
}

