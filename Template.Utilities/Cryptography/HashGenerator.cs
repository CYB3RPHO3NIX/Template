using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Utilities.Cryptography
{
    public static class HashGenerator
    {
        public static string GenerateSHA256Hash(string input, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(input + salt);
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash).ToUpper();
            }
        }
    }
}
