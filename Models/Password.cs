using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace University.Models
{
    public class Password
    {
        public static string GenerateSalt()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password,string salt)
        {
            using var sha = SHA256.Create();
            var combined = salt + password;

            var bytes = Encoding.UTF8.GetBytes(combined);
            var hashbytes = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hashbytes);
        }
    }
}
