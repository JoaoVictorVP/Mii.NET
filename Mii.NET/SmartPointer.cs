using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// A type used to free memory of pointers that have not been used so long, as GC will collect the SmartPointer he will free pointer on Dispose method
/// </summary>
public unsafe class SmartPointer : IDisposable
{
    public readonly void* Ptr;
    public void Dispose()
    {
        NativeMemory.Free(Ptr);
    }

    ~SmartPointer() => Dispose();

    public SmartPointer(void* ptr)
    {
        Ptr = ptr;
    }
}