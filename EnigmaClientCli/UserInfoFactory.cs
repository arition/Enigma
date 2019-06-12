using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnigmaLib;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaClientCli
{
    public static class UserInfoFactory
    {
        public static Dictionary<int, UserInfo> UserInfos { get; set; } = new Dictionary<int, UserInfo>();

        private static bool _inited;

        public static void Init()
        {
            if (_inited) return;
            try
            {
                if (!File.Exists("user.json")) return;
                using (var file = File.OpenText("user.json"))
                {
                    UserInfos = JsonConvert.DeserializeObject<Dictionary<int, UserInfo>>(file.ReadToEnd());
                }

                if (UserInfos == null)
                    UserInfos = new Dictionary<int, UserInfo>();

                foreach (var userInfo in UserInfos)
                {
                    userInfo.Value.ReInit();
                }

                _inited = true;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Failed to load key library, Reason: \n{e}");
            }
        }

        public static void Save()
        {
            using (var file = File.CreateText("user.json"))
            {
                file.Write(JsonConvert.SerializeObject(UserInfos));
            }
        }

        public static UserInfo Create(int userId)
        {
            Init();
            if (!UserInfos.ContainsKey(userId))
            {
                var userInfo = new UserInfo(userId);
                var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(userInfo.User.PublicKeyString));
                Console.WriteLine($"New user detected: {userInfo.User.Username}, Public Key: {encoded}");
                UserInfos.Add(userInfo.User.UserId, userInfo);
                Save();
            }

            return UserInfos[userId];
        }

        public static UserInfo Create(User user)
        {
            Init();
            if (!UserInfos.ContainsKey(user.UserId))
            {
                var userInfo = new UserInfo(user);
                UserInfos.Add(userInfo.User.UserId, userInfo);
                Save();
            }
            else
            {
                UserInfos[user.UserId].User = user;
            }

            return UserInfos[user.UserId];
        }
    }
}
