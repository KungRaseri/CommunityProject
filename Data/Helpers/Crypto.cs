using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Data.Helpers
{
    public class Crypto
    {
        private readonly byte[] CookieToken;

        public Crypto(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Must provide a token");
            }

            CookieToken = Encoding.UTF8.GetBytes(token);
        }

        // salt needs to be at least 22 characters
        public string PasswordCrypt(string password, string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentNullException(nameof(salt), "Must provide a salt");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public string EncryptUserProfile(string profile)
        {
            var result = EncryptString(profile, Convert.ToBase64String(CookieToken));

            return result;
        }

        public string DecryptUserProfile(string profile)
        {
            var result = DecryptString(profile, Convert.ToBase64String(CookieToken));
            return result;
        }

        private string EncryptString(string value, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            string encryptedValue;

            using (var aes = Aes.Create())
            {
                aes.Padding = PaddingMode.Zeros;
                using (var encryptor = aes.CreateEncryptor(key, aes.IV))
                {
                    CryptoStream ecs = null;

                    using (var ems = new MemoryStream())
                    {
                        byte[] decryptedContent = { };
                        try
                        {
                            ecs = new CryptoStream(ems, encryptor, CryptoStreamMode.Write);
                            using (var esw = new StreamWriter(ecs))
                            {
                                esw.Write(value);
                                esw.Flush();
                                ecs.FlushFinalBlock();
                                decryptedContent = ems.ToArray();
                            }

                        }
                        catch (Exception e)
                        {
                            ecs?.Dispose();
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

        private string DecryptString(string value, string keyString)
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
                    string result = string.Empty;
                    CryptoStream dcs = null;
                    using (var dms = new MemoryStream(cipher))
                    {
                        try
                        {
                            dcs = new CryptoStream(dms, decryptor, CryptoStreamMode.Read);
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
                        catch (Exception e)
                        {
                            dcs?.Dispose();
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