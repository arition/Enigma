using System;
using System.Collections.Generic;
using System.Text;
using EnigmaLib;
using EnigmaLib.Model;

namespace EnigmaClientCli
{
    public static class UserInfoFactory
    {
        public static Dictionary<int,UserInfo> UserInfos { get; set; } = new Dictionary<int, UserInfo>();

        public static UserInfo Create(int userId)
        {
            if (!UserInfos.ContainsKey(userId))
            {
                var userInfo = new UserInfo(userId);
                UserInfos.Add(userInfo.User.UserId, userInfo);
            }

            return UserInfos[userId];
        }

        public static UserInfo Create(User user)
        {
            if (!UserInfos.ContainsKey(user.UserId))
            {
                var userInfo = new UserInfo(user);
                UserInfos.Add(userInfo.User.UserId, userInfo);
            }

            return UserInfos[user.UserId];
        }
    }
}
