using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaLib.Model
{
    [Table("Group")]
    public class Group
    {
        [Key] public int GroupId { get; set; }

        public List<User> Users { get; set; }
    }
}