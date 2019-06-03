using System.Net.Http;
using System.Threading.Tasks;
using EnigmaLib.Model;

namespace EnigmaLib.API
{
    public class GroupAPI : APIBase
    {
        public async Task<Group> GetGroupAsync(int groupId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/group/{groupId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Group>();
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{EndPoint}/group");
            GenerateAuth(request);
            AddJsonContent(request, group);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Group>();
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{EndPoint}/group/{group.GroupId}");
            GenerateAuth(request);
            AddJsonContent(request, group);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Group>();
        }
    }
}