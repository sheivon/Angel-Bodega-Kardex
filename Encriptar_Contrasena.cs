
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace kardex_Web
{
    public class Encriptar_Contrasena
    {

        private static readonly string EncryptionKey = "MAKV2SPBNI99212";

        private static readonly byte[] Salt = new byte[]
        {
        0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D,
        0x65, 0x64, 0x76, 0x65, 0x64, 0x65,
        0x76
        };
        
// Disable the warning.
#pragma warning disable SYSLIB0060
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes aes = Aes.Create())
            {
                using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt, 1000, HashAlgorithmName.SHA1))
                {
                    aes.Key = pdb.GetBytes(32);
                    aes.IV = pdb.GetBytes(16);
                }

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
// Disable the warning.
#pragma warning disable SYSLIB0060
        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt, 1000, HashAlgorithmName.SHA1))
                {
                    aes.Key = pdb.GetBytes(32);
                    aes.IV = pdb.GetBytes(16);
                }

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }

                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
    }
}

