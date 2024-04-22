using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AI.Faiss.Base;
using AI.Faiss.Enums;
using AI.Faiss.Extensions;
using static AI.Faiss.Base.FaissNative;
namespace AI.Faiss;

public class FaissIndex : IDisposable
{
    private readonly FaissIndexHandle _indexHandle;

    internal FaissIndex(FaissIndexHandle indexHandle) => _indexHandle = indexHandle;

    /// <summary>
    /// Создает индекс указанного типа, определенного параметром конструктора
    /// Смотрите <see href="https://github.com/facebookresearch/faiss/wiki/The-index-factory">https://github.com/facebookresearch/faiss/wiki/The-index-factoryfaiss</see> для синтаксиса.
    /// </summary>
    /// <param name="dimension">размерность вектора</param>
    /// <param name="constructor">строка конструктора индекса faiss</param>
    /// <param name="metric">метрика расстояния</param>
    /// <returns></returns>
    public static FaissIndex Create(int dimension, string constructor, MetricType metric)
    {
        var ptr = FN_Create(dimension, constructor, metric);
        return new FaissIndex(new FaissIndexHandle(ptr, true));
    }

    /// <summary>
    /// Создает индекс стандартного типа, используя строку "IDMap2,HNSW32"
    /// </summary>
    /// <param name="dimension">размерность вектора</param>
    /// <param name="metric">метрика расстояния</param>
    /// <returns></returns>
    public static FaissIndex CreateDefault(int dimension, MetricType metric)
    {
        var ptr = FN_CreateDefault(dimension, metric);
        return new FaissIndex(new FaissIndexHandle(ptr, true));
    }

    /// <summary>
    /// Загрузить сохраненный индекс
    /// </summary>
    /// <param name="path">путь к файлу</param>
    /// <returns></returns>
    public static FaissIndex Load(string path) =>
        Run(() =>
        {
            var handle = FN_ReadIndex(path);
            return new FaissIndex(new FaissIndexHandle(handle, true));
        });

    /// <summary>
    /// Сохранить индекс
    /// </summary>
    /// <param name="path">путь к файлу</param>
    public void Save(string path) =>
        Do(() => FN_WriteIndex(_indexHandle.Pointer, path));

    /// <summary>
    /// Количество элементов в индексе
    /// </summary>
    public Int64 Count => Run(() => FN_Count(_indexHandle.Pointer));

    /// <summary>
    /// Метрика расстояния, указанная при создании индекса
    /// </summary>
    public MetricType MetricType => Run(() => FN_MetricType(_indexHandle.Pointer));

    /// <summary>
    /// Размерность векторов индекса, указанная при создании индекса
    /// </summary>
    public int Dimension => Run(() => FN_Dimension(_indexHandle.Pointer));

    /// <summary>
    /// Добавление векторов в индекс (может быть не определено для некоторых типов индексов).
    /// </summary>
    /// <param name="n">количество векторов (указанной размерности)</param>
    /// <param name="vectors">вектора размером n * dimension</param>
    public void AddFlat(int n, float[] vectors) =>
        Do(() =>
        {
            var sp = new ReadOnlySpan<float>(vectors);
            unsafe
            {
                fixed (float* ptr = sp)
                {
                    FN_Add(_indexHandle.Pointer, n, ptr);
                }
            }
        });

    /// <summary>
    /// Аналогично AddFlat, но для двухмерных массивов (может быть не определено для некоторых типов индексов)
    /// </summary>
    /// <param name="vectors">Двухмерный массив векторов (каждый вектор имеет размерность dimension)</param>
    public void Add(float[][] vectors) =>
        Do(() =>
        {
            foreach (var vector in vectors)
            {
                var sp = new ReadOnlySpan<float>(vector);
                unsafe
                {
                    fixed (float* ptr = sp)
                    {
                        FN_Add(_indexHandle.Pointer, 1, ptr);
                    }
                }
            };
        });

    /// <summary>
    /// Добавление векторов и идентификаторов в индекс (может быть не определено для некоторых типов индексов)
    /// </summary>
    /// <param name="n">количество векторов (указанной размерности)</param>
    /// <param name="vectors">вектора размером n * dimension</param>
    /// <param name="ids">идентификаторы</param>
    public void AddWithIdsFlat(int n, float[] vectors, long[] ids) =>
        Do(() =>
        {
            var idSpan = new ReadOnlySpan<long>(ids);
            var sp = new ReadOnlySpan<float>(vectors);
            unsafe
            {
                fixed (float* ptrVec = sp)
                fixed (long* pIds = idSpan)
                {
                    FN_AddWithIds(_indexHandle.Pointer, n, ptrVec, pIds);
                }
            }
        });

