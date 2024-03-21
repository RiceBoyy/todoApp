using System.Security.Cryptography;
using System.Text;

namespace todoApp.Code;
public class Encrypter
{
    public static string Encrypters(string textToEncrypt, string publicKey)
    {
        using (RSACryptoServiceProvider rsaServiceProvider = new RSACryptoServiceProvider())
        {
            rsaServiceProvider.FromXmlString(publicKey);
            byte[] byteArrayTextToEncrypt = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
            byte[] encryptedData = rsaServiceProvider.Encrypt(byteArrayTextToEncrypt, true);

            return Convert.ToBase64String(encryptedData);
        }
    }
}
