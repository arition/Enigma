using System;
using System.Security.Cryptography;

namespace EnigmaLib.Model
{
    public class SignedData:ICloneable
    {
        public byte[] Content { get; set; }
        public RSAParameters PublicKey { get; set; }
        public byte[] SHA256Hash { get; set; }
        public byte[] Signature { get; set; }

        public static SignedData GenerateSignedData(byte[] content, RSAParameters publicKey, RSAParameters privateKey)
        {
            var result = new SignedData
            {
                Content = content,
                PublicKey = publicKey
            };

            result.GenerateSignedData(privateKey);
            return result;
        }

        public void GenerateSignedData(RSAParameters privateKey)
        {
            using (var sha256 = SHA256.Create())
            {
                SHA256Hash = sha256.ComputeHash(Content);
            }

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                Signature = rsa.SignHash(SHA256Hash, HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}