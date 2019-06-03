using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using EnigmaLib;
using EnigmaLib.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace EnigmaTest
{
    [TestFixture]
    public class ServerTest
    {
        private bool DoInitServer => true;
        private Process ServerProcess { get; set; }
        private string EndPoint => "http://localhost:5000/";
        private string ServerPath => "../../../../EnigmaServer/";
        private HttpClient HttpClient => new HttpClient();

        [OneTimeSetUp]
        public void InitServer()
        {
            if (!DoInitServer) return;
            ServerProcess = new Process
            {
                StartInfo = new ProcessStartInfo("dotnet", "run")
                {
                    WorkingDirectory = Path.Combine(Environment.CurrentDirectory, ServerPath),
                    UseShellExecute = true
                }
            };
            ServerProcess.Start();
            Thread.Sleep(2000);
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            if (!DoInitServer) return;
            ServerProcess.Kill();
            ServerProcess.Close();
        }

        [TestCase("arition")]
        [TestCase("rev")]
        public void TestAddUser(string username, RSAParameters? publicRSAParameters = null)
        {
            if (publicRSAParameters == null)
                (_, publicRSAParameters) = GenerateRSAKey();

            var user = new User
            {
                Username = username,
                PublicKey = publicRSAParameters.Value
            };
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint + "api/user");
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            var response = HttpClient.SendAsync(request).Result;
            var result = response.Content.ReadAsAsync<User>().Result;
            //response = HttpClient.GetAsync(EndPoint + "api/user/" + result.UserId).Result;
            //result = response.Content.ReadAsAsync<User>().Result;
            Assert.AreEqual(user.Username, result.Username);
            Assert.IsTrue(user.PublicKey.IsEqual(result.PublicKey));
        }

        private (RSAParameters, RSAParameters) GenerateRSAKey()
        {
            RSAParameters privateRSAParameters, publicRSAParameters;
            using (var rsa = RSA.Create())
            {
                privateRSAParameters = rsa.ExportParameters(true);
                publicRSAParameters = rsa.ExportParameters(false);
            }

            return (privateRSAParameters, publicRSAParameters);
        }

        private HttpStatusCode TestAuthSendRequest(SignedData signedData)
        {
            var authData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedData)));
            var request = new HttpRequestMessage(HttpMethod.Get, EndPoint + "api/user/" + 1);
            request.Headers.Add("Authorization", $"Bearer {authData}");
            var response = HttpClient.SendAsync(request).Result;
            return response.StatusCode;
        }

        [Test]
        public void TestAddGroup()
        {
            var (privateRSAParameters, publicRSAParameters) = GenerateRSAKey();
            TestAddUser("adam", publicRSAParameters);
            var signedData = new SignedData
            {
                Content = Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                PublicKey = publicRSAParameters
            };
            signedData.GenerateSignedData(privateRSAParameters);
            var authData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedData)));
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint + "api/group");
            request.Headers.Add("Authorization", $"Bearer {authData}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Group {GroupName = "test"}),
                Encoding.UTF8, "application/json");
            var response = HttpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public void TestAddUserToGroup()
        {
            var (privateRSAParameters, publicRSAParameters) = GenerateRSAKey();
            TestAddUser("adam", publicRSAParameters);
            var signedData = new SignedData
            {
                Content = Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                PublicKey = publicRSAParameters
            };
            signedData.GenerateSignedData(privateRSAParameters);
            var authData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedData)));
            var request = new HttpRequestMessage(HttpMethod.Post, EndPoint + "api/group");
            request.Headers.Add("Authorization", $"Bearer {authData}");
            request.Content = new StringContent(JsonConvert.SerializeObject(new Group {GroupName = "test"}),
                Encoding.UTF8, "application/json");
            var response = HttpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var group = JsonConvert.DeserializeObject<Group>(response.Content.ReadAsStringAsync().Result);
            request = new HttpRequestMessage(HttpMethod.Get, EndPoint + "api/invite/create/" + group.GroupId);
            request.Headers.Add("Authorization", $"Bearer {authData}");
            response = HttpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var inviteLink =
                JsonConvert.DeserializeObject<GroupInviteLink>(response.Content.ReadAsStringAsync().Result);

            var (privateRSAParameters2, publicRSAParameters2) = GenerateRSAKey();
            TestAddUser("charles", publicRSAParameters2);
            signedData = new SignedData
            {
                Content = Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                PublicKey = publicRSAParameters2
            };
            signedData.GenerateSignedData(privateRSAParameters2);
            authData = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(signedData)));
            request = new HttpRequestMessage(HttpMethod.Get,
                EndPoint + $"api/invite/enter/{inviteLink.GroupInviteLinkId}/{inviteLink.InviteCode}");
            request.Headers.Add("Authorization", $"Bearer {authData}");
            response = HttpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var inviteLink2 = JsonConvert.DeserializeObject<GroupInviteLink>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(group.GroupId, inviteLink2.GroupId);


            request = new HttpRequestMessage(HttpMethod.Get,
                EndPoint + $"api/group/1");
            request.Headers.Add("Authorization", $"Bearer {authData}");
            response = HttpClient.SendAsync(request).Result;
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

        [Test]
        public void TestAuth()
        {
            var (privateRSAParameters, publicRSAParameters) = GenerateRSAKey();
            TestAddUser("adam", publicRSAParameters);
            var signedData = new SignedData
            {
                Content = Encoding.UTF8.GetBytes(DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                PublicKey = publicRSAParameters
            };
            signedData.GenerateSignedData(privateRSAParameters);
            Assert.AreEqual(HttpStatusCode.OK, TestAuthSendRequest(signedData));

            signedData.Content = Encoding.UTF8.GetBytes((DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 40).ToString());
            signedData.GenerateSignedData(privateRSAParameters);
            Assert.AreEqual(HttpStatusCode.OK, TestAuthSendRequest(signedData));

            var badSignedData = (SignedData) signedData.Clone();
            badSignedData.SHA256Hash[0] = 0;
            Assert.AreEqual(HttpStatusCode.Unauthorized, TestAuthSendRequest(badSignedData));

            badSignedData = (SignedData) signedData.Clone();
            badSignedData.Signature[0] = 0;
            Assert.AreEqual(HttpStatusCode.Unauthorized, TestAuthSendRequest(badSignedData));

            badSignedData = (SignedData) signedData.Clone();
            badSignedData.PublicKey.Modulus[0] = 0;
            Assert.AreEqual(HttpStatusCode.Unauthorized, TestAuthSendRequest(badSignedData));
        }
    }
}