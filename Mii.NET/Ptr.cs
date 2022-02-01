using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// A simple struct to represent a pointer in a relatively safe context, can be used on generic arguments
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe struct Ptr<T> : IDisposable where T : unmanaged
{
    /// <summary>
    /// A null ptr version of <see cref="Ptr{T}"/>
    /// </summary>
    public static readonly Ptr<T> Null = new Ptr<T>();

    public T* To;

    /// <summary>
    /// Is this pointer null
    /// </summary>
    public bool IsNull => To == null;

    /// <summary>
    /// Get's an reference for T from pointer
    /// </summary>
    public ref T On => ref *To;

    /// <summary>
    /// Malloc for <typeparamref name="T"/>
    /// </summary>
    public void Alloc()
    {
        To = (T*)NativeMemory.Alloc((nuint)sizeof(T));
    }
    /// <summary>
    /// Release memory from <typeparamref name="T"/>
    /// </summary>
    public void Dispose() => NativeMemory.Free(To);

    /// <summary>
    /// Create's a new instance of pointer
    /// </summary>
    /// <param name="alloc">Malloc <typeparamref name="T"/></param>
    public Ptr(bool alloc)
    {
        To = null;
        if (alloc)
            Alloc();
    }
    /// <summary>
    /// Create a new instance of pointer from reference
    /// </summary>
    /// <param name="from">The reference of <typeparamref name="T"/> to get</param>
    public Ptr(ref T from)
    {
        fixed (T* ptr = &from)
            To = ptr;
    }
    /// <summary>
    /// Create a new instance of pointer from unsafe pointer
    /// </summary>
    /// <param name="from">Unsafe pointer to store inside</param>
    public Ptr(T* from)
    {
        To = from;
    }
}