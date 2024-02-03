using AI.Faiss;
using AI.Faiss.Enums;

Random rnd = new(1);

Example1();

void Example1()
{
    int dimension = 3;
    int vectorsCount = 5;
    int topK = 5;
    using var faissIndex = FaissIndex.CreateDefault(dimension, MetricType.METRIC_L2);
    var data = new float[vectorsCount][];
    for (int i = 0; i < vectorsCount; i++)
    {
        data[i] = new float[dimension];
        for (int j = 0; j < dimension; j++)
            data[i][j] = (float)Math.Round(rnd.NextDouble() - 0.4, 3);
        
        Console.WriteLine("Vector index={0} data={1}", i, string.Join(" ", data[i]));
    }
    Console.WriteLine();

    long[] ids = new long[vectorsCount];
    for (int i = 0; i < vectorsCount; i++)
        ids[i] = i;

    faissIndex.AddWithIds(data, ids);

    var searchVector = new float[dimension];
    for (int i = 0; i < dimension; i++)
    {
        searchVector[i] = data[3][i] + (float)rnd.NextDouble() / 3.0f;
    }
    var (nbrDists, nbrIds) = faissIndex.Search(new float[][] { searchVector }, 5);

    for (int i = 0; i < topK; i++)
    {
        Console.WriteLine("index={0} distance={1}", nbrIds[0][i], nbrDists[0][i]);
    }
}