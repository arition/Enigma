using System.Net.Http;
using System.Threading.Tasks;
using EnigmaLib.Model;

namespace EnigmaLib.API
{
    public class GroupInviteLinkAPI : APIBase
    {
        public async Task<GroupInviteLink> GetGroupInviteLinkAsync(int groupId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/invite/create/{groupId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<GroupInviteLink>();
        }

        public async Task<GroupInviteLink> RevokeGroupInviteLinkAsync(int inviteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{EndPoint}/invite/{inviteId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<GroupInviteLink>();
        }

        public async Task<GroupInviteLink> EnterGroupInviteLinkAsync(int inviteId, string inviteCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/invite/enter/{inviteId}/{inviteCode}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<GroupInviteLink>();
        }
    }
}