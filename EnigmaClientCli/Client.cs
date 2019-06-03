using System;
using System.Linq;
using System.Threading.Tasks;

namespace EnigmaClientCli
{
    public class Client
    {
        public UserInfo Me { get; set; }

        public async void Run()
        {
            Console.WriteLine("Enigma Cli 1.0");
            Console.WriteLine("type 'help' to get all usable commands");
            await Loop();
        }

        private async Task Loop()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                await ParseCommandsAsync(input);
            }
        }

        private async Task ParseCommandsAsync(string input)
        {
            var commandList = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (commandList.Count == 0) return;
            try
            {
                switch (commandList[0])
                {
                    case "g":
                    case "group":
                        if (commandList.Count == 1)
                            await ListGroupAsync();
                        else
                            switch (commandList[1])
                            {
                                case "add":
                                    if (commandList.Count != 4)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    var inviteId = int.Parse(commandList[2]);
                                    await AddGroupAsync(inviteId, commandList[3]);
                                    break;
                                case "invite":
                                    if (commandList.Count != 3)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    var groupId = int.Parse(commandList[2]);
                                    break;
                            }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task ListGroupAsync()
        {
            var me = await Global.APIBase.CreateUserAPI().GetMeAsync();
            var groupAPI = Global.APIBase.CreateGroupAPI();

            foreach (var groupI in me.GroupUsers.OrderBy(t => t.GroupId)
                .Select((t, index) => new {t.GroupId, Index = index}))
            {
                var group = await groupAPI.GetGroupAsync(groupI.GroupId);
                Console.WriteLine($"{groupI.Index}: {group.GroupName}");
            }
        }

        private async Task AddGroupAsync(int inviteId, string inviteCode)
        {
            var api = Global.APIBase.CreateGroupInviteLinkAPI();
            await api.EnterGroupInviteLinkAsync(inviteId, inviteCode);
            await ListGroupAsync();
        }

        private async Task CreateInviteAsync(int groupId)
        {
            var api = Global.APIBase.CreateGroupInviteLinkAPI();
            var invite = await api.CreateGroupInviteLinkAPI().GetGroupInviteLinkAsync(groupId);
            Console.WriteLine($"InviteId: {invite.GroupInviteLinkId} InviteCode: {invite.InviteCode}");
        }
    }
}