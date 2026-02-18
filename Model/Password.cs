using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace part2_exersice.Model
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