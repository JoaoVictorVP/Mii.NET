using IzaBlockchain.Net;
using Newtonsoft.Json;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Mii.NET;

[JsonConverter(typeof(Hash256Json_Converter))]
public unsafe struct Hash256 : IHashValue<Hash256>
{
    ulong A, B,  C, D;

    public byte this[int index]
    {
        get
        {
            Span<byte> bytes = stackalloc byte[Size];
            GetBytes(bytes);
            return bytes[index];
        }
    }
    public int Size => sizeof(ulong) * 4;
    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], A);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..16], B);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[16..24], C);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[24..], D);
    }

    public bool Equals(Hash256 other) => A == other.A && B == other.B && C == other.C && D == other.D;
    public bool Equals(IHashValue other) => Equals((Hash256)other);

    public static bool operator ==(Hash256 left, Hash256 right) => left.Equals(right);
    public static bool operator !=(Hash256 left, Hash256 right) => !(left == right);

    #region Public
    public static Hash256 From(Span<byte> data)
    {
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(data);
        return new Hash256(hash);
    }
    public static Hash256 From(Span<byte> data, Span<byte> key)
    {
        Span<byte> hash = stackalloc byte[32];
        HMACSHA256.HashData(key, data);
        return new Hash256(hash);
    }
    #endregion


    public static Hash256 Parse(string text)
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 4];
        MiiUtils.FromHexString(text, bytes);
        return new Hash256(bytes);
    }
    public override string ToString()
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 4];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], A);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..16], B);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[16..24], C);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[24..], D);

        return Convert.ToHexString(bytes);
    }

    public override int GetHashCode()
    {
        return (int) (A * B * C * D);
    }

    public Hash256(ReadOnlySpan<byte> bytes)
    {
        A = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);
        B = BinaryPrimitives.ReadUInt64LittleEndian(bytes[8..16]);
        C = BinaryPrimitives.ReadUInt64LittleEndian(bytes[16..24]);
        D = BinaryPrimitives.ReadUInt64LittleEndian(bytes[24..]);
    }
    public Hash256(byte* bytesPtr)
    {
        ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(bytesPtr, sizeof(ulong) * 4);

        A = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);
        B = BinaryPrimitives.ReadUInt64LittleEndian(bytes[8..16]);
        C = BinaryPrimitives.ReadUInt64LittleEndian(bytes[16..24]);
        D = BinaryPrimitives.ReadUInt64LittleEndian(bytes[24..]);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals((Hash256)obj);
    }
}
public class Hash256Json_Converter : JsonConverter<Hash256>
{
    public override Hash256 ReadJson(JsonReader reader, Type objectType, Hash256 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return Hash256.Parse(reader.ReadAsString());
    }

    public override void WriteJson(JsonWriter writer, Hash256 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}