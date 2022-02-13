namespace IzaBlockchain.Net;

public struct NativeString : IDisposable
{
    readonly NativeArray<char> ptr;
    bool readOnly = false;
    /// <summary>
    /// The size of this <see cref="NativeString"/>
    /// </summary>
    public int Length => ptr.Size;
    public char this[int index]
    {
        get => ptr[index];
        set
        {
            if (readOnly)
                return;
            ptr.Ref(index) = value;
        }
    }

    /// <summary>
    /// Retrieve this <see cref="NativeString"/> as readonly
    /// </summary>
    /// <returns></returns>
    public NativeString AsReadOnly() => this with { readOnly = true };

    public NativeString(Span<char> span)
    {
        ptr = new NativeArray<char>(span);
    }
    public NativeString(string hString)
    {
        ptr = new NativeArray<char>(hString.AsSpan());
    }

    public static bool operator ==(NativeString a, NativeString b) => a.Equals(b);
    public static bool operator !=(NativeString a, NativeString b) => !(a == b);

    public static implicit operator NativeString (string from) => new NativeString(from);
    public static explicit operator string (NativeString from) => from.ToString();
    public static implicit operator Span<char> (NativeString from) => from.ptr;

    /// <summary>
    /// Returns a clone for this <see cref="NativeString"/>
    /// </summary>
    /// <returns></returns>
    public NativeString Clone() => new NativeString(this);

    /// <summary>
    /// Releases native resources used by this <see cref="NativeString"/>
    /// </summary>
    public void Dispose() => ptr.Dispose();

    /// <summary>
    /// Check <paramref name="other"/> reference is equal to this <see cref="NativeString"/> reference
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public unsafe bool ReferenceEquals(NativeString other) => ptr.Ptr == other.ptr.Ptr;

    /// <summary>
    /// Check one <see cref="NativeString"/> match to another
    /// </summary>
    /// <param name="other">The other <see cref="NativeString"/> to compare</param>
    /// <returns></returns>
    public bool Equals(NativeString other)
    {
        if (ptr.Size != other.ptr.Size) return false;
        for (int i = 0; i < ptr.Size; i++)
            if (ptr[i] != other.ptr[i])
                return false;
        return true;
    }
    /// <summary>
    /// Gets the hash code of this <see cref="NativeString"/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int hash = 0;
        for (int i = 0; i < ptr.Size; i++)
            hash += ptr[i];
        return hash;
    }
    /// <summary>
    /// Gets a heap version of this string as output
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string f = "";
        for (int i = 0; i < ptr.Size; i++)
            f += ptr[i];
        return f;
    }
}