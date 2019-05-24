using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaLib
{
    public class EncryptHelper : IDisposable
    {
        public EncryptHelper(RSAParameters rsaParameters)
        {
            RSA.ImportParameters(rsaParameters);
            AES.KeySize = 256;
            AES.Mode = CipherMode.CBC;
        }

        private RSA RSA { get; } = RSA.Create();
        private Aes AES { get; } = Aes.Create();

        public void Dispose()
        {
            RSA?.Dispose();
            AES?.Dispose();
        }

        public async Task<EncryptedData> Encrypt(string str)
        {
            return await Encrypt(Encoding.UTF8.GetBytes(str));
        }

        public async Task<EncryptedData> Encrypt(byte[] data)
        {
            EncryptedData result;
            using (var ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var (encryptedData, msEncrypt) = await EncryptToStream(ms);
                encryptedData.AESEncryptedData = msEncrypt.ToArray();
                result = encryptedData;
            }

            return result;
        }

        public async Task<(EncryptedData, MemoryStream)> EncryptToStream(Stream stream)
        {
            AES.GenerateIV();
            AES.GenerateKey();

            var msEncrypt = new MemoryStream();
            using (var encryptor = AES.CreateEncryptor())
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                await stream.CopyToAsync(csEncrypt);
            }

            var aesKeyData = new AESKeyData {IV = AES.IV, Key = AES.Key};
            var aesKeyDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(aesKeyData));
            var encryptedData = new EncryptedData
            {
                RSAEncryptedAESKey = RSA.Encrypt(aesKeyDataBytes, RSAEncryptionPadding.Pkcs1)
            };
            return (encryptedData, msEncrypt);
        }
    }
}