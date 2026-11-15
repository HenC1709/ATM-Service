using System.Security.Cryptography;
using ATM2.Core.Interfaces.Security;

namespace ATM2.Infrastructure.Security
{
    /// <summary>
    /// Hash con PBKDF2 (SHA-256) + salt aleatorio. Formato guardado: "iteraciones.salt.hash" (Base64).
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public string Hash(string value)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(value, salt, Iterations, Algorithm, KeySize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string value, string hash)
        {
            string[] parts = hash.Split('.');
            if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations)) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expected = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual = Rfc2898DeriveBytes.Pbkdf2(value, salt, iterations, Algorithm, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }
}
