using System;
using System.Security.Cryptography;
using System.Text;

namespace USR_ElectroPilot.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(bytes.Length * 2);

                foreach (var value in bytes)
                {
                    builder.Append(value.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }

            return string.Equals(HashPassword(password), passwordHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
