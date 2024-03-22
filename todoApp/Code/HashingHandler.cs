using BCrypt.Net;
using System;
using System.Security.Cryptography;
using System.Text;

namespace todoApp.Code
{
    public class HashingHandler
    {
        private readonly HashAlgorithmName _hashingAlgo = new HashAlgorithmName("SHA256");

        // Sequentially hash the input text using different hashing algorithms
        public string AllIncludedHashing(string textToHash, string userId)
        {
            // Step 1: Hash using PBKDF2
            string pbkdf2Hash = PBKDF2_Hashing(textToHash, userId);

            // Step 2: Hash the result using HMAC SHA256
            string hmacHash = HMACHashing(pbkdf2Hash, userId);

            // Step 3: Hash the result using SHA256
            string sha256Hash = SHA256Hashing(hmacHash, userId);

            // Step 4: Finally, hash the result using BCrypt
            string bcryptHash = BCRYPT_Hashing(sha256Hash);

            return bcryptHash;
        }

        public bool CheckHashing(string textToCheck, string userId, string existingHash)
        {
            // Replicate the hashing process up to before BCrypt hashing
            // Step 1: Hash using PBKDF2
            string pbkdf2Hash = PBKDF2_Hashing(textToCheck, userId);

            // Step 2: Hash the result using HMAC SHA256
            string hmacHash = HMACHashing(pbkdf2Hash, userId);

            // Step 3: Hash the result using SHA256
            string sha256Hash = SHA256Hashing(hmacHash, userId);

            // Since BCrypt hashing includes the salt and can validate internally,
            // we directly use BCrypt's verification method to compare the final hashes.
            return BCRYPT_HashingValidation(sha256Hash, existingHash);
        }

        public string SHA256Hashing(string textToHash, string userId)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(textToHash);
                byte[] hash = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hash);
            }
        }

        public string HMACHashing(string textToHash, string userId)
        {
            byte[] myKey = Encoding.ASCII.GetBytes(userId);
            using (HMACSHA256 hmac = new HMACSHA256(myKey))
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(textToHash);
                byte[] hashedValue = hmac.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashedValue);
            }
        }

        public string PBKDF2_Hashing(string textToHash, string userId)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(textToHash);
            byte[] salt = Encoding.ASCII.GetBytes(userId);
            int iterations = 10000; // Use a more secure number of iterations
            int outputBits = 256;

            using (var pbkdf2 = new Rfc2898DeriveBytes(inputBytes, salt, iterations, _hashingAlgo))
            {
                byte[] hashedValue = pbkdf2.GetBytes(outputBits / 8);
                return Convert.ToBase64String(hashedValue);
            }
        }

        public string BCRYPT_Hashing(string textToHash)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            bool entropy = true;
            var hashType = HashType.SHA256;
            return BCrypt.Net.BCrypt.HashPassword(textToHash, salt, entropy, hashType);
        }

        public bool BCRYPT_HashingValidation(string textToHash, string hashedValue)
        {
            bool entropy = true;
            var hashType = HashType.SHA256;
            return BCrypt.Net.BCrypt.Verify(textToHash, hashedValue, entropy, hashType);
        }
    }
}
