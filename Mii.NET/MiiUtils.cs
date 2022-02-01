namespace IzaBlockchain.Net;

public static class MiiUtils
{
    /// <summary>
    /// Alloc a span, if it's size is bellow 1024 bytes then use stackalloc, if above then use a NativeArray'byte' using a smart pointer (GC)
    /// </summary>
    /// <param name="size">The size of span to alloc</param>
    /// <returns></returns>
    public static unsafe Span<byte> AllocSpan(int size)
    {
        if (size < 1024)
        {
            byte* span = stackalloc byte[size];
            return new Span<byte>(span, size);
        }
        else
            return new NativeArray<byte>(size).SmartClean();
    }
    public static bool FromHexString(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        if (chars.Length == 0)
            return false;
        if ((uint)chars.Length % 2 != 0)
            throw new FormatException("Invalid Hex Format");

        if (!HexUtils.TryDecodeFromUtf16(chars, bytes, out _))
            throw new FormatException("Invalid Hex Format");

        return true;
    }
}
