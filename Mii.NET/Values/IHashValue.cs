namespace Mii.NET;

public interface IHashValue<T> : IHashValue
{
    /// <summary>
    /// Is this <typeparamref name="T"/> hash value equal to other?
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(T other);
}
public interface IHashValue
{
    public byte this[int index] { get; }
    /// <summary>
    /// The size of this has implementation
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Is this hash value equal to other?
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IHashValue other);

    /// <summary>
    /// Writes this hash value of size <see cref="Size"/> into a span of bytes
    /// </summary>
    /// <param name="bytes"></param>
    public void GetBytes(Span<byte> bytes);
}