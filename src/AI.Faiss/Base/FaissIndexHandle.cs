using System;
using System.Runtime.InteropServices;

namespace AI.Faiss.Base;

internal class FaissIndexHandle : SafeHandle
{
    public override bool IsInvalid => handle == IntPtr.Zero;

    public FaissIndexHandle(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
    {
        
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
            FaissNative.FN_Release(handle);

        return !IsInvalid;
    }

    internal IntPtr Pointer => !IsInvalid ? handle : throw new Exception("handle not valid");
}
