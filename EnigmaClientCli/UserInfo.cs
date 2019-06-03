using System;
using System.Net.Http;
using EnigmaLib;
using EnigmaLib.Model;

namespace EnigmaClientCli
{
    public class UserInfo : IEquatable<UserInfo>
    {
        public User User { get; set; }
        public EncryptHelper EncryptHelper { get; set; }
        public DecryptHelper DecryptHelper { get; set; }

        /// <summary>
        /// Init User from API
        /// </summary>
        /// <param name="userId"></param>
        public UserInfo(int userId)
        {
            //httpClient.GetStringAsync()
        }

        /// <summary>
        /// Init User using existed Info
        /// </summary>
        /// <param name="user"></param>
        public UserInfo(User user)
        {
            User = user;
        }

        public bool Equals(UserInfo other)
        {
            return User.UserId == other.User.UserId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserInfo) obj);
        }

        public override int GetHashCode()
        {
            return User.UserId;
        }
    }
}