using System;
using System.Security.Cryptography;
using System.Text;

namespace AtendTeleMedicina.Domain.Services.Helpers
{
    public static class CryptoHashService
    {
        public static string GenerateSha1Hash(this string text, Encoding encoder = null)
        {
            var buffer = encoder?.GetBytes(text) ?? Encoding.Default.GetBytes(text);
            var cryptoTransformSha1 = new SHA1CryptoServiceProvider();
            var hash = BitConverter.ToString(cryptoTransformSha1.ComputeHash(buffer)).Replace("-", "");
            return hash.ToLower();
        }
    }
}
