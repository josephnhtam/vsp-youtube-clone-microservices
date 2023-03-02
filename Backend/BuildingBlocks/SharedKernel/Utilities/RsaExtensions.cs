using System.Security.Cryptography;
using System.Text;

namespace SharedKernel.Utilities {
    public static class RsaExtensions {

        public static string EncryptToBase64String (this RSA publicKey, string data, RSAEncryptionPadding? padding = null) {
            return Convert.ToBase64String(publicKey.Encrypt(Encoding.UTF8.GetBytes(data), padding ?? RSAEncryptionPadding.Pkcs1));
        }

        public static string DescryptFromBase64String (this RSA privateKey, string encrypted, RSAEncryptionPadding? padding = null) {
            return Encoding.UTF8.GetString(privateKey.Decrypt(Convert.FromBase64String(encrypted), padding ?? RSAEncryptionPadding.Pkcs1));
        }

    }
}
