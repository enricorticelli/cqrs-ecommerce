using System.Security.Cryptography;
using System.Text;

namespace Account.Infrastructure.Services;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string encoded)
    {
        var chunks = encoded.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (chunks.Length != 2)
        {
            return false;
        }

        try
        {
            var salt = Convert.FromBase64String(chunks[0]);
            var expected = Convert.FromBase64String(chunks[1]);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, expected.Length);
            return CryptographicOperations.FixedTimeEquals(hash, expected);
        }
        catch
        {
            return false;
        }
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
