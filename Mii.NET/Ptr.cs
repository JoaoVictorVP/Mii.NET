using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// A simple struct to represent a pointer in a relatively safe context, can be used on generic arguments
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe struct Ptr<T> : IDisposable where T : unmanaged
{
    public static readonly Ptr<T> Null = new Ptr<T>();

    public T* To;
    public bool IsNull => To == null;

    public ref T On => ref *To;

    public void Alloc()
    {
        To = (T*)NativeMemory.Alloc((nuint)sizeof(T));
    }
    public void Dispose() => NativeMemory.Free(To);

    public Ptr(bool alloc)
    {
        To = null;
        if (alloc)
            Alloc();
    }
    public Ptr(ref T from)
    {
        fixed (T* ptr = &from)
            To = ptr;
    }
    public Ptr(T* from)
    {
        To = from;
    }
}