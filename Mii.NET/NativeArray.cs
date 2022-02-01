using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
{
    public bool IsNull => ptr == null;

    public readonly int Size;
    T* ptr;

    public ref T Ref(int index) => ref ptr[index];

    public T this[int index]
    {
        get => ptr[index];
        set => ptr[index] = value;
    }

    public void Dispose() => NativeMemory.Free(ptr);

    public SmartPointer CreateSmartPointer() => new SmartPointer(ptr);

    public NativeArray<T> SmartClean()
    {
        _ = new SmartPointer(ptr);
        return this;
    }

    public static implicit operator Span<T>(NativeArray<T> narray) => new Span<T>(narray.ptr, narray.Size);
    public static implicit operator T*(NativeArray<T> narray) => narray.ptr;
    public static implicit operator void*(NativeArray<T> narray) => narray.ptr;

    public static implicit operator ReadOnlySpan<T>(NativeArray<T> array) => new ReadOnlySpan<T>(array.ptr, array.Size);

    public NativeArray(int size)
    {
        ptr = (T*)NativeMemory.Alloc((nuint)size, (nuint)sizeof(T));
        Size = size;
    }
    public NativeArray(Span<T> span)
    {
        int size = span.Length;
        ptr = (T*)NativeMemory.Alloc((nuint)size, (nuint)sizeof(T));
        Size = size;

        for (int i = 0; i < span.Length; i++)
            ptr[i] = span[i];
    }
}
