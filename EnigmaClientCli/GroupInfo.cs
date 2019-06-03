using System;
using System.Collections.Generic;
using System.Text;
using EnigmaLib.Model;

namespace EnigmaClientCli
{
    public class GroupInfo
    {
        public Group Group { get; set; }
        public List<UserInfo> Users { get; set; }

        public GroupInfo(Group group)
        {
            foreach (var groupUser in Group.GroupUsers)
            {
                //Users.Add(UserInfoFactory.Create(groupUser.));
            }
        }
    }
}
