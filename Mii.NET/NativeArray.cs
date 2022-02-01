using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// Native array based on new .NET 6 NativeMemory class
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
{
    /// <summary>
    /// Verify if this array points to a null pointer
    /// </summary>
    public bool IsNull => ptr == null;

    /// <summary>
    /// The specified size of this array
    /// </summary>
    public readonly int Size;
    T* ptr;

    /// <summary>
    /// Get's a reference from specific index of this array
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ref T Ref(int index) => ref ptr[index];

    public T this[int index]
    {
        get => ptr[index];
        set => ptr[index] = value;
    }

    /// <summary>
    /// Releases allocated memory of this array
    /// </summary>
    public void Dispose() => NativeMemory.Free(ptr);

    /// <summary>
    /// Returns an smart pointer from this array that will dispose internal pointer when needed
    /// </summary>
    /// <returns></returns>
    public SmartPointer CreateSmartPointer() => new SmartPointer(ptr);

    /// <summary>
    /// Create an smart pointer internally on this array and make's this array be disposed
    /// </summary>
    /// <returns></returns>
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
