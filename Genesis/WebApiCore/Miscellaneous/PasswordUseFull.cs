using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCore.Miscellaneous
{
    public class PasswordUseFull
    {
        private static readonly Random random = new Random();

        public static string GetHash(string plainText, byte[] saltBytes = null)
        {
            if (saltBytes == null)
            {
                int minSaltSize = 4;
                int maxSaltSize = 8;

                
                int saltSize = random.Next(minSaltSize, maxSaltSize);
                saltBytes = new byte[saltSize];

                
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                
                rng.GetNonZeroBytes(saltBytes);
            }

            
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            
            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            
            for (int i = 0; i < plainTextBytes.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextBytes[i];
            }

            
            for (int i = 0; i < saltBytes.Length; i++)
            {
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];
            }

            
            HashAlgorithm hash = new SHA512Managed();

            
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashWithSaltBytes[i] = hashBytes[i];
            }

            
            for (int i = 0; i < saltBytes.Length; i++)
            {
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];
            }

            
            return Convert.ToBase64String(hashWithSaltBytes);
        }

        public static bool ValidatePass(string pass, string hash)
        {
            
            byte[] hashWithSaltBytes = Convert.FromBase64String(hash);

            
            int hashSizeInBits, hashSizeInBytes;
            hashSizeInBits = 512;

            
            hashSizeInBytes = hashSizeInBits / 8;

            
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            
            byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

            
            for (int i = 0; i < saltBytes.Length; i++)
            {
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
            }

            
            string expectedHashString = GetHash(pass, saltBytes);

            
            return (hash == expectedHashString);
        }
    }
}
