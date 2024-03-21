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
        public string? _publicKey;

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
        public string EncryptAsymetrisHandler(string textToEncrypt)
        {
            return Encrypter.Encrypters(textToEncrypt, _publicKey);
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
