using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Template.Utilities.Cryptography
{
    public static class PasswordSaltGenerator
    {
        public static string GenerateSalt(int size = 16)
        {
            var randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).ToUpper();
        }
    }
}
