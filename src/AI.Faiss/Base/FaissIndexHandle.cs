using System;
using System.Runtime.InteropServices;

namespace AI.Faiss.Base;

internal class FaissIndexHandle(IntPtr invalidHandleValue, bool ownsHandle) : SafeHandle(invalidHandleValue, ownsHandle)
{
    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
            FaissNative.FN_Release(handle);

        return !IsInvalid;
    }

    internal IntPtr Pointer => !IsInvalid ? handle : throw new Exception("handle not valid");
}