    /// <summary>
    /// Аналогично AddWithIdsFlat, но для двухмерных массивов (может быть не определено для некоторых типов индексов)
    /// </summary>
    /// <param name="vectors">Двухмерный массив векторов (каждый вектор имеет размерность dimension)</param>
    /// <param name="ids">идентификаторы</param>
    public void AddWithIds(float[][] vectors, long[] ids) =>
        Do(() =>
        {
            var idSpan = new ReadOnlySpan<long>(ids);
            for (int i = 0; i < vectors.Length; i++)
            {
                //По умолчанию, faiss копирует данные в свою собственную память
                var sp = new ReadOnlySpan<float>(vectors[i]);
                unsafe
                {
                    fixed (float* ptrVec = sp)
                    {
                        fixed (long* ptrIdx = idSpan.Slice(i, 1))
                            FN_AddWithIds(_indexHandle.Pointer, 1, ptrVec, ptrIdx);
                    }
                }
            }
        });

    /// <summary>
    /// Поиск ближайших соседей в индексе для заданных векторов
    /// </summary>
    /// <param name="n">количество входных векторов</param>
    /// <param name="vectors">одномерный массив входных векторов размером n * dimension</param>
    /// <param name="k">количество соседей</param>
    /// <returns>кортеж: (расстояния [n*k], идентификаторы соседей [n*k])</returns>
    public Tuple<float[], long[]> SearchFlat(int n, float[] vectors, int k)
    {
        var distances = new float[n * k];
        var labels = new long[n * k];
        Do(() =>
        {
            var distSpan = new Span<float>(distances);
            var lblSpan = new Span<long>(labels);
            var vecSpan = new ReadOnlySpan<float>(vectors);
            unsafe
            {
                fixed (float* ptrDists = distSpan)
                fixed (float* ptrVecs = vecSpan)
                fixed (long* ptrLbls = lblSpan)
                {
                    FN_Search(_indexHandle.Pointer, n, ptrVecs, k, ptrDists, ptrLbls);
                }
            }
        });

        return Tuple.Create(distances, labels);
    }

    /// <summary>
    /// Аналогично SearchFlat, но для двухмерных массивов
    /// </summary>
    /// <param name="vectors">вектора размером n * dimension</param>
    /// <param name="k">количество возвращаемых ближайших соседей</param>
    /// <returns>кортеж: (расстояния [n][k], идентификаторы соседей [n][k])</returns>
    public Tuple<float[][], long[][]> Search(float[][] vectors, int k)
    {
        var vecs = vectors.SelectMany(r => r).ToArray();
        var (dists, lbls) = SearchFlat(vectors.Length, vecs, k);
        return Tuple.Create(dists.Chunk(k).ToArray(), lbls.Chunk(k).ToArray());
    }

    /// <summary>
    /// Поиск соседей для заданных векторов и возврат векторов соседей
    /// Может быть не определено для некоторых типов индексов
    /// Возвращает кортеж плоских массивов: расстояния [n*k], идентификаторы соседей [n*k], и векторы соседей [n*k*dimension]
    /// </summary>
    /// <param name="n">количество входных векторов</param>
    /// <param name="vectors">одномерный массив входных векторов размером n * dimension</param>
    /// <param name="k">количество соседей</param>
    /// <returns>кортеж: (расстояния [n*k], идентификаторы соседей [n*k], векторы соседей [n*k*dimension])</returns>
    public Tuple<float[], long[], float[]> SearchAndReconstruct(int n, float[] vectors, int k)
    {
        var distances = new float[n * k];
        var labels = new long[n * k];
        var recons = new float[n * k * this.Dimension];
        Do(() =>
        {
            var distSpan = new Span<float>(distances);
            var lblSpan = new Span<long>(labels);
            var reconSpan = new Span<float>(recons);
            var vecSpan = new ReadOnlySpan<float>(vectors);
            unsafe
            {
                fixed (float* ptrDists = distSpan)
                fixed (float* ptrVecs = vecSpan)
                fixed (long* ptrLbls = lblSpan)
                fixed (float* ptrRecons = reconSpan)
                {
                    FN_SearchAndReconstruct(_indexHandle.Pointer, n, ptrVecs, k, ptrDists, ptrLbls, ptrRecons);
                }
            }
        });

        return Tuple.Create(distances, labels, recons);
    }

