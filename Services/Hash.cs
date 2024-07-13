using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace ClinicWPF.Services
{
    public class Hash
    {
        public static string HashPassword(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            using (SHA256Managed sHA256ManagedString = new SHA256Managed())
            {
                byte[] hash = sHA256ManagedString.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
