using System.Globalization;
using System.Runtime.CompilerServices;

namespace IzaBlockchain.Net;

/// <summary>
/// A <see cref="NativeString"/> to represent a non-managed alternative to .NET default <see cref="string"/>
/// </summary>
public struct NativeString : IDisposable
{
    /// <summary>
    /// Returns the internal <see cref="NativeArray{char}"/> for this <see cref="NativeString"/>
    /// </summary>
    /// <returns></returns>
    public NativeArray<char> GetArray() => ptr;

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
    NativeString(NativeArray<char> ptr)
    {
        this.ptr = ptr;
    }

    #region PUBLIC

    static CultureInfo culture = CultureInfo.CurrentCulture;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeString ToUpper()
    {
        //var rPtr = ptr.Ptr;
        //var culture = CultureInfo.CurrentCulture;
        var culture = NativeString.culture;
        int size = ptr.Size;
        for (int i = 0; i < size; i++)
        {
            ref char c = ref ptr.Ref(i);
            c = char.ToUpper(c, culture);
            //ptr.Ref(i) = char.ToUpper(ptr[i]);
            //rPtr[i] = char.ToUpper(rPtr[i], culture);
        }
        return this;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeString ToLower()
    {
        //var rPtr = ptr.Ptr;
        //var culture = CultureInfo.CurrentCulture;
        var culture = NativeString.culture;
        int size = ptr.Size;
        for (int i = 0; i < size; i++)
        {
            ref char c = ref ptr.Ref(i);
            c = char.ToLower(c, culture);
            //ptr.Ref(i) = char.ToUpper(ptr[i]);
            //rPtr[i] = char.ToUpper(rPtr[i], culture);
        }
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public NativeString Trim()
    {
        int countTrim = 0;
        int beginIndexForFirst = -1;
        int virtCountTrim = 0;
        int beginIndexForLast = -1;
        bool begunChars = false;
        for (int i = 0; i < ptr.Size; i++)
        {
            if(!begunChars)
            {
                if (char.IsWhiteSpace(ptr[i]))
                {
                    countTrim++;
                    continue;
                }
                else
                {
                    beginIndexForFirst = i;
                    begunChars = true;
                }
            }
            if (char.IsWhiteSpace(ptr[i]))
            {
                if (virtCountTrim == 0)
                    beginIndexForLast = i;
                virtCountTrim++;
            }
            else
            {
                beginIndexForLast = -1;
                virtCountTrim = 0;
            }
        }

        if (beginIndexForFirst == 0 && beginIndexForLast < 0)
            return this;

        countTrim += virtCountTrim;

        if (beginIndexForLast < 0) beginIndexForLast = ptr.Size;

        int size = ptr.Size - countTrim;
        var nptr = new NativeArray<char>(size);
        int setIndex = 0;
        for (int i = beginIndexForFirst; i < beginIndexForLast; i++)
        {
            nptr[setIndex] = ptr[i];
            setIndex++;
        }
        ptr.Dispose();

        return new NativeString(nptr);
    }

    #endregion


    public static bool operator ==(NativeString a, NativeString b) => a.Equals(b);
    public static bool operator !=(NativeString a, NativeString b) => !(a == b);

    public static NativeString operator +(NativeString a, NativeString b)
    {
        int aSize = a.ptr.Size;
        int bSize = b.ptr.Size;
        int size = aSize + bSize;

        var nptr = a.ptr.Realloc(size);

        for (int i = 0; i < bSize; i++)
            nptr[i + aSize] = b.ptr[i];

        return new NativeString(nptr);
    }

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