using System.Collections;
using System.Runtime.CompilerServices;

namespace IzaBlockchain.Net;

public unsafe struct NativeList<T> : IDisposable, IList<T> where T : unmanaged
{
    public T this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }
    public ref T FromIndex(int index)
    {
        return ref array.Ref(index);
    }

    int size = 0;
    int capacity = 0;
    NativeArray<T> array = default;

    bool readOnly;

    public int Count => size;

    public bool IsReadOnly => readOnly;

    public void Dispose() => array.Dispose();

    #region Public

    public void Add(T item)
    {
        array[size] = item;
        size++;
    }
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0)
            return false;
        RemoveAt(index);
        return true;
    }
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= size)
            throw new IndexOutOfRangeException();

        if (index == size - 1)
            array[index] = default;
        else
        {
            for (int i = index + 1; i < size; i++)
                array[i - 1] = array[i];
        }
        size--;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int IndexOf(T item)
    {
        int itemHash = item.GetHashCode();
        for (int i = 0; i < size; i++)
            if (array[i].GetHashCode() == itemHash)
                return i;
        return -1;
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    void checkBounds()
    {
        if(size >= capacity)
        {
            capacity *= 2;
            newOrExtendArrayForCapacity();
        }
    }

    void newOrExtendArrayForCapacity()
    {
        var arr = new NativeArray<T>(capacity);
        if (!array.IsNull)
        {
            if (capacity < array.Size)
                throw new Exception("Can't bind lower capacity to a list");
            ((Span<T>)array).CopyTo(arr);
            array.Dispose();
        }
        array = arr;
    }

    public void Insert(int index, T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        for (int i = 0; i < size; i++)
            array[i] = default;
        size = 0;
    }

    public bool Contains(T item)
    {
        int itemHash = item.GetHashCode();
        for (int i = 0; i < size; i++)
            if (array[i].GetHashCode() == itemHash)
                return true;
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((Span<T>)array).CopyTo(array);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
            yield return array[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < size; i++)
            yield return array[i];
    }

    public NativeList<T> AsReadOnly()
    {
        var copy = this;
        copy.readOnly = true;
        return copy;
    }

    public NativeList(IList<T> collection)
    {
        capacity = collection.Count;
        readOnly = false;
        newOrExtendArrayForCapacity();
        foreach (var item in collection)
            Add(item);
    }
    public NativeList(IEnumerable<T> collection)
    {
        capacity = collection.Count();
        readOnly = false;
        newOrExtendArrayForCapacity();
        foreach (var item in collection)
            Add(item);
    }
    public NativeList(int capacity)
    {
        this.capacity = capacity;
        readOnly = false;
        newOrExtendArrayForCapacity();
    }
}
