using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaLib
{
    public class DecryptHelper
    {
        public DecryptHelper(RSAParameters rsaParameters)
        {
            RSA.ImportParameters(rsaParameters);
            AES.KeySize = 256;
            AES.Mode = CipherMode.CBC;
        }

        private RSA RSA { get; } = RSA.Create();
        private Aes AES { get; } = Aes.Create();

        public async Task<T> Decrypt<T>(EncryptedData encryptedData)
        {
            return JsonConvert.DeserializeObject<T>(await DecryptToString(encryptedData));
        }

        public async Task<string> DecryptToString(EncryptedData encryptedData)
        {
            using (var stream = await DecryptFromStream(encryptedData))
            {
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<byte[]> DecryptToBytes(EncryptedData encryptedData)
        {
            using (var stream = await DecryptFromStream(encryptedData))
            {
                return stream.ToArray();
            }
        }

        public async Task<MemoryStream> DecryptFromStream(EncryptedData encryptedData, Stream stream = null)
        {
            var aesKeyDataJson =
                Encoding.UTF8.GetString(RSA.Decrypt(encryptedData.RSAEncryptedAESKey, RSAEncryptionPadding.Pkcs1));
            var aesKeyData = JsonConvert.DeserializeObject<AESKeyData>(aesKeyDataJson);
            AES.IV = aesKeyData.IV;
            AES.Key = aesKeyData.Key;

            if (stream == null)
                stream = new MemoryStream(encryptedData.AESEncryptedData);

            var msDecrypt = new MemoryStream();
            using (var decryptor = AES.CreateDecryptor())
            using (var csDecrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
            {
                await csDecrypt.CopyToAsync(msDecrypt);
                msDecrypt.Seek(0, SeekOrigin.Begin);
            }

            return msDecrypt;
        }
    }
}