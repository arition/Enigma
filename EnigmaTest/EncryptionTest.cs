using System;
using System.Linq;
using System.Security.Cryptography;
using EnigmaLib;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EnigmaTest
{
    public class EncryptionTest
    {
        private RSAParameters PrivateRSAParameters { get; set; }
        private RSAParameters PublicRSAParameters { get; set; }
        private static Random Random { get; } = new Random();

        //test for setup the RSA initialization
        [SetUp]
        public void Setup()
        {
            var rsa = RSA.Create();
            PrivateRSAParameters = rsa.ExportParameters(true);
            PublicRSAParameters = rsa.ExportParameters(false);
        }

        //test for setup the encryption environment
        [Test]
        public void Test()
        {
            var encryptor = new EncryptHelper(PublicRSAParameters);
            var decryptor = new DecryptHelper(PrivateRSAParameters);
            var str = RandomStringGen();
            var encData = encryptor.Encrypt(str).Result;
            var resultStr = decryptor.DecryptToString(encData).Result;
            Assert.AreEqual(str, resultStr);
        }

        //test for RSA parameter
        [Test]
        public void RSAParameterTest()
        {
            var privateKeyJson = JsonConvert.SerializeObject(PrivateRSAParameters);
            var publicKeyJson = JsonConvert.SerializeObject(PublicRSAParameters);
            var privateKey = JsonConvert.DeserializeObject<RSAParameters>(privateKeyJson);
            var publicKey = JsonConvert.DeserializeObject<RSAParameters>(publicKeyJson);
            Assert.IsTrue(privateKey.IsEqual(PrivateRSAParameters));
            Assert.IsTrue(publicKey.IsEqual(PublicRSAParameters));
        }

        private string RandomStringGen(int length = 100)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
