using IzaBlockchain.Net;
using Newtonsoft.Json;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Mii.NET;

[JsonConverter(typeof(Hash512Json_Converter))]
public unsafe struct Hash512 : IHashValue<Hash512>
{
    ulong A, B,  C, D,  E, F, G, H;

    public byte this[int index]
    {
        get
        {
            Span<byte> bytes = stackalloc byte[Size];
            GetBytes(bytes);
            return bytes[index];
        }
    }
    public int Size => sizeof(ulong) * 8;
    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], A);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..16], B);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[16..24], C);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[24..32], D);

        BinaryPrimitives.WriteUInt64LittleEndian(bytes[32..40], E);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[40..48], F);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[48..56], G);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[56..], H);
    }

    public bool Equals(Hash512 other) => A == other.A && B == other.B && C == other.C && D == other.D && E == other.E && F == other.F && G == other.G && H == other.H;

    public bool Equals(IHashValue other) => Equals((Hash512)other);

    public static bool operator ==(Hash512 left, Hash512 right) => left.Equals(right);
    public static bool operator !=(Hash512 left, Hash512 right) => !(left == right);

    #region Public
    public static Hash512 From(Span<byte> data)
    {
        Span<byte> hash = stackalloc byte[32];
        SHA512.HashData(data);
        return new Hash512(hash);
    }
    public static Hash512 From(Span<byte> data, Span<byte> key)
    {
        Span<byte> hash = stackalloc byte[32];
        HMACSHA512.HashData(key, data);
        return new Hash512(hash);
    }
    #endregion


    public static Hash512 Parse(string text)
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 8];
        MiiUtils.FromHexString(text, bytes);
        return new Hash512(bytes);
    }
    public override string ToString()
    {
        Span<byte> bytes = stackalloc byte[sizeof(ulong) * 8];
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], A);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..16], B);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[16..24], C);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[24..32], D);

        BinaryPrimitives.WriteUInt64LittleEndian(bytes[32..40], E);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[40..48], F);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[48..56], G);
        BinaryPrimitives.WriteUInt64LittleEndian(bytes[56..], H);

        return Convert.ToHexString(bytes);
    }

    public override int GetHashCode()
    {
        return (int) (A * B * C * D * E * F * G * H);
    }

    public Hash512(ReadOnlySpan<byte> bytes)
    {
        A = BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]);
        B = BinaryPrimitives.ReadUInt64LittleEndian(bytes[8..16]);

        C = BinaryPrimitives.ReadUInt64LittleEndian(bytes[16..24]);
        D = BinaryPrimitives.ReadUInt64LittleEndian(bytes[24..32]);

        E = BinaryPrimitives.ReadUInt64LittleEndian(bytes[32..40]);
        F = BinaryPrimitives.ReadUInt64LittleEndian(bytes[40..48]);
        G = BinaryPrimitives.ReadUInt64LittleEndian(bytes[48..56]);
        H = BinaryPrimitives.ReadUInt64LittleEndian(bytes[56..]);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return Equals((Hash512)obj);
    }
}
public class Hash512Json_Converter : JsonConverter<Hash512>
{
    public override Hash512 ReadJson(JsonReader reader, Type objectType, Hash512 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return Hash512.Parse(reader.ReadAsString());
    }

    public override void WriteJson(JsonWriter writer, Hash512 value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}