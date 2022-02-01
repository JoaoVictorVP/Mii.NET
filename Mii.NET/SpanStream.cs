using System.Buffers.Binary;

namespace IzaBlockchain.Net;

/// <summary>
/// A stack living span that will act like a stream, this means you can do read and writting operations iteratively without need to care about size and other things, extensible via SpanStack internal use (see implementation)
/// </summary>
public ref struct SpanStream
{
    public Span<byte> AsSpan() => stack.Span;
    SpanStack<byte> stack;
    bool reading;

    public bool CanWrite => !reading && stack.Size > 0;
    public bool CanRead => reading && stack.Size > 0;

    /// <summary>
    /// Write's an individual byte into stream
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public SpanStream WriteByte(byte value)
    {
        if (!CanWrite)
            return this;
        stack.Push(value);

        return this;
    }

    #region Writting Specifics
    /// <summary>
    /// Shorthand for writting numbers (int32) on SpanStream
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public unsafe SpanStream WriteInt32(int number)
    {
        byte* num = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(new Span<byte>(num, sizeof(int)), number);
        stack.PushAll(num, sizeof(int));

        return this;
    }
    #endregion

    /// <summary>
    /// Writes buffer (pointer) bytes into stream of the specified length
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public unsafe SpanStream Write(byte* buffer, int length)
    {
        if (!CanWrite)
            return this;
        stack.PushAll(buffer, length);

        return this;
    }
    /// <summary>
    /// Writes buffer (span) with a fixed internal length into this stream
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public SpanStream Write(Span<byte> buffer)
    {
        if (!CanWrite)
            return this;
        stack.PushAll(buffer);

        return this;
    }

    /// <summary>
    /// Reads one byte of this stream
    /// </summary>
    /// <returns></returns>
    public byte ReadByte()
    {
        if (!CanRead)
            return 0;
        return stack.Pop();
    }
    /// <summary>
    /// Reads a buffer of fixed size from this stream (caution with sizes)
    /// </summary>
    /// <param name="buffer"></param>
    public void Read(Span<byte> buffer)
    {
        for (int i = 0; i < stack.Size; i++)
            buffer[i] = stack.Pop();
    }
    /// <summary>
    /// Reads a specified count of bytes from stream and returns an allocated span
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    public unsafe Span<byte> ReadTo(int to)
    {
        Span<byte> buffer = MiiUtils.AllocSpan(to);

        for (int i = 0; i < to; i++)
            buffer[i] = stack.Pop();

        return buffer;
    }

    /// <summary>
    /// Releases the native allocations (if any) from memory
    /// </summary>
    public void Dispose()
    {
        stack.Dispose();
    }

    public SpanStream(Span<byte> span, bool reading = true)
    {
        //Span = span;
        //current = 0;
        //remaining = span.Length;
        stack = new SpanStack<byte>();
        stack.Initialize(span);

        this.reading = reading;
    }
    public SpanStream(int capacity)
    {
        stack = new SpanStack<byte>();
        stack.Initialize(capacity);

        this.reading = false;
    }
    public SpanStream()
    {
        stack = new SpanStack<byte>();
        stack.Initialize(32);

        this.reading = false;
    }
}
