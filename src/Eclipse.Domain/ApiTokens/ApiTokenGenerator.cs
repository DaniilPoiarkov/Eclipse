using System.Security.Cryptography;
using System.Text;

namespace Eclipse.Domain.ApiTokens;

public static class ApiTokenGenerator
{
    private const string Prefix = "eclp_";

    internal static string Generate()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Prefix + Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string Hash(string plaintext)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintext));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    internal static string Mask(string plaintext)
    {
        var body = plaintext[Prefix.Length..];
        return $"{Prefix}{body[..5]}...{plaintext[^5..]}";
    }
}
