using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;

namespace todoApp.Code
{
    public class HashingHandler
    {
        private readonly HashAlgorithmName _hashingAlgo = new HashAlgorithmName("SHA256");

        // DONT USE THIS TO CPR
        // if make a check in home to check if returntype is a string.
        public string MBDHashing(string TextToHash, string returnType)
        {
            var convertHash = "";
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(TextToHash);
            byte[] hash = md5.ComputeHash(inputBytes);
            
            if (returnType == "string")
            {
                convertHash = Convert.ToBase64String(hash);
            }

            return convertHash;
        }

        // DONT USE SHA1
        public string SHA256Hashing(string TextToHash)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(TextToHash);
            byte[] hash = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(hash);
        }

        // FOR MESSAGE
        public string HMACHashing(string TextToHash)
        {
            byte[] myKey = Encoding.ASCII.GetBytes("NielseErFavoritLære");
            byte[] inputBytes = Encoding.ASCII.GetBytes(TextToHash);
            
            // NOT Static
            HMACSHA256 hmac = new HMACSHA256();

            // the key is currently hardcoded
            hmac.Key = myKey;
            byte[] hashedValue = hmac.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashedValue);
        }

        // DO THIS IN CPR
        public string PBKDF2_Hashing(string TextToHash, string id)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(TextToHash);
            byte[] salt = Encoding.ASCII.GetBytes(id); // Use the 'id' as salt

            // Use the predefined hash algorithm name
            int iterations = 10;
            int outputBits = 256; // Assuming you want a 256-bit output, adjust as needed

            // Using Rfc2898DeriveBytes for PBKDF2 hashing
            using (var pbkdf2 = new Rfc2898DeriveBytes(inputBytes, salt, iterations, _hashingAlgo))
            {
                byte[] hashedValue = pbkdf2.GetBytes(outputBits / 8); // Dividing by 8 to convert bits to bytes
                return Convert.ToBase64String(hashedValue);
            }
        }

        // INSTALL BCrypt.Net-Next NuGet Package
        public string BCRYPT_Hashing(string TextToHash)
        {
            // example one
            // return BCrypt.Net.BCrypt.HashPassword(TextToHash);

            // example two
            //int workFactor = 10;
            //bool entropy = true;
            //return BCrypt.Net.BCrypt.HashPassword(TextToHash, workFactor, entropy);

            // example three
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            bool entropy = true;
            var hashType = HashType.SHA256;
            return BCrypt.Net.BCrypt.HashPassword(TextToHash, salt, entropy, hashType);
        }
        public bool BCRYPT_HashingValidation(string TextToHash,string HashedValue)
        {
            // example one
            // return BCrypt.Net.BCrypt.Verify(TextToHash, HashedValue);

            // example two
            //bool entropy = true;
            //return BCrypt.Net.BCrypt.Verify(TextToHash, HashedValue, entropy);

            // example three
            bool entropy = true;
            var hashType = HashType.SHA256;
            return BCrypt.Net.BCrypt.Verify(TextToHash, HashedValue, entropy, hashType);
        }
    }
}
