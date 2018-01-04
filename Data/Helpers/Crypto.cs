using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Data.Helpers
{
    public static class Crypto
    {
        //TODO: Store in DB or appsettings.
        private static readonly byte[] CookieToken = Encoding.UTF8.GetBytes("1hssk29ash4ksah3lt29sj");

        // salt needs to be at least 22 characters
        public static string PasswordCrypt(string password, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                return "Exception|Salt is required!";
            }

            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public static string EncryptUserProfile(string profile)
        {
            var result = EncryptString(profile, Convert.ToBase64String(CookieToken));

            return result;
        }

        public static string DecryptUserProfile(string profile)
        {
            var result = DecryptString(profile, Convert.ToBase64String(CookieToken));
            return result;
        }

        private static string EncryptString(string value, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            string encryptedValue;

            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    using (var ems = new MemoryStream())
                    {
                        byte[] decryptedContent;
                        using (var ecs = new CryptoStream(ems, encryptor, CryptoStreamMode.Write))
                        using (var esw = new StreamWriter(ecs))
                        {
                            esw.Write(value);
                            esw.Flush();
                            ecs.FlushFinalBlock();
                            decryptedContent = ems.ToArray();
                        }

                        var iv = aes.IV;

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        encryptedValue = Convert.ToBase64String(result);
                    }
                }
            }

            return encryptedValue;
        }

        private static string DecryptString(string value, string keyString)
        {
            var fullCipher = Convert.FromBase64String(value);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var dms = new MemoryStream(cipher))
                    {
                        using (var dcs = new CryptoStream(dms, decryptor, CryptoStreamMode.Read))
                        using (var dsr = new StreamReader(dcs))
                        {
                            try
                            {
                                result = dsr.ReadToEnd();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                        }
                    }
                    // Remove the padding from the decrypted string
                    result = result.Split('\0')[0];
                    return result;
                }
            }
        }
    }
}