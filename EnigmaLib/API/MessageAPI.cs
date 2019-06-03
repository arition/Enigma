using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EnigmaLib.Model;

namespace EnigmaLib.API
{
    public class MessageAPI : APIBase
    {
        public async Task<List<Message>> GetLatestMessageAsync(int groupId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/message/latest/{groupId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Message>>();
        }

        public async Task<List<Message>> GetPrevMessageAsync(int groupId, int messageId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/message/prev/{groupId}/{messageId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Message>>();
        }

        public async Task<List<Message>> GetNextMessageAsync(int groupId, int messageId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{EndPoint}/message/next/{groupId}/{messageId}");
            GenerateAuth(request);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Message>>();
        }

        public async Task<Message> PostMessageAsync(Message message)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{EndPoint}/message");
            GenerateAuth(request);
            AddJsonContent(request, message);
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Message>();
        }
    }
}