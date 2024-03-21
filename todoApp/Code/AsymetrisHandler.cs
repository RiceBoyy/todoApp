using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System;
using System.Text;


namespace todoApp.Code
{
    public class AsymetrisHandler
    {
        // store private key, in files  or db
        // file create, file exist, file readAll.
        private string? _privateKey;
        public static string? _publicKey;

        public AsymetrisHandler()
        {
            
            using (RSA rsa = RSA.Create())
            {
                RSAParameters PrivateKeyParameter = rsa.ExportParameters(true);
                _privateKey = rsa.ToXmlString(true); // catifi is not use in json, but xml

                RSAParameters PublicKeyParameter = rsa.ExportParameters(true);
                _publicKey = rsa.ToXmlString(false);

            }
        }
        // ENCRYPT AS EARLY AS POSSIBLE
        //public string EncryptAsymetrisHandler(string textToEncrypt)
        //{
        //    return Encrypter.Encrypters(textToEncrypt, _publicKey);
        //}
        public static string EcryptAsymtrisk(string textToEncrypt, string publicKey)
        {
            using (RSACryptoServiceProvider rsaServiceProvider = new RSACryptoServiceProvider())
            {
                rsaServiceProvider.FromXmlString(publicKey);
                byte[] byteArrayTextToEncrypt = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                byte[] encryptedData = rsaServiceProvider.Encrypt(byteArrayTextToEncrypt, true);

                return Convert.ToBase64String(encryptedData);
            }
        }
        // DECRYPT AS EARLY AS POSSIBLE
        public string DecryptAsymtrisk(string textToDecrypt)
        {
            using (RSACryptoServiceProvider RSAserviceProvider = new RSACryptoServiceProvider())
            {
                RSAserviceProvider.FromXmlString(_privateKey);
                byte[] byteArrayTextToDecrypt = Convert.FromBase64String(textToDecrypt);
                byte[] DecryptedText = RSAserviceProvider.Decrypt(byteArrayTextToDecrypt, true);
                string decryptedDataAsString = System.Text.Encoding.UTF8.GetString(DecryptedText);
                return decryptedDataAsString;
            }
        }
    }

}
