using System.Security.Cryptography;

namespace CoreApp
{
    public static class PasswordHelper
    {
        // Hash a password using PBKDF2
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía.\r\n", nameof(password));
            // Parameters
            int saltSize = 16; // 128 bit
            int keySize = 32;  // 256 bit
            int iterations = 100_000;

            // Generate salt
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            // Derive key
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(keySize);

            // Combine salt + key
            var hashBytes = new byte[saltSize + keySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, saltSize);
            Buffer.BlockCopy(key, 0, hashBytes, saltSize, keySize);

            // Return as base64 string
            return Convert.ToBase64String(hashBytes);
        }

        // Verifica si el password plano coincide con el hash almacenado
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            int saltSize = 16;
            int keySize = 32;
            int iterations = 100_000;
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[saltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, saltSize);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(keySize);
            for (int i = 0; i < keySize; i++)
            {
                if (hashBytes[i + saltSize] != key[i])
                    return false;
            }
            return true;
        }
    }
}
