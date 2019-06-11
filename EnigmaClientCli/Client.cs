using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EnigmaLib;
using EnigmaLib.Model;

namespace EnigmaClientCli
{
    public class Client
    {
        //some functions in class client
        public UserInfo Me { get; set; }
        public List<GroupInfo> GroupInfo { get; set; } = new List<GroupInfo>();

        public async void Run()
        {
            Console.WriteLine("Enigma Cli 1.0");
            Console.WriteLine("type 'help' to get all usable commands");
            await InitMeAsync();
            await InitGroupAsync();
            await Loop();
        }
        //basic UI and readin
        private async Task Loop()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                await ParseCommandsAsync(input);
                Console.WriteLine("----------------------");
            }
        }
        //handle the input and error report
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
                                case "info":
                                    if (commandList.Count != 3)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    var groupNo = int.Parse(commandList[2]);
                                    await GetGroupAsync(groupNo);
                                    break;
                                case "enter":
                                    if (commandList.Count != 4)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    var inviteId = int.Parse(commandList[2]);
                                    await EnterGroupAsync(inviteId, commandList[3]);
                                    break;
                                case "create-invite":
                                    if (commandList.Count != 3)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    var groupId = int.Parse(commandList[2]);
                                    await CreateInviteAsync(groupId);
                                    break;
                                case "create":
                                    if (commandList.Count != 3)
                                    {
                                        Console.WriteLine("Invalid Argument");
                                        return;
                                    }

                                    await CreateGroupAsync(new Group {GroupName = commandList[2]});
                                    break;
                                case string num when int.TryParse(num, out _):
                                    var i = int.Parse(num);
                                    if (commandList.Count > 2)
                                    {
                                        var text = string.Join(" ", commandList.Skip(2));
                                        await SendMessagesAsync(i, new TextMessageContent {Text = text});
                                    }
                                    else
                                    {
                                        await GetMessagesAsync(i);
                                    }

                                    break;
                            }
                        break;
                    default:
                        Console.WriteLine("Unknown operation.");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //initiation of me and group
        private async Task InitMeAsync()
        {
            Me = new UserInfo(await Global.APIBase.CreateUserAPI().GetMeAsync());
            Me.DecryptHelper = Me.DecryptHelper ?? new DecryptHelper(Global.APIBase.PrivateKey);
        }

        private async Task InitGroupAsync()
        {
            await InitMeAsync();
            var groupAPI = Global.APIBase.CreateGroupAPI();

            GroupInfo.Clear();
            foreach (var groupI in Me.User.GroupUsers.OrderBy(t => t.GroupId))
            {
                var group = await groupAPI.GetGroupAsync(groupI.GroupId);
                GroupInfo.Add(new GroupInfo(group));
            }
        }

        private async Task InitGroupAsync(int groupNo)
        {
            var groupAPI = Global.APIBase.CreateGroupAPI();
            GroupInfo[groupNo] = new GroupInfo(await groupAPI.GetGroupAsync(GroupInfo[groupNo].Group.GroupId));
        }

        private async Task ListGroupAsync()
        {
            await InitGroupAsync();
            foreach (var groupInfo in GroupInfo.Select((t, index) => new {t.Group.GroupName, Index = index}))
                Console.WriteLine($"{groupInfo.Index}: {groupInfo.GroupName}");
        }

        private async Task GetGroupAsync(int groupNo)
        {
            await InitGroupAsync(groupNo);
            Console.WriteLine($"GroupName: {GroupInfo[groupNo].Group.GroupName}");
            Console.WriteLine(
                $"GroupMember: {string.Join(", ", GroupInfo[groupNo].Users.Select(t => t.User.Username))}");
        }

        private async Task EnterGroupAsync(int inviteId, string inviteCode)
        {
            var api = Global.APIBase.CreateGroupInviteLinkAPI();
            await api.EnterGroupInviteLinkAsync(inviteId, inviteCode);
            await ListGroupAsync();
        }
        //create and invite group
        private async Task CreateInviteAsync(int groupNo)
        {
            var api = Global.APIBase.CreateGroupInviteLinkAPI();
            var invite = await api.CreateGroupInviteLinkAPI().GetGroupInviteLinkAsync(GroupInfo[groupNo].Group.GroupId);
            Console.WriteLine($"InviteId: {invite.GroupInviteLinkId} InviteCode: {invite.InviteCode}");
        }

        private async Task CreateGroupAsync(Group group)
        {
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(group, new ValidationContext(group), validationResults))
            {
                Console.WriteLine("Create Group Failed: ");
                Console.WriteLine(string.Join(Environment.NewLine, validationResults));
                return;
            }

            var api = Global.APIBase.CreateGroupAPI();
            await api.CreateGroupAsync(group);
            await ListGroupAsync();
        }
        //read messages in the group
        private async Task GetMessagesAsync(int groupNo)
        {
            await InitGroupAsync(groupNo);
            var api = Global.APIBase.CreateMessageAPI();
            var msgs = await api.GetLatestMessageAsync(GroupInfo[groupNo].Group.GroupId);
            var processedMsg = msgs.Select(msg =>
            {
                var textMsg = Me.DecryptHelper.Decrypt<TextMessageContent>(msg.EncryptedData).Result;
                return $"{msg.FromUser.Username}\n{textMsg.SendTime.ToLocalTime():g}\n{textMsg.Text}\n";
            });
            Console.WriteLine(string.Join("\n", processedMsg));
        }
        //send messages in the group
        private async Task SendMessagesAsync(int groupNo, TextMessageContent message)
        {
            await InitGroupAsync(groupNo);
            var api = Global.APIBase.CreateMessageAPI();
            foreach (var userInfo in GroupInfo[groupNo].Users)
            {
                var msg = new Message
                {
                    EncryptedData = await userInfo.EncryptHelper.Encrypt(message),
                    FromUserId = Me.User.UserId,
                    ToUserId = userInfo.User.UserId,
                    GroupId = GroupInfo[groupNo].Group.GroupId
                };
                await api.PostMessageAsync(msg);
            }

            await GetMessagesAsync(groupNo);
        }
    }
}
