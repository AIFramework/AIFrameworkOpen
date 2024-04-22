using System;
using System.Runtime.InteropServices;
using AI.Faiss.Enums;

namespace AI.Faiss.Base;

internal class FaissNative
{
    [DllImport("faiss_c", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    internal static extern IntPtr FN_Create(int dimension, [MarshalAs(UnmanagedType.LPStr)] string description, MetricType metric);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern IntPtr FN_CreateDefault(int dimension, MetricType metric);

    [DllImport("faiss_c", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    internal static extern void FN_WriteIndex(IntPtr idx, [MarshalAs(UnmanagedType.LPStr)] string path);

    [DllImport("faiss_c", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    internal static extern IntPtr FN_ReadIndex([MarshalAs(UnmanagedType.LPStr)] string path);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_Add(IntPtr idx, int n, float* x);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_AddWithIds(IntPtr idx, int n, float* x, long* ids);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_Search(IntPtr idx, int n, float* x, int k, float* distances, long* labels);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_Assign(IntPtr idx, int n, float* x, long* labels, int k);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_Train(IntPtr idx, int n, float* x);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern void FN_Reset(IntPtr idx);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_RemoveIds(IntPtr idx, int n, long* ids);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_ReconstructBatch(IntPtr idx, int n, long* ids, float* recons);

    [DllImport("faiss_c", SetLastError = true)]
    unsafe internal static extern void FN_SearchAndReconstruct(IntPtr idx, int n, float* x, int k, float* distances, long* labels, float* recons);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern void FN_MergeFrom(IntPtr idx, IntPtr otherIndex, Int64 add_id);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern void FN_Release(IntPtr idx);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern int FN_Dimension(IntPtr idx);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern MetricType FN_MetricType(IntPtr idx);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern Int64 FN_Count(IntPtr idx);

    [DllImport("faiss_c", SetLastError = true)]
    internal static extern IntPtr FN_GetLastError();
}
