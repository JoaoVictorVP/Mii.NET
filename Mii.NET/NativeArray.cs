using System.Collections;
using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// Native array based on new .NET 6 NativeMemory class
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe struct NativeArray<T> : IDisposable, IEnumerable<T> where T : unmanaged
{
    /// <summary>
    /// Verify if this array points to a null pointer
    /// </summary>
    public bool IsNull => ptr == null;

    /// <summary>
    /// The specified size of this array
    /// </summary>
    public readonly int Size;

    /// <summary>
    /// A property to make <see cref="NativeArray{T}"/> support <see cref="Range"/> operations
    /// </summary>
    public int Length => Size;

    public T* Ptr => ptr;
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

    /// <summary>
    /// Slice this <see cref="NativeArray{T}"/> into a new array of specified <paramref name="size"/> starting from index <paramref name="start"/>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public NativeArray<T> Slice(int start, int size)
    {
        var sliceArr = new NativeArray<T>(size);
        Span<T> span = this;
        span = span.Slice(start, size);

        span.CopyTo(sliceArr);

        return sliceArr;
    }

    /// <summary>
    /// Create a literal copy of this array (with another memory allocation, so it need to be disposed separately)
    /// </summary>
    /// <returns></returns>
    public NativeArray<T> Clone() => new NativeArray<T>(this);

    public static implicit operator Span<T>(NativeArray<T> narray) => new Span<T>(narray.ptr, narray.Size);
    public static implicit operator T*(NativeArray<T> narray) => narray.ptr;
    public static implicit operator void*(NativeArray<T> narray) => narray.ptr;

    public static implicit operator ReadOnlySpan<T>(NativeArray<T> array) => new ReadOnlySpan<T>(array.ptr, array.Size);

    public NativeArray(int size)
    {
        ptr = (T*)NativeMemory.Alloc((nuint)size, (nuint)sizeof(T));
        Size = size;
    }
    public NativeArray(ReadOnlySpan<T> span)
    {
        int size = span.Length;
        ptr = (T*)NativeMemory.Alloc((nuint)size, (nuint)sizeof(T));
        Size = size;

        for (int i = 0; i < span.Length; i++)
            ptr[i] = span[i];
    }
    /*public NativeArray(IEnumerable<T> enumerable)
    {
        Size = enumerable.Count();
        ptr = (T*)NativeMemory.Alloc((nuint)Size, (nuint)sizeof(T));
        int index = 0;
        foreach(var item in enumerable)
        {
            ptr[index] = item;
            index++;
        }
    }*/

    public override string ToString()
    {
        string ret = $"NativeArray<{typeof(T).Name}> of size: {Size},\n{{ ";
        for (int i = 0; i < Size; i++)
            ret += $"{i}: [{ptr[i]}] ";
        return ret + '}';
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Size; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
