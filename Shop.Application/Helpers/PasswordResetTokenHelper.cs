using System.Security.Cryptography;
using System.Text;

namespace Shop.Application.Helpers;

public static class PasswordResetTokenHelper
{
    public static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public static string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}