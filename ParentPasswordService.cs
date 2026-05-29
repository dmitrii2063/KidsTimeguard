using System.Security.Cryptography;

namespace KidsComputerTimeGuard;

public static class ParentPasswordService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 150_000;

    public static (string HashBase64, string SaltBase64) CreateHash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Derive(password, salt);
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static void SetPassword(AppSettings settings, string password)
    {
        var (hash, salt) = CreateHash(password);
        settings.ParentPasswordHash = hash;
        settings.ParentPasswordSalt = salt;
    }

    public static bool Verify(string password, AppSettings settings)
    {
        if (string.IsNullOrEmpty(settings.ParentPasswordHash)
            || string.IsNullOrEmpty(settings.ParentPasswordSalt))
            return false;

        try
        {
            var salt = Convert.FromBase64String(settings.ParentPasswordSalt);
            var expected = Convert.FromBase64String(settings.ParentPasswordHash);
            var actual = Derive(password, salt);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsConfigured(AppSettings settings) =>
        !string.IsNullOrEmpty(settings.ParentPasswordHash)
        && !string.IsNullOrEmpty(settings.ParentPasswordSalt);

    private static byte[] Derive(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
}
