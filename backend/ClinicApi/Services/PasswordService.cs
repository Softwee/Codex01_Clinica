using System.Security.Cryptography;
using System.Text;

namespace ClinicApi.Services;

public static class PasswordService
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var providedHash = HashPassword(providedPassword);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(hashedPassword),
            Convert.FromHexString(providedHash));
    }
}
