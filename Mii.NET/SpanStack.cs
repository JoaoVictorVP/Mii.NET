using System.Runtime.InteropServices;

namespace IzaBlockchain.Net;

/// <summary>
/// An smart stack-living span that will use safely stack allocations or native memory all automatically.
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe ref struct SpanStack<T> where T : unmanaged
{
    public Span<T> Span;
    public int Size => size;
    public int Capacity => capacity;
    int size;
    int capacity;

    T* ptr;
    bool malloc => sizeof(T) * Capacity > 1024;

    public void PushAll(Span<T> values)
    {
        size += values.Length;
        if (size >= capacity)
            IncreaseSpan();
        for (int i = Size - values.Length; i < values.Length; i++)
            Span[i] = values[i];
    }
    public void PushAll(T* values, int length)
    {
        size += length;
        if (size >= capacity)
            IncreaseSpan();
        for (int i = size - length; i < length; i++)
            Span[i] = values[i];
    }

    public void Push(T value)
    {
        size++;
        if (size >= capacity)
            IncreaseSpan();
        Span[Size - 1] = value;
    }
    public T Pop()
    {
        ref var ret = ref Span[size - 1];
        size--;
        return ret;
    }
    public T Peek() => Span[size - 1];

    void IncreaseSpan()
    {
        capacity *= 2;
        var nspan = GetSpan(capacity);
        Span.CopyTo(nspan);

        Span = nspan;
    }

    public void Initialize(Span<T> from)
    {
        capacity = from.Length;
        Span = GetSpan(capacity);
        from.CopyTo(Span);
        size = Capacity;
    }
    public void Initialize(int size)
    {
        capacity = size;
        Span = GetSpan(capacity);
        this.size = size;
    }

    public void Dispose()
    {
        if (malloc && ptr != null)
            NativeMemory.Free(ptr);
        ptr = null;
    }

    Span<T> GetSpan(int capacity)
    {
        Dispose();
        if (malloc)
            return new Span<T>(ptr = (T*)NativeMemory.Alloc((nuint)capacity, (nuint)sizeof(T)), sizeof(T) * capacity);
        var span = stackalloc T[capacity];
        return new Span<T>(span, capacity);
    }

    public SpanStack()
    {
        size = 0;
        capacity = 0;
        Span = default;

        ptr = null;
    }
}
