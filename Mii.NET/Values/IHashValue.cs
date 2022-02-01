namespace Mii.NET;

public interface IHashValue<T> : IHashValue
{
    public bool Equals(T other);
}
public interface IHashValue
{
    public byte this[int index] { get; }
    public int Size { get; }
    public bool Equals(IHashValue other);

    public void GetBytes(Span<byte> bytes);
}