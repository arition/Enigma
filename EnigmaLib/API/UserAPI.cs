using System.Net.Http;
using System.Threading.Tasks;
using EnigmaLib.Model;

namespace EnigmaLib.API
{
    public class UserAPI : APIBase
    {
        public async Task<User> GetMeAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/user/me");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/user/{userId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{EndPoint}/user");
            AddJsonContent(request, user);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<User>();
        }
    }
}