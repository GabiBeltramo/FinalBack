namespace GestionDeGastos.Models
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class CryptoHelper
    {
        private static readonly byte[] Key = Encoding.ASCII.GetBytes("UnaClaveSecreta"); // Clave secreta compartida

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                byte[] iv = aes.IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(Key, iv), CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plainText);
                        }
                    }

                    byte[] encryptedBytes = memoryStream.ToArray();

                    byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(combinedBytes);
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] combinedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                byte[] iv = new byte[aes.IV.Length];
                byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];

                Buffer.BlockCopy(combinedBytes, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(Key, iv), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