    /// <summary>
    /// Вернуть векторы по заданным идентификаторам
    /// Возвращает одномерный массив векторов размером ids.Length * dimension
    /// </summary>
    /// <param name="ids">массив идентификаторов</param>
    /// <returns>векторы [ids.Length * dimension]</returns>
    public float[] Reconstruct(long[] ids)
    {
        var recons = new float[ids.Length * this.Dimension];
        Do(() =>
        {
            var idSpan = new ReadOnlySpan<long>(ids);
            var reconSpan = new Span<float>(recons);
            unsafe
            {
                fixed (long* ptrIds = idSpan)
                fixed (float* ptrVecs = reconSpan)
                {
                    FN_ReconstructBatch(_indexHandle.Pointer, ids.Length, ptrIds, ptrVecs);
                }
            }
        });

        return recons;
    }

    /// <summary>
    /// Обучение индекса на представительном наборе векторов
    /// Может быть не определено для некоторых типов индексов
    /// </summary>
    /// <param name="n">количество обучающих векторов</param>
    /// <param name="vectors">одномерный массив векторов [n * dimension]</param>
    /// 
    public void Train(int n, float[] vectors) =>
        Do(() =>
        {
            var vecSpan = new ReadOnlySpan<float>(vectors);
            unsafe
            {
                fixed (float* ptrVecs = vecSpan)
                {
                    FN_Train(_indexHandle.Pointer, n, ptrVecs);
                }
            }
        });

    /// <summary>
    /// Находит идентификаторы для заданных векторов
    /// Может быть не определено для некоторых типов индексов
    /// Возвращает вектор размера n, содержащий связанные идентификаторы
    /// </summary>
    /// <param name="n">количество векторов</param>
    /// <param name="vectors">одномерный массив векторов [n * dimension]</param>
    /// <returns>вектор размера n, содержащий связанные идентификаторы</returns>
    public long[] Assign(int n, float[] vectors)
    {
        var ids = new long[n];
        Do(() =>
        {
            var vecSpan = new ReadOnlySpan<float>(vectors);
            var idsSpan = new Span<long>(ids);
            unsafe
            {
                fixed (float* ptrVecs = vecSpan)
                fixed (long* ptrIds = idsSpan)
                {
                    FN_Assign(_indexHandle.Pointer, n, ptrVecs, ptrIds, 1);
                }
            }
        });
        return ids;
    }

    /// <summary>
    /// Удаление сохраненных векторов с заданными идентификаторами из индекса (может не поддерживаться некоторыми типами индексов)
    /// </summary>
    /// <param name="ids"></param>
    public void RemoveIds(long[] ids) =>
        Do(() =>
        {
            var idsSpan = new ReadOnlySpan<long>(ids);
            unsafe
            {
                fixed (long* ptrIds = idsSpan)
                {
                    FN_RemoveIds(_indexHandle.Pointer, ids.Length, ptrIds);
                }
            }
        });

    /// <summary>
    /// Объединение данных из другого индекса
    /// Другой индекс становится пустым
    /// (может быть не реализовано для некоторых типов индексов).
    /// </summary>
    /// <param name="otherIndex">индекс с которым нужно мержить</param>
    /// <param name="add_id">добавление к идентификатору каждого элемента другого индекса</param>
    public void MergeFrom(FaissIndex otherIndex, long add_id = 0L) =>
        Do(() => FN_MergeFrom(_indexHandle.Pointer, otherIndex._indexHandle.Pointer, add_id));

    /// <summary>
    /// Получить последнее сообщение об ошибке
    /// </summary>
    public static string LastError()
    {
        var ptr = FN_GetLastError();
        if (ptr == IntPtr.Zero)
            return "Сообщение о последней ошибке отсутствует";

        try
        {
            return Marshal.PtrToStringAnsi(ptr);
        }
        catch
        {
            return "Невозможно получить информацию об ошибке";
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Run<T>(Func<T> comp)
    {
        try
        {
            return comp();
        }
        catch (SEHException ex)
        {
            var msg = LastError();
            throw new Exception(msg, ex);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Do(Action comp)
    {
        try
        {
            comp();
        }
        catch (SEHException ex)
        {
            var msg = LastError();
            throw new Exception(msg, ex);
        }
    }

    public void Dispose()
    {
        _indexHandle?.Dispose();
    }
}
