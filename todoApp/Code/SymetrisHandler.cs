using Microsoft.AspNetCore.DataProtection;

namespace todoApp.Code
{
    public class SymetrisHandler
    {
        private readonly IDataProtector _dataProtector;

        public SymetrisHandler(IDataProtectionProvider dataProtectionProvider)
        {
            // THIS SHOULD BE THE KEY
            _dataProtector = dataProtectionProvider.CreateProtector("SomeCoolKey");
        }

        public string Encrypt(string textToEncrypt) =>
            _dataProtector.Protect(textToEncrypt);

        public string Decrypt(string textToDecrypt) =>
            _dataProtector.Unprotect(textToDecrypt);
    }
}
