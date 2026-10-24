using System.Security.Cryptography;

namespace HistoriaClinica.Core.Security;

public sealed record PasswordHashResult(string Hash, string Salt, int Iterations);

public static class PasswordHasher
{
    public const int Iterations = 210_000;
    public const int SaltSize = 16;
    public const int HashSize = 32;

    public static PasswordHashResult HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return new PasswordHashResult(
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt),
            Iterations);
    }

    public static bool VerifyPassword(string password, string hash, string salt, int iterations)
    {
        if (string.IsNullOrEmpty(password)
            || string.IsNullOrWhiteSpace(hash)
            || string.IsNullOrWhiteSpace(salt)
            || iterations <= 0)
        {
            return false;
        }

        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            var saltBytes = Convert.FromBase64String(salt);
            var candidate = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256,
                hashBytes.Length);

            return CryptographicOperations.FixedTimeEquals(candidate, hashBytes);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
