using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaLib.API
{
    public class APIBase
    {
        public string EndPoint => "http://127.0.0.1:5000/api";
        public static HttpClient HttpClient { get; set; } = new HttpClient();
        public RSAParameters PrivateKey { get; set; }

        protected virtual void GenerateAuth(HttpRequestMessage httpRequest)
        {
            var signedData = new SignedData
            {
                Content = Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                PublicKey = PrivateKey.ToPublicKey()
            };
            signedData.GenerateSignedData(PrivateKey);
            var authData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedData)));
            httpRequest.Headers.Add("Authorization", $"Bearer {authData}");
        }

        protected virtual void AddJsonContent<T>(HttpRequestMessage httpRequestMessage, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        public virtual UserAPI CreateUserAPI()
        {
            return new UserAPI {PrivateKey = PrivateKey};
        }

        public virtual GroupAPI CreateGroupAPI()
        {
            return new GroupAPI { PrivateKey = PrivateKey };
        }

        public virtual GroupInviteLinkAPI CreateGroupInviteLinkAPI()
        {
            return new GroupInviteLinkAPI { PrivateKey = PrivateKey };
        }

        public virtual MessageAPI CreateMessageAPI()
        {
            return new MessageAPI { PrivateKey = PrivateKey };
        }
    }
}
