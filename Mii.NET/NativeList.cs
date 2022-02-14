using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace IzaBlockchain.Net;

/// <summary>
/// Implementation of list based on <see cref="NativeArray{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public unsafe struct NativeList<T> : IDisposable, IList<T> where T : unmanaged
{
    public T this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }
    /// <summary>
    /// Get's an reference to an item on specified index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ref T FromIndex(int index)
    {
        return ref array.Ref(index);
    }

    int size = 0;
    int capacity = 0;
    NativeArray<T> array = default;

    bool readOnly;

    /// <summary>
    /// Current count of itens on this list
    /// </summary>
    public int Count => size;

    /// <summary>
    /// Is this list a read-only list?
    /// </summary>
    public bool IsReadOnly => readOnly;

    /// <summary>
    /// Releases allocated memory from this list
    /// </summary>
    public void Dispose() => array.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    void add(T item, bool checkBounds)
    {
        int index = size;

        size++;
        if (checkBounds)
            this.checkBounds();

        array[index] = item;
    }

    #region Public

    /// <summary>
    /// Verify if a <typeparamref name="T"/> item matching <paramref name="predicate"/> exist on this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Exists(delegate*<int, T, bool> predicate)
    {
        for (int i = 0; i < size; i++)
            if (predicate(i, array[i]))
                return true;
        return false;
    }

    /// <summary>
    /// Find a <typeparamref name="T"/> item matching <paramref name="predicate"/>, return default if it doesn't exist
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T Find(delegate*<int, T, bool> predicate)
    {
        for (int i = 0; i < size; i++)
        {
            ref T item = ref array.Ref(i);
            if (predicate(i, item))
                return item;
        }
        return default;
    }

    /// <summary>
    /// Iterates through all <typeparamref name="T"/> items on this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="iterator"></param>
    public void ForEach(delegate*<int, T, void> iterator)
    {
        for (int i = 0; i < size; i++)
            iterator(i, array[i]);
    }

    /// <summary>
    /// Add a new <paramref name="item"/> to this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item) => add(item, true);
    /// <summary>
    /// Add a range of <paramref name="items"/> to this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="items"></param>
    public void AddRange(IList<T> items)
    {
        int count = items.Count;
        int nSize = size + count;
        if (nSize > capacity)
        {
            capacity += nSize - capacity;
            newOrExtendArrayForCapacity();
        }
        for (int i = 0; i < count; i++)
            add(items[i], false);
    }
    public void AddRange(Span<T> items)
    {
        int count = items.Length;
        int nSize = size + count;
        if(nSize > capacity)
        {
            capacity += nSize - capacity;
            newOrExtendArrayForCapacity();
        }
        for (int i = 0; i < count; i++)
            add(items[i], false);
    }

    /// <summary>
    /// Remove all items that matches <paramref name="predicate"/>
    /// </summary>
    /// <param name="predicate"></param>
    public void RemoveAll(delegate*<int, T, bool> predicate)
    {
        using NativeList<T> toRemove = new NativeList<T>(32);
        for(int i = 0; i < size; i++)
        {
            ref T item = ref array.Ref(i);
            if (predicate(i, item))
                toRemove.Add(item);
        }

        for (int i = 0; i < toRemove.size; i++)
            Remove(toRemove[i]);
    }
    /// <summary>
    /// Remove a <paramref name="item"/> from this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0)
            return false;
        RemoveAt(index);
        return true;
    }
    /// <summary>
    /// Remove a item from this <see cref="NativeList{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
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
    /// <summary>
    /// Get the index of <paramref name="item"/> in this <see cref="NativeList{T}"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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
            capacity = size * 2;
            newOrExtendArrayForCapacity();
        }
    }

    void newOrExtendArrayForCapacity()
    {
        if (array.IsNull)
            array = new NativeArray<T>(capacity);
        else
        {
            if (capacity < array.Size)
                throw new Exception("Can't bind lower capacity to a list");
            array = array.Realloc(capacity);
        }

        /*var arr = new NativeArray<T>(capacity);
        if (!array.IsNull)
        {
            if (capacity < array.Size)
                throw new Exception("Can't bind lower capacity to a list");
            ((Span<T>)array).CopyTo(arr);
            array.Dispose();
        }
        array = arr;*/
    }

    /// <summary>
    /// Insert a <paramref name="item"/> on this <see cref="NativeList{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Insert(int index, T item)
    {
        if(index == size)
        {
            Add(item);
            return;
        }

        if(index < 0 || index >= size)
            throw new ArgumentOutOfRangeException("index", index, "Can't insert after size or before 0");

        size++;
        checkBounds();

        using var oarr = array.Slice(index, size - (index + 1));

        /*for(int i = index + 1; i < capacity; i++)
        {
            array[i] = oarr[i - index];
        }*/
        for (int i = 0; i < oarr.Length; i++)
            array[(index + 1) + i] = oarr[i];
        array[index] = item;
    }

    /// <summary>
    /// Clear the items of this <see cref="NativeList{T}"/>
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < size; i++)
            array[i] = default;
        size = 0;
    }

    /// <summary>
    /// Check if this <see cref="NativeList{T}"/> contains <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Does this <see cref="NativeList{T}"/> contains <paramref name="item"/>?</returns>
    public bool Contains(T item)
    {
        int itemHash = item.GetHashCode();
        for (int i = 0; i < size; i++)
            if (array[i].GetHashCode() == itemHash)
                return true;
        return false;
    }

    /// <summary>
    /// Copy the contents of this <see cref="NativeList{T}"/> into <paramref name="array"/>, starting from <paramref name="arrayIndex"/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
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

    /// <summary>
    /// Turns this <see cref="NativeList{T}"/> readonly
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Get a string representation for this <see cref="NativeList{T}"/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string ret = $"NativeList<{typeof(T).Name}> of size: {size},\n{{ ";
        for (int i = 0; i < size; i++)
            ret += $"{i}: [{array[i]}] ";
        return ret + '}';
    }
}
