using System.Security.Cryptography;
using System.Text;
using WPLoggingLibrary;

namespace Helfer
{
    public static class StringEncryptor
    {
        private const int IvLength = 16;
        private static readonly Encoding Utf8 = new UTF8Encoding(false);
        private static readonly FileLogger _logger = FileLogger.Instance;
        public static string EncryptString(string plainText, string key)
        {
            try
            {
                if (plainText == null)
                {
                    _logger.Log(LogLevel.Warning, $"Kein Text angegeben in EncryptString(plainText, key)");
                    return string.Empty;
                }
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.Log(LogLevel.Warning, $"Kein Key angegeben in EncryptString(plainText, key)");
                    return string.Empty;
                }

                byte[] iv;
                byte[] cipherBytes;

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Utf8.GetBytes(key);
                    aesAlg.GenerateIV();
                    iv = aesAlg.IV;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);

                    using MemoryStream msEncrypt = new();
                    msEncrypt.Write(iv, 0, iv.Length);
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Utf8.GetBytes(plainText);
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    cipherBytes = msEncrypt.ToArray();
                }

                return Convert.ToBase64String(cipherBytes);
            }catch(Exception ex)
            {
                _logger.Log(LogLevel.Warning, $"Verschlüßelung fehlgeschlagen! {ex.Message}");
                return string.Empty;
            }
        }

        public static string DecryptString(string cipherText, string key)
        {
            try
            {
                if (cipherText == null)
                {
                    _logger.Log(LogLevel.Warning, $"Kein Text angegeben in DecryptString(cipherText, key)");
                    return string.Empty;
                }
                if (string.IsNullOrWhiteSpace(key))
                {
                    _logger.Log(LogLevel.Warning, $"Kein Key angegeben in DecryptString(cipherText, key)");
                    return string.Empty;
                }

                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] iv = new byte[IvLength];
                Array.Copy(cipherBytes, iv, IvLength);

                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Utf8.GetBytes(key);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new(cipherBytes, IvLength, cipherBytes.Length - IvLength);
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt, Utf8);
                return srDecrypt.ReadToEnd();
            }catch(Exception ex)
            {
                _logger.Log(LogLevel.Warning, $"Entschlüßelung fehlgeschlagen! {ex.Message}");
                return string.Empty;
            }
        }

        public static string GenerateKey()
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.GenerateKey();
                return Convert.ToBase64String(aesAlg.Key);
            }catch(Exception ex)
            {
                _logger.Log(LogLevel.Warning, $"Schlüßel erstellen fehlgeschlagen! {ex.Message}");
                return string.Empty;
            }
        }

        public static string GenerateIV()
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.GenerateIV();
                return Convert.ToBase64String(aesAlg.IV);
            }catch(Exception ex) {
                _logger.Log(LogLevel.Warning, $"IV erstellen fehlgeschlagen! {ex.Message}");
                return string.Empty;
            }
        }
    }
}
