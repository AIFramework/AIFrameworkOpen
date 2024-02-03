using System;
using System.Collections.Generic;
using System.Linq;

namespace AI.Faiss.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
    {
        if (size <= 0)
            throw new ArgumentException("Chunk size must be greater than 0.", nameof(size));

        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return GetChunk(enumerator, size).ToArray();
        }
    }

    private static IEnumerable<T> GetChunk<T>(IEnumerator<T> enumerator, int chunkSize)
    {
        do
        {
            yield return enumerator.Current;
        } while (--chunkSize > 0 && enumerator.MoveNext());
    }
}

