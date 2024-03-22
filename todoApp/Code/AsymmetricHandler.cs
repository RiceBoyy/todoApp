using System.Security.Cryptography;
using System;
using System.Text;
using System.IO;

namespace todoApp.Code
{
    public class AsymmetricHandler
    {
        private string privateKey;
        public string PublicKey;
        private readonly string privateKeyFilePath;
        private readonly string publicKeyFilePath;

        public AsymmetricHandler()
        {
            string projectRootPath = AppDomain.CurrentDomain.BaseDirectory;
            string keyDirectoryPath = Path.Combine(projectRootPath, ".aspnet", "https");
            Directory.CreateDirectory(keyDirectoryPath);

            // Construct the file paths for keys
            privateKeyFilePath = Path.Combine(keyDirectoryPath, "privateKey.xml");
            publicKeyFilePath = Path.Combine(keyDirectoryPath, "publicKey.xml");

            if (File.Exists(privateKeyFilePath) && File.Exists(publicKeyFilePath))
            {
                // Read existing keys from the files
                privateKey = File.ReadAllText(privateKeyFilePath);
                PublicKey = File.ReadAllText(publicKeyFilePath);
            }
            else
            {
                // Generate new keys and save to files
                using (RSA rsa = RSA.Create())
                {
                    privateKey = rsa.ToXmlString(true); // Include private parameters
                    PublicKey = rsa.ToXmlString(false); // Exclude private parameters

                    // Save the keys to their respective files
                    File.WriteAllText(privateKeyFilePath, privateKey);
                    File.WriteAllText(publicKeyFilePath, PublicKey);
                }
            }
        }

        public string DecryptAsymtrisk(string textToDecrypt)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateKey);
                byte[] byteArrayTextToDecrypt = Convert.FromBase64String(textToDecrypt);
                byte[] decryptedText = rsa.Decrypt(byteArrayTextToDecrypt, RSAEncryptionPadding.OaepSHA256);
                string decryptedDataAsString = Encoding.UTF8.GetString(decryptedText);
                return decryptedDataAsString;
            }
        }

        public string EncryptAsymtrisk(string textToEncrypt)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(PublicKey);
                byte[] byteArrayTextToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] encryptedData = rsa.Encrypt(byteArrayTextToEncrypt, RSAEncryptionPadding.OaepSHA256);

                return Convert.ToBase64String(encryptedData);
            }
        }
    }
}
