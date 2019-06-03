using System;
using System.Collections.Generic;
using System.Text;
using EnigmaLib;
using EnigmaLib.Model;

namespace EnigmaClientCli
{
    public static class UserInfoFactory
    {
        public static Dictionary<int,UserInfo> UserInfos { get; set; }

        public static UserInfo Create(int userId)
        {
            if (!UserInfos.ContainsKey(userId))
            {
                var userInfo = new UserInfo(userId);
                userInfo.EncryptHelper = new EncryptHelper(userInfo.User.PublicKey);
                UserInfos.Add(userInfo.User.UserId, userInfo);
            }

            return UserInfos[userId];
        }
    }
}
