﻿using System;
using System.Net.Http;
using EnigmaLib;
using EnigmaLib.Model;
using Newtonsoft.Json;

namespace EnigmaClientCli
{
    public class UserInfo : IEquatable<UserInfo>
    {
        public User User { get; set; }
        [JsonIgnore] public EncryptHelper EncryptHelper { get; set; }
        [JsonIgnore] public DecryptHelper DecryptHelper { get; set; }

        /// <summary>
        /// Init User from API
        /// </summary>
        /// <param name="userId"></param>
        public UserInfo(int userId)
        {
            var api = Global.APIBase.CreateUserAPI();
            User = api.GetUserAsync(userId).Result;
            EncryptHelper = new EncryptHelper(User.PublicKey);
        }

        /// <summary>
        /// Init User using existed Info
        /// </summary>
        /// <param name="user"></param>
        public UserInfo(User user)
        {
            User = user;
            EncryptHelper = new EncryptHelper(User.PublicKey);
        }

        [JsonConstructor]
        private UserInfo() { }

        public void ReInit()
        {
            if (EncryptHelper == null)
                EncryptHelper = new EncryptHelper(User.PublicKey);
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