using IzaBlockchain.Net;
using Newtonsoft.Json;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Mii.NET;

[JsonConverter(typeof(Hash128Json_Converter))]
public struct Hash128 : IHashValue<Hash128>
{
    public byte this[int index]
    {
        get
        {
            Span<byte> bytes = stackalloc byte[sizeof(ulong) * 2];
            GetBytes(bytes);
            return bytes[index];
        }
    }
    public int Size => 16;
    ulong A, B;
    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], A);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..], B);
    }

    public bool Equals(Hash128 other) => A == other.A && B == other.B;

    public bool Equals(IHashValue other) => Equals((Hash128)other);


    public static bool operator ==(Hash128 left, Hash128 right) => left.Equals(right);
    public static bool operator !=(Hash128 left, Hash128 right) => !(left == right);

    #region Public
    public static Hash128 From(Span<byte> data)
    {
        Span<byte> hash = stackalloc byte[16];
        SHA1.HashData(data);
        return new Hash128(hash);
    }
    public static Hash128 From(Span<byte> data, Span<byte> key)
    {
        Span<byte> hash = stackalloc byte[16];
        HMACSHA1.HashData(key, data);
        return new Hash128(hash);
    }
    #endregion

    public static Hash128 Parse(string text)
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 2];
        MiiUtils.FromHexString(text, bytes);
        return new Hash128(bytes);
    }
    public override string ToString()
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 2];
        GetBytes(bytes);

        return Convert.ToHexString(bytes);
    }

    public override int GetHashCode()
    {
        return (int) (A * B);
    }

    public Hash128(ReadOnlySpan<byte> bytes)
    {
        A = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);
        B = BinaryPrimitives.ReadUInt64LittleEndian(bytes[8..16]);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals((Hash128)obj);
    }
}
public class Hash128Json_Converter : JsonConverter<Hash128>
{
    public override Hash128 ReadJson(JsonReader reader, Type objectType, Hash128 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return Hash128.Parse(reader.ReadAsString());
    }

    public override void WriteJson(JsonWriter writer, Hash128 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
